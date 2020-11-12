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
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    //Blinn-Phong�����ģ��Ψһ��������ǣ�Blinn-Phong�������Ƿ�����������֮��ļнǣ�������ģ�Ͳ������ǹ۲췽���뷴��������ļнǡ�
    if(blinn)
    {
        //��ȡ�������
        vec3 halfwayDir = normalize(lightDir + viewDir);
        //���㾵������:�Ա��淨�ߺͰ����������һ��Լ�����(Clamped Dot Product)���õ�˽����Ϊ�����Ӷ���ȡ����֮��нǵ�����ֵ��֮������ֵȡ����ȴη�
        spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

        //����ģ����Blinn-Phongģ��Ҳ��һЩϸ΢�Ĳ�𣺰����������淨�ߵļн�ͨ����С�ڹ۲��뷴�������ļнǡ����ԣ�������úͷ�����ɫ���Ƶ�Ч�����ͱ�����ʹ��Blinn-Phongģ��ʱ�����淴������ø���һ�㡣ͨ�����ǻ�ѡ�������ɫʱ����ȷ�����2��4����
    }
    else
    {
        vec3 reflectDir = reflect(-lightDir, normal);
        spec = pow(max(dot(viewDir, reflectDir), 0.0), 8.0);
    }
    vec3 specular = vec3(0.3) * spec; // assuming bright white light color
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}