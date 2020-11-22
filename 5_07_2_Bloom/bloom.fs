#version 330 core
layout (location = 0) out vec4 FragColor;   //对应GL_COLOR_ATTACHMENT0
layout (location = 1) out vec4 BrightColor; //对应GL_COLOR_ATTACHMENT1

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

struct Light {
    vec3 Position;
    vec3 Color;
};

uniform Light lights[4];
uniform sampler2D diffuseTexture;
uniform vec3 viewPos;

void main()
{
    vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    // ambient
    vec3 ambient = 0.0 * color;
    // lighting
    vec3 lighting = vec3(0.0);
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    for(int i = 0; i < 4; i++)
    {
        // diffuse
        vec3 lightDir = normalize(lights[i].Position - fs_in.FragPos);
        float diff = max(dot(lightDir, normal), 0.0);
        vec3 result = lights[i].Color * diff * color;
        // attenuation (use quadratic as we have gamma correction)
        float distance = length(fs_in.FragPos - lights[i].Position);
        result *= 1.0 / (distance * distance);
        lighting += result;
    }
    vec3 result = ambient + lighting;
    //检查颜色值是否高于某个阈值，如果高于就渲染到亮光颜色缓存中，即标记为发光区域（不仅仅是光源才超过，有些亮的部分也会超过）
    float brightness = dot(result, vec3(0.2126, 0.7152, 0.0722));
    //这也说明了为什么泛光在HDR基础上能够运行得很好。因为HDR中，我们可以将颜色值指定超过1.0这个默认的范围，我们能够得到对一个图像中的亮度的更好的控制权。没有HDR我们必须将阈限设置为小于1.0的数，虽然可行，但是亮部很容易变得很多，这就导致光晕效果过重。
    if(brightness > 1.0) {
        BrightColor = vec4(result, 1.0);
    }
    else {
        BrightColor = vec4(0.0, 0.0, 0.0, 1.0);
    }
    FragColor = vec4(result, 1.0);
}
