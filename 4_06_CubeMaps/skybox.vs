#version 330 core
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;

void main()
{
    TexCoords = aPos;
    vec4 pos = projection * view * vec4(aPos, 1.0);
    //我们需要欺骗深度缓冲，让它认为天空盒有着最大的深度值1.0，
    //将输出位置的z分量等于它的w分量，让z分量永远等于1.0，这样的话，当透视除法执行之后，z分量会变为w / w = 1.0
    gl_Position = pos.xyww;
}