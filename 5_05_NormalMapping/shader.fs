#version 330 core
out vec4 FragColor;

//TBN矩阵用法1：直接使用
//in VS_OUT {
//    vec3 FragPos;
//    vec2 TexCoords;
//    mat3 TBN;
//} fs_in;

//TBN矩阵用法2：使用逆矩阵
in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 TangentLightPos;
    vec3 TangentViewPos;
    vec3 TangentFragPos;
} fs_in;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{           
    // obtain normal from normal map in range [0,1]
    vec3 normal = texture(normalMap, fs_in.TexCoords).rgb;
    // transform normal vector to range [-1,1]
    normal = normalize(normal * 2.0 - 1.0);  // this normal is in tangent space
   
    // get diffuse color
    vec3 color = texture(diffuseMap, fs_in.TexCoords).rgb;
    // ambient
    vec3 ambient = 0.1 * color;

    // diffuse

    //TBN矩阵用法1：直接使用
    //normal = normalize(fs_in.TBN * normal);
    //vec3 lightDir = normalize(lightPos - fs_in.FragPos);

    //TBN矩阵用法2：使用逆矩阵
    vec3 lightDir = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);


    //为何要用第二种方法呢？    
    //将向量从世界空间转换到切线空间有个额外好处，我们可以把所有相关向量在顶点着色器中转换到切线空间，不用在像素着色器中做这件事。这是可行的，因为lightPos和viewPos不是每个fragment运行都要改变，对于fs_in.FragPos，我们也可以在顶点着色器计算它的切线空间位置。基本上，不需要把任何向量在像素着色器中进行变换，而第一种方法中就是必须的，因为采样出来的法线向量对于每个像素着色器都不一样。
    //所以现在不是把TBN矩阵的逆矩阵发送给像素着色器，而是将切线空间的光源位置，观察位置以及顶点位置发送给像素着色器。这样我们就不用在像素着色器里进行矩阵乘法了。这是一个极佳的优化，因为顶点着色器通常比像素着色器运行的少。这也是为什么这种方法是一种更好的实现方式的原因。


    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;

    // specular
    //vec3 viewDir = normalize(viewPos - fs_in.FragPos); //TBN矩阵用法1：直接使用
    vec3 viewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos); //TBN矩阵用法2：使用逆矩阵
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

    vec3 specular = vec3(0.2) * spec;
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}