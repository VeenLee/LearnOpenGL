#version 330 core
out vec4 FragColor;

in vec3 TexCoords; // 方向向量
uniform samplerCube skybox; // 立方体贴图的纹理采样器

void main()
{
    //cubemap有自己特有的属性，可以使用方向向量对它们索引和采样
    FragColor = texture(skybox, TexCoords);
}