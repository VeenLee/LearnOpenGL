#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 Position;

in vec2 TexCoords;

//uniform sampler2D texture_normal1;
uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_reflect1;

uniform vec3 cameraPos;
uniform samplerCube skybox;

void main()
{
	float ratio = 1.00 / 1.52;
    vec3 I = normalize(Position - cameraPos);
    vec3 R = reflect(I, normalize(Normal)); //∑¥…‰
	//vec3 R = refract(I, normalize(Normal), ratio); //’€…‰
    
	//FragColor = vec4(texture(skybox, R).rgb, 1.0);
	
	//FragColor = texture(texture_reflect1, TexCoords);
	FragColor = vec4(texture(skybox, R).rgb * vec3(texture(texture_reflect1, TexCoords)), 1.0);
}