#version 330 core
out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;

void main()
{
	//�����ǰѹ�Դ����ɫ���������ɫֵ��ˣ����õ��ľ�������������������ɫ��Ҳ������������֪������ɫ��
	FragColor = vec4(lightColor * objectColor, 1.0);
}
