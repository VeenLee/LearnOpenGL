#version 330 core

layout (location = 0) out vec4 Accumulation;
layout (location = 1) out float Revealage;

in vec2 TexCoords;

uniform sampler2D texture1;

void main()
{
    vec4 color = texture(texture1, TexCoords);
    if (color.a < 0.01)
        discard;

    // Bias fragments with higher opacity and fragments closer to the camera.
    // Clamping keeps the half-float accumulation target numerically stable.
    float weight = clamp(
        pow(min(1.0, color.a * 10.0) + 0.01, 3.0) * 1e8
        * pow(1.0 - gl_FragCoord.z * 0.9, 3.0),
        1e-2,
        3e3
    );

    Accumulation = vec4(color.rgb * color.a, color.a) * weight;
    Revealage = color.a;
}
