#version 330 core
out float FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform vec3 samples[64];

// parameters (如果用它们作为uniforms，会更容易调整效果)
int kernelSize = 64;
float radius = 0.5; //1.0 or 0.5
float bias = 0.025;

//求出在屏幕上铺满噪声纹理所需缩放比例，屏幕尺寸除以噪声纹理大小
const vec2 noiseScale = vec2(800.0/4.0, 600.0/4.0); 

uniform mat4 projection;

void main()
{
    //获取输入值
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = normalize(texture(gNormal, TexCoords).rgb);
    vec3 randomVec = normalize(texture(texNoise, TexCoords * noiseScale).xyz);
    //创建TBN矩阵，将向量从切线空间变换到观察空间
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal)); //使用Gramm-Schmidt Process，创建一个正交基(Orthogonal Basis)，每一次都会根据randomVec的值稍微倾斜
    //注意因为我们使用了一个随机向量来构造切线向量，我们没必要有一个恰好沿着几何体表面的TBN矩阵，也就是不需要逐顶点切线(和双切)向量
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);
    //对每个核心样本进行迭代，计算遮挡因子
    float occlusion = 0.0;
    for(int i = 0; i < kernelSize; ++i)
    {
        //获取sample位置
        vec3 samplePos = TBN * samples[i]; //切线->观察空间
        //用radius乘上偏移样本来增加(或减少)有效取样半径，再加到片段位置上
        samplePos = fragPos + samplePos * radius;
        
        //变换sample位置到屏幕空间，从而可以像“直接渲染它的位置到屏幕上”一样获取sample的(线性)深度值
        vec4 offset = vec4(samplePos, 1.0);
        offset = projection * offset; //观察->裁剪空间(屏幕空间)
        offset.xyz /= offset.w; //透视除法
        offset.xyz = offset.xyz * 0.5 + 0.5; //变换到0.0-1.0范围内
        
        //获取sample(第一个不被遮挡的可见片段)深度值
        float sampleDepth = texture(gPosition, offset.xy).z; // get depth value of kernel sample
        
        //检查sample样本当前深度值是否大于gPosition中存储的深度值，如果是，则添加到最终的遮挡因子上
        //当检测一个靠近表面边缘的片段时，它将会获取测试表面之下的其他表面的深度值，这些值将会不正确地影响遮蔽因子，所以需要引入一个范围测试从而保证只有当被测深度值在取样半径内时才会影响遮蔽因子
        //使用GLSL的smoothstep函数，它非常光滑地在第一和第二个参数范围内插值第三个参数。
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(fragPos.z - sampleDepth));
        occlusion += (sampleDepth >= samplePos.z + bias ? 1.0 : 0.0) * rangeCheck;
    }
    //将遮挡因子根据核心的大小标准化，并输出结果。用1.0减去遮蔽因子，以便直接使用遮蔽因子去缩放环境光照分量
    occlusion = 1.0 - (occlusion / kernelSize);
    
    FragColor = occlusion;
}
