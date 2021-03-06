#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D image;

uniform bool horizontal;
uniform float weight[5] = float[] (0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162);

void main()
{
     vec2 tex_offset = 1.0 / textureSize(image, 0); //由于纹理坐标范围是0.0到1.0，可以计算移动一个像素对应的距离
     vec3 result = texture(image, TexCoords).rgb * weight[0]; //当前像素本身的贡献值
     if(horizontal) {
         for(int i = 1; i < 5; ++i) {
            //水平方向各像素贡献值
            result += texture(image, TexCoords + vec2(tex_offset.x * i, 0.0)).rgb * weight[i];
            result += texture(image, TexCoords - vec2(tex_offset.x * i, 0.0)).rgb * weight[i];
         }
     }
     else {
         for(int i = 1; i < 5; ++i) {
             //竖直方向各像素贡献值
             result += texture(image, TexCoords + vec2(0.0, tex_offset.y * i)).rgb * weight[i];
             result += texture(image, TexCoords - vec2(0.0, tex_offset.y * i)).rgb * weight[i];
         }
     }
     FragColor = vec4(result, 1.0);
}