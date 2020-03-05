#version 330 core
out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;

//GLSL有一个供纹理对象使用的内建数据类型，叫做采样器(Sampler)，它以纹理类型作为后缀，比如sampler1D、sampler3D
// texture samplers
uniform sampler2D texture1;
uniform sampler2D texture2;

uniform float mixValue;

void main()
{
	//GLSL内建的mix函数需要接受两个值作为参数，并对它们根据第三个参数进行线性插值
	// linearly interpolate between both textures (80% container, 20% awesomeface)
	FragColor = mix(texture(texture1, TexCoord), texture(texture2, vec2(1.0 - TexCoord.x, TexCoord.y)), mixValue);
}
