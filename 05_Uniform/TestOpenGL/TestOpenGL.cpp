//
// �������ε���ɫ����ʱ��仯
//

#include "pch.h"
#include <iostream>

#include "glad/glad.h"
#include "GLFW/glfw3.h"

//�ص�����������ÿ�δ��ڴ�С��������ʱ�򱻵���
void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
	glViewport(0, 0, width, height);
}

void processInput(GLFWwindow *window)
{
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
		glfwSetWindowShouldClose(window, true);
}

int main()
{
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	//glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);

	GLFWwindow* window = glfwCreateWindow(800, 600, "LearnOpenGL", NULL, NULL);
	if (window == NULL)
	{
		std::cout << "Failed to create GLFW window" << std::endl;
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);

	if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
	{
		std::cout << "Failed to initialize GLAD" << std::endl;
		return -1;
	}
	glViewport(0, 0, 800, 600);

	glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);

	float vertices[] = {
		-0.5f, -0.5f, 0.0f,
		0.5f, -0.5f, 0.0f,
		0.0f,  0.5f, 0.0f,
		//// second triangle
		//0.0f, -0.5f, 0.0f,  // left
		//0.9f, -0.5f, 0.0f,  // right
		//0.45f, 0.5f, 0.0f   // top 
	};

	/*
		�����������(Vertex Array Object, VAO)�����񶥵㻺������������󶨣��κ����Ķ������Ե��ö��ᴢ�������VAO�С������ĺô����ǣ������ö�������ָ��ʱ����ֻ��Ҫ����Щ����ִ��һ�Σ�
	֮���ٻ��������ʱ��ֻ��Ҫ����Ӧ��VAO�����ˡ���ʹ�ڲ�ͬ�������ݺ���������֮���л���÷ǳ��򵥣�ֻ��Ҫ�󶨲�ͬ��VAO�����ˡ��ո����õ�����״̬�����洢��VAO�С�
	OpenGL�ĺ���ģʽҪ������ʹ��VAO��������֪������δ������ǵĶ������롣������ǰ�VAOʧ�ܣ�OpenGL��ܾ������κζ�����
	*/
	//����һ��VAO
	unsigned int VAO;
	glGenVertexArrays(1, &VAO);

	//����һ��VBO����
	unsigned int VBO;
	glGenBuffers(1, &VBO);

	// bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
	glBindVertexArray(VAO);

	//���´����Ļ���󶨵�GL_ARRAY_BUFFERĿ����
	glBindBuffer(GL_ARRAY_BUFFER, VBO);

	//���ƶ������鵽�����й�OpenGLʹ��
	/*
	glBufferData��һ��ר���������û���������ݸ��Ƶ���ǰ�󶨻���ĺ�����
	���ĵ�һ��������Ŀ�껺������ͣ����㻺�����ǰ�󶨵�GL_ARRAY_BUFFERĿ���ϡ�
	�ڶ�������ָ���������ݵĴ�С(���ֽ�Ϊ��λ)����һ���򵥵�sizeof������������ݴ�С���С�
	����������������ϣ�����͵�ʵ�����ݡ�
	���ĸ�����ָ��������ϣ���Կ���ι�����������ݡ�����������ʽ��
		GL_STATIC_DRAW �����ݲ���򼸺�����ı䡣
		GL_DYNAMIC_DRAW�����ݻᱻ�ı�ܶࡣ
		GL_STREAM_DRAW ������ÿ�λ���ʱ����ı䡣
	�����ε�λ�����ݲ���ı䣬ÿ����Ⱦ����ʱ������ԭ������������ʹ�����������GL_STATIC_DRAW��
	���������˵һ�������е����ݽ�Ƶ�����ı䣬��ôʹ�õ����;���GL_DYNAMIC_DRAW��GL_STREAM_DRAW����������ȷ���Կ������ݷ����ܹ�����д����ڴ沿�֡�
	*/
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

	//GLSL������ɫ����Դ����
	const char* vertexShaderSource = "#version 330 core\n \
	layout(location = 0) in vec3 aPos; // λ�ñ���������λ��ֵΪ0\n \
	\n \
	void main()\n \
	{\n \
		gl_Position = vec4(aPos, 1.0); // ע��������ΰ�һ��vec3��Ϊvec4�Ĺ������Ĳ���\n \
	}\n";

	//����һ��������ɫ������
	unsigned int vertexShader;
	vertexShader = glCreateShader(GL_VERTEX_SHADER);

	//����ɫ��Դ�븽�ӵ���ɫ�������ϣ�Ȼ�������
	glShaderSource(vertexShader, 1, &vertexShaderSource, NULL);
	glCompileShader(vertexShader);

	//������ʱ����
	int  success;
	char infoLog[512];
	glGetShaderiv(vertexShader, GL_COMPILE_STATUS, &success);
	if (!success)
	{
		glGetShaderInfoLog(vertexShader, 512, NULL, infoLog);
		std::cout << "ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" << infoLog << std::endl;
	}

	//GLSLƬ����ɫ����Դ����
	const char* fragmentShaderSource = "#version 330 core\n \
	out vec4 FragColor;\n \
	\n \
	uniform vec4 ourColor; // ��OpenGL����������趨�������\n \
	\n \
	void main()\n \
	{\n \
		FragColor = ourColor;\n \
	}\n";

	//����Ƭ����ɫ��
	unsigned int fragmentShader;
	fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fragmentShader, 1, &fragmentShaderSource, NULL);
	glCompileShader(fragmentShader);

	//������ʱ����
	glGetShaderiv(fragmentShader, GL_COMPILE_STATUS, &success);
	if (!success)
	{
		glGetShaderInfoLog(fragmentShader, 512, NULL, infoLog);
		std::cout << "ERROR::SHADER::FRAGMENT::COMPILATION_FAILED\n" << infoLog << std::endl;
	}

	//��ɫ���������(Shader Program Object)�Ƕ����ɫ���ϲ�֮������������ɵİ汾
	//����һ���������
	unsigned int shaderProgram;
	shaderProgram = glCreateProgram();

	//��glLinkProgram��������
	glAttachShader(shaderProgram, vertexShader);
	glAttachShader(shaderProgram, fragmentShader);
	glLinkProgram(shaderProgram);

	//���������ɫ�������Ƿ�ʧ��
	glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
	if (!success) {
		glGetProgramInfoLog(shaderProgram, 512, NULL, infoLog);
		std::cout << "ERROR::SHADER::LINK::COMPILATION_FAILED\n" << infoLog << std::endl;
	}

	//��������������
	//��glUseProgram��������֮��ÿ����ɫ�����ú���Ⱦ���ö���ʹ������������Ҳ����֮ǰд����ɫ��)��
	glUseProgram(shaderProgram);

	//�ڰ���ɫ���������ӵ���������Ժ󣬼ǵ�ɾ����ɫ���������ǲ�����Ҫ������
	glDeleteShader(vertexShader);
	glDeleteShader(fragmentShader);

	//OpenGL����֪��������ν����ڴ��еĶ������ݣ��Լ�������ν������������ӵ�������ɫ���������ϡ�
	//���Ǳ�������Ⱦǰָ��OpenGL����ν��Ͷ������ݡ�
	/*
	���ǵĶ��㻺�����ݻᱻ����Ϊ���������ӣ�
		1.λ�����ݱ�����Ϊ32λ��4�ֽڣ�����ֵ��
		2.ÿ��λ�ð���3��������ֵ��
		3.����3��ֵ֮��û�п�϶��������ֵ�����⼸��ֵ�������н�������(Tightly Packed)��
		4.�����е�һ��ֵ�ڻ��忪ʼ��λ�á�
	*/

	/*
	glVertexAttribPointer�����Ĳ����ǳ��࣬�����һ���һ�������ǣ�
	��һ������ָ������Ҫ���õĶ������ԡ����ǵ������ڶ�����ɫ����ʹ��layout(location = 0)������position�������Ե�λ��ֵ(Location)�������԰Ѷ������Ե�λ��ֵ����Ϊ0��
		��Ϊ����ϣ�������ݴ��ݵ���һ�����������У������������Ǵ���0��
	�ڶ�������ָ���������ԵĴ�С������������һ��vec3������3��ֵ��ɣ����Դ�С��3��
	����������ָ�����ݵ����ͣ�������GL_FLOAT(GLSL��vec*�����ɸ�����ֵ��ɵ�)��
	���ĸ��������������Ƿ�ϣ�����ݱ���׼��(Normalize)�������������ΪGL_TRUE���������ݶ��ᱻӳ�䵽0�������з�����signed������-1����1֮�䡣���ǰ�������ΪGL_FALSE��
	�����������������(Stride)�������������������Ķ���������֮��ļ���������¸���λ��������3��float֮�����ǰѲ�������Ϊ3 * sizeof(float)��
		Ҫע�������������֪����������ǽ������еģ���������������֮��û�п�϶������Ҳ��������Ϊ0����OpenGL�������岽���Ƕ��٣�ֻ�е���ֵ�ǽ�������ʱ�ſ��ã���
		һ�������и���Ķ������ԣ����Ǿͱ����С�ĵض���ÿ����������֮��ļ���������ں���ῴ����������ӣ���ע:�����������˼��˵���Ǵ�������Եڶ��γ��ֵĵط�����������0λ��֮���ж����ֽڣ���
	������������������void*��������Ҫ���ǽ��������ֵ�ǿ������ת��������ʾλ�������ڻ�������ʼλ�õ�ƫ����(Offset)������λ������������Ŀ�ͷ������������0�����ǻ��ں�����ϸ�������������
	*/
	//���ö�������ָ��
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);

	//���ö������ԣ���������Ĭ���ǽ��õ�
	glEnableVertexAttribArray(0);

	while (!glfwWindowShouldClose(window))
	{
		// ����
		processInput(window);

		glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT);

		// draw our first triangle
		glUseProgram(shaderProgram);
		glBindVertexArray(VAO); // seeing as we only have a single VAO there's no need to bind it every time, but we'll do so to keep things a bit more organized

		// ����uniform��ɫ
		float timeValue = glfwGetTime();
		float greenValue = sin(timeValue) / 2.0f + 0.5f;
		int vertexColorLocation = glGetUniformLocation(shaderProgram, "ourColor");
		glUniform4f(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);

		//Ҫ�����������Ҫ�����壬OpenGL�������ṩ��glDrawArrays��������ʹ�õ�ǰ�������ɫ����֮ǰ����Ķ����������ã���VBO�Ķ������ݣ�ͨ��VAO��Ӱ󶨣�������ͼԪ��
		//��һ�����������Ǵ�����Ƶ�OpenGLͼԪ�����͡��ڶ�������ָ���˶����������ʼ����������������0�����һ������ָ�����Ǵ�����ƶ��ٸ����㣬������3������ֻ�����ǵ���������Ⱦһ�������Σ���ֻ��3�����㳤����
		glDrawArrays(GL_TRIANGLES, 0, 3); //����������������Σ��������������Ը�Ϊ6
		// glBindVertexArray(0); // no need to unbind it every time 

		// ��鲢�����¼�����������
		glfwSwapBuffers(window);
		glfwPollEvents();
	}

	glfwTerminate();

	return 0;
}
