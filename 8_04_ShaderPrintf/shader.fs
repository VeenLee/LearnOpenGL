#version 450
out vec4 FragColor;

uniform ivec2 mouse;

in vec3 ourColor;

layout (std430) buffer printfBuffer
{
    uint printfLocation;
    uint printfData[];
};

void main()
{
    uint printfIndex = min(atomicAdd(printfLocation, 8u), printfData.length() - 8u); //缓冲区元素个数
    printfData[printfIndex++] = 37;           //%
    printfData[printfIndex++] = 117;          //u
    printfData[printfIndex++] = (mouse).x;
    printfData[printfIndex++] = 32;           //Space
    printfData[printfIndex++] = 37;           //%
    printfData[printfIndex++] = 117;          //u
    printfData[printfIndex++] = (mouse).y;
    printfData[printfIndex++] = 10;           //\n

    FragColor = vec4(ourColor, 1.0);
}
