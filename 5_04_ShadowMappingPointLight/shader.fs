#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D diffuseTexture;
uniform samplerCube depthMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

uniform float far_plane;
uniform bool shadows;


//偏移方向数组
vec3 gridSamplingDisk[20] = vec3[]
(
   vec3(1, 1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1, 1,  1), 
   vec3(1, 1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1, 1, -1),
   vec3(1, 1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1, 1,  0),
   vec3(1, 0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1, 0, -1),
   vec3(0, 1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0, 1, -1)
);

float ShadowCalculation(vec3 fragPos)
{
    // get vector between fragment position and light position
    vec3 fragToLight = fragPos - lightPos;


    ////根据"片段到光源坐标原点向量"计算片段(线性)深度值
    //float currentDepth = length(fragToLight);
    ////根据"片段到光源坐标原点向量"采样深度纹理
    //float closestDepth = texture(depthMap, fragToLight).r;
    ////将深度值从(线性)纹理数据[0,1]恢复到原始深度值
    //closestDepth *= far_plane;
    ////简单计算阴影
    //float bias = 0.05; // we use a much larger bias since depth is now in [near_plane, far_plane] range
    //float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;


    ////根据"片段到光源坐标原点向量"计算片段(线性)深度值
    //float currentDepth = length(fragToLight);
    ////百分比渐进过滤（PCF，percentage-closer filtering）
    //float shadow = 0.0;
    //float bias = 0.05; 
    //float samples = 4.0;
    //float offset = 0.1;
    ////动态计算纹理偏移量，在三个轴向各采样samples次，最后对子样本进行平均化
    //for(float x = -offset; x < offset; x += offset / (samples * 0.5))
    //{
    //    for(float y = -offset; y < offset; y += offset / (samples * 0.5))
    //    {
    //        for(float z = -offset; z < offset; z += offset / (samples * 0.5))
    //        {
    //            float closestDepth = texture(depthMap, fragToLight + vec3(x, y, z)).r; //采样时加入偏移量
    //            closestDepth *= far_plane;   // Undo mapping [0,1]
    //            if(currentDepth - bias > closestDepth)
    //                shadow += 1.0;
    //        }
    //    }
    //}
    //shadow /= (samples * samples * samples);


    //然而，当samples设置为4.0时每个fragment需要采样64次，开销很大，这些采样大多数都是多余的，与其在原始方向向量附近处采样，不如在采样方向向量的垂直方向进行采样更有意义。
    //可是，没有（简单的）方式能够指出哪一个子方向是多余的。有个技巧可以使用：用一个偏移量方向数组，它们差不多都是分开的，每一个指向完全不同的方向，剔除彼此接近的那些子方向。
    //这么做的好处是与之前的PCF算法相比，我们需要的样本数量变少了。
    float shadow = 0.0;
    float bias = 0.15; //每个样本的bias（偏移）高度依赖于上下文，总是要根据场景进行微调的
    int samples = 20;
    float viewDistance = length(viewPos - fragPos);
    //基于观察者与片段的距离来改变偏移半径diskRadius，这样就能根据观察者的距离来增加偏移半径，当距离更远的时候阴影更柔和，更近就更锐利
    float diskRadius = (1.0 + (viewDistance / far_plane)) / 25.0;
    for(int i = 0; i < samples; ++i)
    {
        float closestDepth = texture(depthMap, fragToLight + gridSamplingDisk[i] * diskRadius).r;
        closestDepth *= far_plane;   // undo mapping [0;1]
        if(currentDepth - bias > closestDepth)
            shadow += 1.0;
    }
    shadow /= float(samples);


    // display closestDepth as debug (to visualize depth cubemap)
    //float closestDepth = texture(depthMap, fragToLight).r;
    //FragColor = vec4(vec3(closestDepth), 1.0);

    return shadow;
}

void main()
{           
    vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.3);
    // ambient
    vec3 ambient = 0.3 * color;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // calculate shadow
    float shadow = shadows ? ShadowCalculation(fs_in.FragPos) : 0.0;                      
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;    
    
    FragColor = vec4(lighting, 1.0);
}