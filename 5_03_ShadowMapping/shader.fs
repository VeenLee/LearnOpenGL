#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;

uniform sampler2D diffuseTexture;
uniform sampler2D shadowMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

float ShadowCalculation(vec4 fragPosLightSpace)
{
    //通过传递过来的光源空间位置坐标，与深度图中的信息进行比较，如果大于深度图中的信息，表示该点被遮住，需要显示阴影。

    //执行透视除法
    //首先要明确一点，我们通过gl_Position传递过来的数据，OpenGL已经自动进行过perspective divide处理，就是将裁剪空间坐标[-w,w]变换成[-1,1]。方式就是将x，y，z分量都除以w分量。但是我们手动传过来的FragPosLightSpace没有自动计算过，所以我们要先进行这一步计算
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    //转换到[0,1]范围
    projCoords = projCoords * 0.5 + 0.5;
    //从阴影图中采集当前位置在光的透视视角下的深度信息
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    //获取当前片元在光的透视视角下的深度信息
    float currentDepth = projCoords.z;
    //根据表面朝向光线的角度计算阴影偏移（shadow bias）的偏移量，修复阴影失真(Shadow Acne)
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    //如果偏移量过大,会导致物体似乎与阴影有些脱离，这种现象被称为Peter Panning，因为物体看起来轻轻悬浮在表面之上。
    //shadow bias该是多少合适呢？这就取决于经验了。或许下面这行代码可以解决大部分的问题：
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    //比较当前的深度和shadowMap中的深度值大小，如果当前深度较大，则表示处在阴影中
    //float shadow = currentDepth > closestDepth ? 1.0 : 0.0; //若无偏移量，则会出现条纹状的痕迹
    //float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0; //使用了偏移量后，所有采样点都获得了比表面深度更小的深度值，整个表面就正确地被照亮

    //百分比渐进过滤（PCF，percentage-closer filtering），修复阴影的锯齿状边缘，具体的操作是对相邻的8个片元也进行采样，将这些值加起来之后除以9来决定当前片元的阴影值
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0); //由于纹理坐标范围是0.0到1.0，可以计算移动一个像素对应的距离
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;
        }
    }
    shadow /= 9.0;
    
    //当一个点比光的远平面far_plane还要远时，它的投影坐标的z坐标大于1.0，只要投影向量的z坐标大于1.0，我们就把shadow的值强制设为0.0
    if (projCoords.z > 1.0) {
        shadow = 0.0;
    }

    //将不在光源空间投影范围内的坐标全放到阴影内,仅调试用
    //if (projCoords.x < 0.0 || projCoords.y < 0.0 || projCoords.x > 1.0 || projCoords.y > 1.0) {
    //    shadow = 1.0;
    //}

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
    float shadow = ShadowCalculation(fs_in.FragPosLightSpace);                      
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;    
    
    //FragColor = vec4(vec3(shadow), 1.0);
    FragColor = vec4(lighting, 1.0);
}