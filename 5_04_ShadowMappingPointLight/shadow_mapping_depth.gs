#version 330 core
layout (triangles) in; //����һ��������
layout (triangle_strip, max_vertices=18) out; //����ܹ�6�������Σ�6*3���㣩

uniform mat4 shadowMatrices[6];

out vec4 FragPos; // FragPos from GS (output per emitvertex)

void main()
{
    for(int face = 0; face < 6; ++face)
    {
        //gl_LayerΪ������ɫ���ڽ���������ָ��Emit�����Ļ���ͼ�λᱻ�͵���������ͼ���ĸ��棬
        //��������ʱ��������ɫ���ͻ�������һ�������Ļ���ͼ�η��͵�ͼ�ιܵ�����һ�׶Σ�
        //�������Ǹ�������������ܿ���ÿ������ͼ�ν���Ⱦ����������ͼ����һ���棬��Ȼ��ֻ�е���������һ�����ӵ������֡�������������ͼ�������Ч
        gl_Layer = face;
        for(int i = 0; i < 3; ++i) // for each triangle's vertices
        {
            FragPos = gl_in[i].gl_Position;
            //����Ĺ�ռ�任�������FragPos����ÿ������ռ䶥��任����صĹ�ռ䣬����ÿ��������
            gl_Position = shadowMatrices[face] * FragPos;
            EmitVertex();
        }    
        EndPrimitive();
    }
} 