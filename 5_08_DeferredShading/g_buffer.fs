#version 330 core
//因为使用了多渲染目标，这个布局指示符(Layout Specifier)告诉了OpenGL我们需要渲染到当前的活跃帧缓冲中的哪一个颜色缓冲
layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

void main()
{    
    //存储片段位置向量到G缓冲纹理的第一个位置
    gPosition = FragPos;
    //存储片段法线到G缓冲中
    gNormal = normalize(Normal);
    //存储漫反射颜色到G缓冲中
    gAlbedoSpec.rgb = texture(texture_diffuse1, TexCoords).rgb;
    //存储镜面强度到gAlbedoSpec的alpha分量
    gAlbedoSpec.a = texture(texture_specular1, TexCoords).r;
}