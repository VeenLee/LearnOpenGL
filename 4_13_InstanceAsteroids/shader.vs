#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 2) in vec2 aTexCoords;
//�������������������ݴ�С����һ��vec4����Ϊһ��mat4��������4��vec4��������ҪΪ�������Ԥ��4���������ԡ���Ϊ���ǽ�����λ��ֵ����Ϊ3������ÿһ�еĶ�������λ��ֵ����3��4��5��6��
layout (location = 3) in mat4 aInstanceMatrix; //�൱�ڶ�����3 4 5 6�ĸ���������

out vec2 TexCoords;

uniform mat4 projection;
uniform mat4 view;

void main()
{
    TexCoords = aTexCoords;
    gl_Position = projection * view * aInstanceMatrix * vec4(aPos, 1.0f); 
}