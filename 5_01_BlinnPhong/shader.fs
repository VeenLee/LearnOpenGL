#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D floorTexture;
uniform vec3 lightPos;
uniform vec3 viewPos;
uniform bool blinn;

void main()
{           
    vec3 color = texture(floorTexture, fs_in.TexCoords).rgb;
    // ambient
    vec3 ambient = 0.05 * color;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    vec3 normal = normalize(fs_in.Normal);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    float spec = 0.0;
    //Blinn-Phong与冯氏模型唯一的区别就是，Blinn-Phong测量的是法线与半程向量之间的夹角，而冯氏模型测量的是观察方向与反射向量间的夹角。
    if(blinn)
    {
        //获取半程向量
        vec3 halfwayDir = normalize(lightDir + viewDir);
        //计算镜面光分量:对表面法线和半程向量进行一次约束点乘(Clamped Dot Product)，让点乘结果不为负，从而获取它们之间夹角的余弦值，之后对这个值取反光度次方
        spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

        //冯氏模型与Blinn-Phong模型也有一些细微的差别：半程向量与表面法线的夹角通常会小于观察与反射向量的夹角。所以，如果想获得和冯氏着色类似的效果，就必须在使用Blinn-Phong模型时将镜面反光度设置更高一点。通常我们会选择冯氏着色时反光度分量的2到4倍。
    }
    else
    {
        vec3 reflectDir = reflect(-lightDir, normal);
        spec = pow(max(dot(viewDir, reflectDir), 0.0), 8.0);
    }
    vec3 specular = vec3(0.3) * spec; // assuming bright white light color
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}
