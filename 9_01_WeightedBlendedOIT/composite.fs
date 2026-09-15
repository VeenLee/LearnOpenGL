#version 330 core

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D opaqueTexture;
uniform sampler2D accumulationTexture;
uniform sampler2D revealageTexture;

void main()
{
    vec3 opaque = texture(opaqueTexture, TexCoords).rgb;
    vec4 accumulation = texture(accumulationTexture, TexCoords);
    float revealage = texture(revealageTexture, TexCoords).r;

    vec3 transparent = accumulation.rgb / max(accumulation.a, 1e-5);
    vec3 color = transparent * (1.0 - revealage) + opaque * revealage;
    FragColor = vec4(color, 1.0);
}
