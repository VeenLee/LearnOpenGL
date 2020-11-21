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
        //Reinhardɫ��ӳ���㷨������򵥵�ɫ��ӳ���㷨����ӳ������HDR��ɫֵ��LDR��ɫֵ�ϣ����е�ֵ���ж�Ӧ
        //����㷨����������������ģ���������᲻��ô��ϸҲ����ô�����ֶȡ�
        //vec3 result = hdrColor / (hdrColor + vec3(1.0));

        //�ع�ɫ��ӳ���㷨
        //���ع�ֵ��ʹ�ڰ�������ʾ�����ϸ�ڣ�Ȼ�����ع�ֵ���������ٺڰ������ϸ�ڣ����������ǿ����������������ϸ��
        vec3 result = vec3(1.0) - exp(-hdrColor * exposure);

        //GammaУ��
        result = pow(result, vec3(1.0 / gamma));
        FragColor = vec4(result, 1.0);
    }
    else
    {
        //GammaУ��
        vec3 result = pow(hdrColor, vec3(1.0 / gamma));
        FragColor = vec4(result, 1.0);
    }
}