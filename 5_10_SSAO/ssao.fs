#version 330 core
out float FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform vec3 samples[64];

// parameters (�����������Ϊuniforms��������׵���Ч��)
int kernelSize = 64;
float radius = 0.5; //1.0 or 0.5
float bias = 0.025;

//�������Ļ���������������������ű�������Ļ�ߴ�������������С
const vec2 noiseScale = vec2(800.0/4.0, 600.0/4.0); 

uniform mat4 projection;

void main()
{
    //��ȡ����ֵ
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = normalize(texture(gNormal, TexCoords).rgb);
    vec3 randomVec = normalize(texture(texNoise, TexCoords * noiseScale).xyz);
    //����TBN���󣬽����������߿ռ�任���۲�ռ�
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal)); //ʹ��Gramm-Schmidt Process������һ��������(Orthogonal Basis)��ÿһ�ζ������randomVec��ֵ��΢��б
    //ע����Ϊ����ʹ����һ�����������������������������û��Ҫ��һ��ǡ�����ż���������TBN����Ҳ���ǲ���Ҫ�𶥵�����(��˫��)����
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);
    //��ÿ�������������е����������ڵ�����
    float occlusion = 0.0;
    for(int i = 0; i < kernelSize; ++i)
    {
        //��ȡsampleλ��
        vec3 samplePos = TBN * samples[i]; //����->�۲�ռ�
        //��radius����ƫ������������(�����)��Чȡ���뾶���ټӵ�Ƭ��λ����
        samplePos = fragPos + samplePos * radius;
        
        //�任sampleλ�õ���Ļ�ռ䣬�Ӷ�������ֱ����Ⱦ����λ�õ���Ļ�ϡ�һ����ȡsample��(����)���ֵ
        vec4 offset = vec4(samplePos, 1.0);
        offset = projection * offset; //�۲�->�ü��ռ�(��Ļ�ռ�)
        offset.xyz /= offset.w; //͸�ӳ���
        offset.xyz = offset.xyz * 0.5 + 0.5; //�任��0.0-1.0��Χ��
        
        //��ȡsample(��һ�������ڵ��Ŀɼ�Ƭ��)���ֵ
        float sampleDepth = texture(gPosition, offset.xy).z; // get depth value of kernel sample
        
        //���sample������ǰ���ֵ�Ƿ����gPosition�д洢�����ֵ������ǣ�����ӵ����յ��ڵ�������
        //�����һ�����������Ե��Ƭ��ʱ���������ȡ���Ա���֮�µ�������������ֵ����Щֵ���᲻��ȷ��Ӱ���ڱ����ӣ�������Ҫ����һ����Χ���ԴӶ���ֻ֤�е��������ֵ��ȡ���뾶��ʱ�Ż�Ӱ���ڱ�����
        //ʹ��GLSL��smoothstep���������ǳ��⻬���ڵ�һ�͵ڶ���������Χ�ڲ�ֵ������������
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(fragPos.z - sampleDepth));
        occlusion += (sampleDepth >= samplePos.z + bias ? 1.0 : 0.0) * rangeCheck;
    }
    //���ڵ����Ӹ��ݺ��ĵĴ�С��׼����������������1.0��ȥ�ڱ����ӣ��Ա�ֱ��ʹ���ڱ�����ȥ���Ż������շ���
    occlusion = 1.0 - (occlusion / kernelSize);
    
    FragColor = occlusion;
}
