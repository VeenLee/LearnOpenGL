#version 330 core
//��Ϊʹ���˶���ȾĿ�꣬�������ָʾ��(Layout Specifier)������OpenGL������Ҫ��Ⱦ����ǰ�Ļ�Ծ֡�����е���һ����ɫ����
layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

void main()
{    
    //�洢Ƭ��λ��������G��������ĵ�һ��λ��
    gPosition = FragPos;
    //�洢Ƭ�η��ߵ�G������
    gNormal = normalize(Normal);
    //�洢��������ɫ��G������
    gAlbedoSpec.rgb = texture(texture_diffuse1, TexCoords).rgb;
    //�洢����ǿ�ȵ�gAlbedoSpec��alpha����
    gAlbedoSpec.a = texture(texture_specular1, TexCoords).r;
}