#version 330 core
in vec4 FragPos;

uniform vec3 lightPos;
uniform float far_plane;

void main()
{
    float lightDistance = length(FragPos.xyz - lightPos);
    
    //��fragment�͹�Դ֮��ľ��룬ӳ�䵽0��1�ķ�Χ������д��Ϊfragment�����ֵ��(���Ծ���)
    lightDistance = lightDistance / far_plane;
    
    gl_FragDepth = lightDistance;
}