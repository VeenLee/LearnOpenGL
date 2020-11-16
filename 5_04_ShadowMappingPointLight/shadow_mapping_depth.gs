#version 330 core
layout (triangles) in; //输入一个三角形
layout (triangle_strip, max_vertices=18) out; //输出总共6个三角形（6*3顶点）

uniform mat4 shadowMatrices[6];

out vec4 FragPos; // FragPos from GS (output per emitvertex)

void main()
{
    for(int face = 0; face < 6; ++face)
    {
        //gl_Layer为几何着色器内建变量，它指定Emit发出的基本图形会被送到立方体贴图的哪个面，
        //当不管它时，几何着色器就会像往常一样把它的基本图形发送到图形管道的下一阶段，
        //但当我们更新这个变量就能控制每个基本图形将渲染到立方体贴图的哪一个面，当然这只有当我们有了一个附加到激活的帧缓冲的立方体贴图纹理才有效
        gl_Layer = face;
        for(int i = 0; i < 3; ++i) // for each triangle's vertices
        {
            FragPos = gl_in[i].gl_Position;
            //把面的光空间变换矩阵乘以FragPos，将每个世界空间顶点变换到相关的光空间，生成每个三角形
            gl_Position = shadowMatrices[face] * FragPos;
            EmitVertex();
        }    
        EndPrimitive();
    }
} 