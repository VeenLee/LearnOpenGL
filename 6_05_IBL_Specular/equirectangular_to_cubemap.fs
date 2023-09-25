#version 330 core
out vec4 FragColor;
in vec3 WorldPos;

uniform sampler2D equirectangularMap;

const vec2 invAtan = vec2(0.1591, 0.3183);
vec2 SampleSphericalMap(vec3 v)
{
    vec2 uv = vec2(atan(v.z, v.x), asin(v.y));
    uv *= invAtan;
    uv += 0.5;
    return uv;
}

vec3 ACESToneMapping(vec3 color, float adapted_lum)
{
    const float A = 2.51;
    const float B = 0.03;
    const float C = 2.43;
    const float D = 0.59;
    const float E = 0.14;
    
    color *= adapted_lum;
    return (color * (A * color + B)) / (color * (C * color + D) + E);
}

void main()
{
    vec2 uv = SampleSphericalMap(normalize(WorldPos));
    vec3 color = texture(equirectangularMap, uv).rgb;
    
    // HDR tonemapping
    color = ACESToneMapping(color, 1.0);
    // gamma correct
    color = pow(color, vec3(1.0/2.2));

    FragColor = vec4(color, 1.0);
}