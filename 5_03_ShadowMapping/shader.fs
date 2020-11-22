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
    //ͨ�����ݹ����Ĺ�Դ�ռ�λ�����꣬�����ͼ�е���Ϣ���бȽϣ�����������ͼ�е���Ϣ����ʾ�õ㱻��ס����Ҫ��ʾ��Ӱ��

    //ִ��͸�ӳ���
    //����Ҫ��ȷһ�㣬����ͨ��gl_Position���ݹ��������ݣ�OpenGL�Ѿ��Զ����й�perspective divide�������ǽ��ü��ռ�����[-w,w]�任��[-1,1]����ʽ���ǽ�x��y��z����������w���������������ֶ���������FragPosLightSpaceû���Զ����������������Ҫ�Ƚ�����һ������
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    //ת����[0,1]��Χ
    projCoords = projCoords * 0.5 + 0.5;
    //����Ӱͼ�вɼ���ǰλ���ڹ��͸���ӽ��µ������Ϣ
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    //��ȡ��ǰƬԪ�ڹ��͸���ӽ��µ������Ϣ
    float currentDepth = projCoords.z;
    //���ݱ��泯����ߵĽǶȼ�����Ӱƫ�ƣ�shadow bias����ƫ�������޸���Ӱʧ��(Shadow Acne)
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    //���ƫ��������,�ᵼ�������ƺ�����Ӱ��Щ���룬�������󱻳�ΪPeter Panning����Ϊ���忴�������������ڱ���֮�ϡ�
    //shadow bias���Ƕ��ٺ����أ����ȡ���ھ����ˡ������������д�����Խ���󲿷ֵ����⣺
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    //�Ƚϵ�ǰ����Ⱥ�shadowMap�е����ֵ��С�������ǰ��Ƚϴ����ʾ������Ӱ��
    //float shadow = currentDepth > closestDepth ? 1.0 : 0.0; //����ƫ����������������״�ĺۼ�
    //float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0; //ʹ����ƫ���������в����㶼����˱ȱ�����ȸ�С�����ֵ�������������ȷ�ر�����

    //�ٷֱȽ������ˣ�PCF��percentage-closer filtering�����޸���Ӱ�ľ��״��Ե������Ĳ����Ƕ����ڵ�8��ƬԪҲ���в���������Щֵ������֮�����9��������ǰƬԪ����Ӱֵ
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0); //�����������귶Χ��0.0��1.0�����Լ����ƶ�һ�����ض�Ӧ�ľ���
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;
        }
    }
    shadow /= 9.0;
    
    //��һ����ȹ��Զƽ��far_plane��ҪԶʱ������ͶӰ�����z�������1.0��ֻҪͶӰ������z�������1.0�����ǾͰ�shadow��ֵǿ����Ϊ0.0
    if (projCoords.z > 1.0) {
        shadow = 0.0;
    }

    //�����ڹ�Դ�ռ�ͶӰ��Χ�ڵ�����ȫ�ŵ���Ӱ��,��������
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