//一个传递(Pass-through)几何着色器，它会接收一个点图元，并直接将它传递(Pass)到下一个着色器
//#version 330 core
//layout (points) in;
//layout (points, max_vertices = 1) out;
//
//void main() {    
//    gl_Position = gl_in[0].gl_Position; 
//    EmitVertex(); //当EndPrimitive被调用时，所有发射出的(Emitted)顶点都会合成为指定的输出渲染图元
//    EndPrimitive(); //调用EmitVertex时，gl_Position中的向量会被添加到图元中来
//}

//接受一个点图元作为输入，以这个点为中心，创建一条水平的线图元。
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


//绘制小房子
#version 330 core
layout (points) in;
layout (triangle_strip, max_vertices = 5) out;

//因为几何着色器是作用于输入的一组顶点的，从顶点着色器发来输入数据总是会以数组的形式表示出来，即便我们现在只有一个顶点
in VS_OUT {
    vec3 color;
} gs_in[];

out vec3 fColor;

void build_house(vec4 position)
{    
    fColor = gs_in[0].color; // gs_in[0] 因为只有一个输入顶点
    gl_Position = position + vec4(-0.2, -0.2, 0.0, 0.0);    // 1:左下
    EmitVertex();   
    gl_Position = position + vec4( 0.2, -0.2, 0.0, 0.0);    // 2:右下
    EmitVertex();
    gl_Position = position + vec4(-0.2,  0.2, 0.0, 0.0);    // 3:左上
    EmitVertex();
    gl_Position = position + vec4( 0.2,  0.2, 0.0, 0.0);    // 4:右上
    EmitVertex();
    gl_Position = position + vec4( 0.0,  0.4, 0.0, 0.0);    // 5:顶部
	fColor = vec3(1.0, 1.0, 1.0); //将最后一个顶点的颜色设置为白色，给屋顶落上一些雪
	EmitVertex();
    EndPrimitive();
}

void main() {    
    build_house(gl_in[0].gl_Position);
}
