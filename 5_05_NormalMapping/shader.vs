#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

//TBN�����÷�1��ֱ��ʹ��TBN���󣬰����߿ռ������ת��������ռ�
//out VS_OUT {
//    vec3 FragPos;
//    vec2 TexCoords;
//    mat3 TBN;
//} vs_out;

//TBN�����÷�2��ʹ��TBN���������󣬰�����ռ������ת�������߿ռ�
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
    
    //��TBN�����任������ռ�
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 T = normalize(normalMatrix * aTangent);
    vec3 N = normalize(normalMatrix * aNormal);

    //�ڴ����ļ�������У�TBN�������ܻ��ò�������ֱ����ͻᵼ�����ǵ�ģ����覴á�
    //��ˣ�һ������Gram-Schmidt�������ķ����ͱ����������ͨ��һ���С�Ĵ��ۣ���TBN��������������ֱ��
    T = normalize(T - dot(T, N) * N);

    //�Ӽ����Ͻ���������ɫ�������踱����B��TBN�����������໥��ֱ�����Կ�����T��N�����Ĳ�˼����������B
    vec3 B = cross(N, T);

    //TBN�����÷�1��ֱ��ʹ��TBN���󣬰����߿ռ������ת��������ռ�
    //vs_out.TBN = mat3(T, B, N);

    //TBN�����÷�2��ʹ��TBN���������󣬰�����ռ������ת�������߿ռ�
    mat3 TBN = transpose(mat3(T, B, N)); //��������ÿ������ǵ�λ����ͬʱ�໥��ֱ����һ��������һ�����������ת�þ����������������ȣ�������Ժ���Ҫ��Ϊ��������ñ���ת�ÿ����󣬵��ǽ��ȴ��һ����
    vs_out.TangentLightPos = TBN * lightPos;
    vs_out.TangentViewPos  = TBN * viewPos;
    vs_out.TangentFragPos  = TBN * vs_out.FragPos;

    gl_Position = projection * view * model * vec4(aPos, 1.0);
}