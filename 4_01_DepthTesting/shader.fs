#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture1;

float near = 0.1;
float far = 100.0;

float LinearizeDepth(float depth) {
    float z = depth * 2.0 - 1.0;    //转换成NDC
    return (2.0 * near * far) / (far + near - z * (far - near));
}

void main()
{
    FragColor = texture(texture1, TexCoords);

    //深度缓存中保存的深度值的范围是0.0(近)到1.0(远)，
    //深度测试是在屏幕坐标空间中进行的，可以通过GLSL内置变量gl_FragCoord来访问z值，gl_FragCoord的x和y分量代表了片段的屏幕空间坐标（其中(0, 0)位于左下角）。
    //FragColor = vec4(vec3(gl_FragCoord.z), 1.0);

    //把这种非线性的深度值转换成线性的深度值
    //float depth = LinearizeDepth(gl_FragCoord.z) / far;
    //FragColor = vec4(vec3(depth), 1.0);
}