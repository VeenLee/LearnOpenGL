#version 330 core
out vec4 FragColor;

in vec3 TexCoords; // ��������
uniform samplerCube skybox; // ��������ͼ�����������

void main()
{
    //cubemap���Լ����е����ԣ�����ʹ�÷������������������Ͳ���
    FragColor = texture(skybox, TexCoords);
}