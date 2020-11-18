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


//ƫ�Ʒ�������
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


    ////����"Ƭ�ε���Դ����ԭ������"����Ƭ��(����)���ֵ
    //float currentDepth = length(fragToLight);
    ////����"Ƭ�ε���Դ����ԭ������"�����������
    //float closestDepth = texture(depthMap, fragToLight).r;
    ////�����ֵ��(����)��������[0,1]�ָ���ԭʼ���ֵ
    //closestDepth *= far_plane;
    ////�򵥼�����Ӱ
    //float bias = 0.05; // we use a much larger bias since depth is now in [near_plane, far_plane] range
    //float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;


    ////����"Ƭ�ε���Դ����ԭ������"����Ƭ��(����)���ֵ
    //float currentDepth = length(fragToLight);
    ////�ٷֱȽ������ˣ�PCF��percentage-closer filtering��
    //float shadow = 0.0;
    //float bias = 0.05; 
    //float samples = 4.0;
    //float offset = 0.1;
    ////��̬��������ƫ���������������������samples�Σ���������������ƽ����
    //for(float x = -offset; x < offset; x += offset / (samples * 0.5))
    //{
    //    for(float y = -offset; y < offset; y += offset / (samples * 0.5))
    //    {
    //        for(float z = -offset; z < offset; z += offset / (samples * 0.5))
    //        {
    //            float closestDepth = texture(depthMap, fragToLight + vec3(x, y, z)).r; //����ʱ����ƫ����
    //            closestDepth *= far_plane;   // Undo mapping [0,1]
    //            if(currentDepth - bias > closestDepth)
    //                shadow += 1.0;
    //        }
    //    }
    //}
    //shadow /= (samples * samples * samples);


    //Ȼ������samples����Ϊ4.0ʱÿ��fragment��Ҫ����64�Σ������ܴ���Щ������������Ƕ���ģ�������ԭʼ�������������������������ڲ������������Ĵ�ֱ������в����������塣
    //���ǣ�û�У��򵥵ģ���ʽ�ܹ�ָ����һ���ӷ����Ƕ���ġ��и����ɿ���ʹ�ã���һ��ƫ�����������飬���ǲ�඼�Ƿֿ��ģ�ÿһ��ָ����ȫ��ͬ�ķ����޳��˴˽ӽ�����Щ�ӷ���
    //��ô���ĺô�����֮ǰ��PCF�㷨��ȣ�������Ҫ���������������ˡ�
    float shadow = 0.0;
    float bias = 0.15; //ÿ��������bias��ƫ�ƣ��߶������������ģ�����Ҫ���ݳ�������΢����
    int samples = 20;
    float viewDistance = length(viewPos - fragPos);
    //���ڹ۲�����Ƭ�εľ������ı�ƫ�ư뾶diskRadius���������ܸ��ݹ۲��ߵľ���������ƫ�ư뾶���������Զ��ʱ����Ӱ����ͣ������͸�����
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