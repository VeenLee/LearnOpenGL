#version 330 core
in vec4 FragPos;

uniform vec3 lightPos;
uniform float far_plane;

void main()
{
    float lightDistance = length(FragPos.xyz - lightPos);
    
    //把fragment和光源之间的距离，映射到0到1的范围，把它写入为fragment的深度值。(线性距离)
    lightDistance = lightDistance / far_plane;
    
    gl_FragDepth = lightDistance;
}