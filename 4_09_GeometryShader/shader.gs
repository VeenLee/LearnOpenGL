//һ������(Pass-through)������ɫ�����������һ����ͼԪ����ֱ�ӽ�������(Pass)����һ����ɫ��
//#version 330 core
//layout (points) in;
//layout (points, max_vertices = 1) out;
//
//void main() {    
//    gl_Position = gl_in[0].gl_Position; 
//    EmitVertex(); //��EndPrimitive������ʱ�����з������(Emitted)���㶼��ϳ�Ϊָ���������ȾͼԪ
//    EndPrimitive(); //����EmitVertexʱ��gl_Position�е������ᱻ��ӵ�ͼԪ����
//}

//����һ����ͼԪ��Ϊ���룬�������Ϊ���ģ�����һ��ˮƽ����ͼԪ��
//#version 330 core
//layout (points) in;
//layout (line_strip, max_vertices = 2) out;
//
//void main() {    
//    gl_Position = gl_in[0].gl_Position + vec4(-0.1, 0, 0.0, 0.0); 
//    EmitVertex();
//
//    gl_Position = gl_in[0].gl_Position + vec4( 0.1, 0.0, 0.0, 0.0);
//    EmitVertex();
//
//    EndPrimitive();
//}


//����С����
#version 330 core
layout (points) in;
layout (triangle_strip, max_vertices = 5) out;

//��Ϊ������ɫ���������������һ�鶥��ģ��Ӷ�����ɫ�����������������ǻ����������ʽ��ʾ������������������ֻ��һ������
in VS_OUT {
    vec3 color;
} gs_in[];

out vec3 fColor;

void build_house(vec4 position)
{    
    fColor = gs_in[0].color; // gs_in[0] ��Ϊֻ��һ�����붥��
    gl_Position = position + vec4(-0.2, -0.2, 0.0, 0.0);    // 1:����
    EmitVertex();   
    gl_Position = position + vec4( 0.2, -0.2, 0.0, 0.0);    // 2:����
    EmitVertex();
    gl_Position = position + vec4(-0.2,  0.2, 0.0, 0.0);    // 3:����
    EmitVertex();
    gl_Position = position + vec4( 0.2,  0.2, 0.0, 0.0);    // 4:����
    EmitVertex();
    gl_Position = position + vec4( 0.0,  0.4, 0.0, 0.0);    // 5:����
	fColor = vec3(1.0, 1.0, 1.0); //�����һ���������ɫ����Ϊ��ɫ�����ݶ�����һЩѩ
	EmitVertex();
    EndPrimitive();
}

void main() {    
    build_house(gl_in[0].gl_Position);
}
