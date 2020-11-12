#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D floorTexture;

uniform vec3 lightPositions[4];
uniform vec3 lightColors[4];
uniform vec3 viewPos;
uniform bool gamma;

vec3 BlinnPhong(vec3 normal, vec3 fragPos, vec3 lightPos, vec3 lightColor)
{
    // diffuse
    vec3 lightDir = normalize(lightPos - fragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;
    // simple attenuation
    float max_distance = 1.5;
    float distance = length(lightPos - fragPos);

    //真实的物理世界中，光照的衰减和光源的距离的平方成反比。
    //float attenuation = 1.0 / (distance * distance);
    //然而，当我们使用这个衰减公式的时候，衰减效果总是过于强烈，光只能照亮一小圈，看起来并不真实。出于这个原因，我们使用在基本光照教程投光物中所讨论的那种衰减方程，它给了我们更大的控制权，此外我们还可以使用双曲线函数：
    //float attenuation = 1.0 / distance;

    //Gamma校正产生的第二个问题就是衰减的计算。之前我们用到的衰减计算是与距离的倒数成正比，因为这样光线不会衰减的过快，看上去更真实。如果用上Gamma校正，那么衰减计算要用与距离平方的倒数成正比才合适，否则会出现场景过亮的问题。
    //当我们不使用Gamma校正，使用距离平方的倒数时，衰减的计算就成为这样：(1.0 / (distance ^ 2))^2.2。由于显示器本身的Gamma值关系，亮度衰减会急剧变大，光斑看上去就是非常小的一块。如果使用距离的倒数，衰减的计算就是这样：(1.0 / distance)^2.2 = 1.0 / distance^2.2。这和正常的衰减物理公式很相似。
    //当我们使用Gamma校正，就需要使用distance * distance，计算结果为：[(1.0 / (distance ^ 2))^2.2]^(1/2.2) = 1.0 / (distance ^ 2)，跟物理公式一致

    float attenuation = 1.0 / (gamma ? distance * distance : distance);
    
    diffuse *= attenuation;
    specular *= attenuation;
    
    return diffuse + specular;
}

void main()
{           
    vec3 color = texture(floorTexture, fs_in.TexCoords).rgb;
    vec3 lighting = vec3(0.0);
    for(int i = 0; i < 4; ++i) {
        lighting += BlinnPhong(normalize(fs_in.Normal), fs_in.FragPos, lightPositions[i], lightColors[i]);
    }
    color *= lighting;
    if(gamma) {
        // 进行Gamma校正，Gamma校正的运算必须在片元着色器的末尾运行才正确
        color = pow(color, vec3(1.0/2.2));
    }
    FragColor = vec4(color, 1.0);
}