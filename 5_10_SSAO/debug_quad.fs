#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D textureMap;

void main()
{
    FragColor = vec4(vec3(texture(textureMap, TexCoords).r), 1.0);
}