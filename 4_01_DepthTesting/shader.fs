#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture1;

float near = 0.1;
float far = 100.0;

float LinearizeDepth(float depth) {
    float z = depth * 2.0 - 1.0;    //ת����NDC
    return (2.0 * near * far) / (far + near - z * (far - near));
}

void main()
{
    FragColor = texture(texture1, TexCoords);

    //��Ȼ����б�������ֵ�ķ�Χ��0.0(��)��1.0(Զ)��
    //��Ȳ���������Ļ����ռ��н��еģ�����ͨ��GLSL���ñ���gl_FragCoord������zֵ��gl_FragCoord��x��y����������Ƭ�ε���Ļ�ռ����꣨����(0, 0)λ�����½ǣ���
    //FragColor = vec4(vec3(gl_FragCoord.z), 1.0);

    //�����ַ����Ե����ֵת�������Ե����ֵ
    //float depth = LinearizeDepth(gl_FragCoord.z) / far;
    //FragColor = vec4(vec3(depth), 1.0);
}