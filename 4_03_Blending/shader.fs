#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture1;

void main()
{
	vec4 texColor = texture(texture1, TexCoords);
    //启用了混合就不需要丢弃片段了
    //if(texColor.a < 0.1) {
    //    //使用discard关键字丢弃一个片源
    //    discard;
    //}
    FragColor = texColor;
          
    //FragColor = texture(texture1, TexCoords);
}