#version 450
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;

layout (std430) buffer printfBuffer
{
	uint printfLocation;
	uint printfData[];
};

out vec3 ourColor;

void main()
{
    uint printfIndex = min(atomicAdd(printfLocation, 4u), printfData.length() - 4u); //缓冲区元素个数
	printfData[printfIndex++] = 37;           //%
	printfData[printfIndex++] = 100;          //d
	printfData[printfIndex++] = gl_VertexID;
	printfData[printfIndex++] = 10;           //\n

    gl_Position = vec4(aPos, 1.0);
    ourColor = aColor;
}
