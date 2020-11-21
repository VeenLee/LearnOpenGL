#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D hdrBuffer;
uniform bool hdr;
uniform float exposure;

void main()
{             
    const float gamma = 2.2;
    vec3 hdrColor = texture(hdrBuffer, TexCoords).rgb;
    if(hdr)
    {
        //Reinhard色调映射算法，是最简单的色调映射算法，它映射整个HDR颜色值到LDR颜色值上，所有的值都有对应
        //这个算法是倾向明亮的区域的，暗的区域会不那么精细也不那么有区分度。
        //vec3 result = hdrColor / (hdrColor + vec3(1.0));

        //曝光色调映射算法
        //高曝光值会使黑暗部分显示更多的细节，然而低曝光值会显著减少黑暗区域的细节，但允许我们看到更多明亮区域的细节
        vec3 result = vec3(1.0) - exp(-hdrColor * exposure);

        //Gamma校正
        result = pow(result, vec3(1.0 / gamma));
        FragColor = vec4(result, 1.0);
    }
    else
    {
        //Gamma校正
        vec3 result = pow(hdrColor, vec3(1.0 / gamma));
        FragColor = vec4(result, 1.0);
    }
}