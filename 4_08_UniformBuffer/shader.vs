#version 330 core
layout (location = 0) in vec3 aPos;

//布局限定符    描述
//shared        设置uniform块是多个程序间共享的（默认布局方式）
//packed        设置uniform块占用最小的内存空间，但是这样会禁止程序间共享这个块
//std140        1.4版本之后的标准布局方式
//std430        4.3版本之后的标准布局方式
//row_major     使用行主序的方式来存储uniform块中的矩阵
//column_major  使用列主序的方式来存储uniform块中的矩阵（默认值）,不指定的话等价于layout(xxx, row_major)

//默认情况下，GLSL使用的布局方式是shared。这种布局方式表示一旦偏移量被硬件确定之后，它就能在多个程序之间共享。使用共享布局，只要变量的顺序不变，GLSL就可以优化并重新定位uniform变量。但是我们不知道某个变量的偏移量(可以通过glGetUniformBlockIndex,glGetUniformIndices函数来查询)，所以我们无法事先知道变量的偏移量，也就无法通过硬编码去设置变量。
//可以用一种更确切的方式定死每个变量的偏移，从而可以通过偏移量来设置变量，比如std140

//std140的布局规则：
//GLSL中的每个变量，比如说int、float和bool，都被定义为4字节量。每4个字节将会用一个N来表示。
//变量类型                           布局规则
//标量类型：bool,float等	         每个标量的基准对齐量为N。
//两个分量的向量（如vec2）	         基准对齐量是2N
//三分量向量和4分量向量(vec3,vec4)	 基准对齐量是4N
//标量或向量的数组                   每个元素的基准对齐量与vec4的相同。
//矩阵	                             储存为列向量的数组，每个向量的基准对齐量与vec4的相同。
//结构体	                         等于所有元素根据规则计算后的大小，但会填充到vec4大小的倍数。

//示例:
//layout (std140) uniform ExampleBlock
//{
//                     // 基准对齐量      // 对齐偏移量
//    float value;     // 4               // 0 
//    vec3 vector;     // 16              // 16  (必须是16的倍数，所以 4->16)
//    mat4 matrix;     // 16              // 32  (列 0)
//                     // 16              // 48  (列 1)
//                     // 16              // 64  (列 2)
//                     // 16              // 80  (列 3)
//    float values[3]; // 16              // 96  (values[0])
//                     // 16              // 112 (values[1])
//                     // 16              // 128 (values[2])
//    bool boolean;    // 4               // 144
//    int integer;     // 4               // 148
//}; 

layout (std140) uniform Matrices
{
    mat4 projection;
    mat4 view;
};
uniform mat4 model;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);
}  