#version 330 core
out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;

//GLSL有一个供纹理对象使用的内建数据类型，叫做采样器(Sampler)，它以纹理类型作为后缀，比如sampler1D、sampler3D
uniform sampler2D ourTexture;

void main()
{
    //使用GLSL内建的texture函数来采样纹理的颜色，它第一个参数是纹理采样器，第二个参数是对应的纹理坐标
    FragColor = texture(ourTexture, TexCoord);
}
