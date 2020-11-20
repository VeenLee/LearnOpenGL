#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

//TBN矩阵用法1：直接使用TBN矩阵，把切线空间的向量转换到世界空间
//out VS_OUT {
//    vec3 FragPos;
//    vec2 TexCoords;
//    mat3 TBN;
//} vs_out;

//TBN矩阵用法2：使用TBN矩阵的逆矩阵，把世界空间的向量转换到切线空间
out VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 TangentLightPos;
    vec3 TangentViewPos;
    vec3 TangentFragPos;
} vs_out;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{
    vs_out.FragPos = vec3(model * vec4(aPos, 1.0));   
    vs_out.TexCoords = aTexCoords;
    
    //将TBN向量变换到世界空间
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 T = normalize(normalMatrix * aTangent);
    vec3 N = normalize(normalMatrix * aNormal);

    //在大量的计算过程中，TBN向量可能会变得不两两垂直，这就会导致我们的模型有瑕疵。
    //因此，一种名叫Gram-Schmidt正交化的方法就被创造出来。通过一点很小的代价，让TBN向量继续两两垂直：
    T = normalize(T - dot(T, N) * N);

    //从技术上讲，顶点着色器中无需副切线B，TBN三向量都是相互垂直的所以可以用T和N向量的叉乘计算出副切线B
    vec3 B = cross(N, T);

    //TBN矩阵用法1：直接使用TBN矩阵，把切线空间的向量转换到世界空间
    //vs_out.TBN = mat3(T, B, N);

    //TBN矩阵用法2：使用TBN矩阵的逆矩阵，把世界空间的向量转换到切线空间
    mat3 TBN = transpose(mat3(T, B, N)); //正交矩阵（每个轴既是单位向量同时相互垂直）的一大属性是一个正交矩阵的转置矩阵与它的逆矩阵相等，这个属性很重要因为逆矩阵的求得比求转置开销大，但是结果却是一样的
    vs_out.TangentLightPos = TBN * lightPos;
    vs_out.TangentViewPos  = TBN * viewPos;
    vs_out.TangentFragPos  = TBN * vs_out.FragPos;

    gl_Position = projection * view * model * vec4(aPos, 1.0);
}