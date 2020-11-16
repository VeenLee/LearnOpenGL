#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    //矩阵乘法满足结合率，一般情况下不满足交换率，
    //当矩阵相乘时，在最右边的矩阵是第一个与向量相乘的，所以应该从右向左读这个乘法。
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    TexCoord = vec2(aTexCoord.x, aTexCoord.y);
}