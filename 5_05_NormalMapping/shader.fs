#version 330 core
out vec4 FragColor;

//TBN�����÷�1��ֱ��ʹ��
//in VS_OUT {
//    vec3 FragPos;
//    vec2 TexCoords;
//    mat3 TBN;
//} fs_in;

//TBN�����÷�2��ʹ�������
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

    //TBN�����÷�1��ֱ��ʹ��
    //normal = normalize(fs_in.TBN * normal);
    //vec3 lightDir = normalize(lightPos - fs_in.FragPos);

    //TBN�����÷�2��ʹ�������
    vec3 lightDir = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);


    //Ϊ��Ҫ�õڶ��ַ����أ�    
    //������������ռ�ת�������߿ռ��и�����ô������ǿ��԰�������������ڶ�����ɫ����ת�������߿ռ䣬������������ɫ����������¡����ǿ��еģ���ΪlightPos��viewPos����ÿ��fragment���ж�Ҫ�ı䣬����fs_in.FragPos������Ҳ�����ڶ�����ɫ�������������߿ռ�λ�á������ϣ�����Ҫ���κ�������������ɫ���н��б任������һ�ַ����о��Ǳ���ģ���Ϊ���������ķ�����������ÿ��������ɫ������һ����
    //�������ڲ��ǰ�TBN�����������͸�������ɫ�������ǽ����߿ռ�Ĺ�Դλ�ã��۲�λ���Լ�����λ�÷��͸�������ɫ�����������ǾͲ�����������ɫ������о���˷��ˡ�����һ�����ѵ��Ż�����Ϊ������ɫ��ͨ����������ɫ�����е��١���Ҳ��Ϊʲô���ַ�����һ�ָ��õ�ʵ�ַ�ʽ��ԭ��


    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;

    // specular
    //vec3 viewDir = normalize(viewPos - fs_in.FragPos); //TBN�����÷�1��ֱ��ʹ��
    vec3 viewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos); //TBN�����÷�2��ʹ�������
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

    vec3 specular = vec3(0.2) * spec;
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}