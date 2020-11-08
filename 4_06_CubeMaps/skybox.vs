#version 330 core
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;

void main()
{
    TexCoords = aPos;
    vec4 pos = projection * view * vec4(aPos, 1.0);
    //������Ҫ��ƭ��Ȼ��壬������Ϊ��պ������������ֵ1.0��
    //�����λ�õ�z������������w��������z������Զ����1.0�������Ļ�����͸�ӳ���ִ��֮��z�������Ϊw / w = 1.0
    gl_Position = pos.xyww;
}