#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));

    //法线矩阵,(模型矩阵左上角的逆矩阵的转置矩阵),我们可以使用inverse和transpose函数自己生成这个法线矩阵，这两个函数对所有类型矩阵都有效。
    //注意我们还要把被处理过的矩阵强制转换为3x3矩阵，来保证它失去了位移属性以及能够乘以vec3的法向量。
    Normal = mat3(transpose(inverse(model))) * aNormal;

    TexCoords = aTexCoords;
    
    gl_Position = projection * view * vec4(FragPos, 1.0);
}