#version 330 core
out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;

//GLSL��һ�����������ʹ�õ��ڽ��������ͣ�����������(Sampler)����������������Ϊ��׺������sampler1D��sampler3D
uniform sampler2D ourTexture;

void main()
{
    //ʹ��GLSL�ڽ���texture�����������������ɫ������һ��������������������ڶ��������Ƕ�Ӧ����������
    FragColor = texture(ourTexture, TexCoord);
}
