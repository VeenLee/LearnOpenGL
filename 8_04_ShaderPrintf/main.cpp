//
// shader printf, 输出shader变量值
//

#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include "shader_s.h"
#include "camera.h"
#include "model.h"

#include <iostream>
#include <time.h>

void framebuffer_size_callback(GLFWwindow* window, int width, int height);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);
void processInput(GLFWwindow *window);
unsigned int loadTexture(const char* path, bool gammaCorrection);

GLuint createPrintBuffer(unsigned size = 16 * 1024 * 1024);
void deletePrintBuffer(GLuint printBuffer);
void bindPrintBuffer(GLuint program, GLuint printBuffer);
std::string getPrintBufferString(GLuint printBuffer);

// settings
const unsigned int SCR_WIDTH = 120;
const unsigned int SCR_HEIGHT = 80;

bool blinn = false;
bool blinnKeyPressed = false;

// camera
Camera camera(glm::vec3(0.0f, 0.0f, 3.0f));
float lastX = SCR_WIDTH / 2.0f;
float lastY = SCR_HEIGHT / 2.0f;
bool firstMouse = true;

// timing
float deltaTime = 0.0f;
float lastFrame = 0.0f;

int main()
{
	// glfw: initialize and configure
	// ------------------------------
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	//glfwWindowHint(GLFW_SAMPLES, 4);

#ifdef __APPLE__
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE); // uncomment this statement to fix compilation on OS X
#endif

	// glfw window creation
	// --------------------
	GLFWwindow* window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "LearnOpenGL", NULL, NULL);
	if (window == NULL)
	{
		std::cout << "Failed to create GLFW window" << std::endl;
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);
	glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
	glfwSetCursorPosCallback(window, mouse_callback);
	glfwSetScrollCallback(window, scroll_callback);

	// glad: load all OpenGL function pointers
	// ---------------------------------------
	if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
	{
		std::cout << "Failed to initialize GLAD" << std::endl;
		return -1;
	}

	// configure global opengl state
	// -----------------------------
	glEnable(GL_DEPTH_TEST);

	// build and compile shaders
	// -------------------------
	Shader shader("shader.vs", "shader.fs");

	float vertices[] = {
		//---- 位置 ----       ---- 颜色 ----     - 纹理坐标 -
		1.0f,  1.0f, 0.0f,    1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // 右上
		1.0f, -1.0f, 0.0f,    0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // 右下
		-1.0f, -1.0f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f,   // 左下
		-1.0f,  1.0f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // 左上
	};

	unsigned int indices[] = {
		0, 1, 3, // first triangle
		1, 2, 3  // second triangle
	};

	unsigned int VAO;
	glGenVertexArrays(1, &VAO);

	unsigned int VBO;
	glGenBuffers(1, &VBO);

	unsigned int EBO;
	glGenBuffers(1, &EBO);

	glBindVertexArray(VAO);

	glBindBuffer(GL_ARRAY_BUFFER, VBO);

	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

	//设置顶点位置属性指针
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)0);
	//启用顶点属性，顶点属性默认是禁用的
	glEnableVertexAttribArray(0);

	//设置顶点颜色属性指针
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);

	//设置顶点纹理属性指针
	glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	shader.use();

	// render loop
	// -----------
	while (!glfwWindowShouldClose(window))
	{
		// per-frame time logic
		// --------------------
		float currentTime = glfwGetTime();
		deltaTime = currentTime - lastFrame;
		lastFrame = currentTime;

		// input
		// -----
		processInput(window);

		// render
		// ------
		glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		shader.use();


		glUniform2i(glGetUniformLocation(shader.ID, "mouse"), lastX, lastY);

		// create a buffer to hold the printf results
		GLuint printBuffer = createPrintBuffer();
		// bind it to the current program
		bindPrintBuffer(shader.ID, printBuffer);


		// draw triangle
		glBindVertexArray(VAO);
		glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);


		// convert to string, output to console
		std::string strOutput = getPrintBufferString(printBuffer);
		printf("\nGLSL print:\n%s\n", strOutput.c_str());
		// clean up
		deletePrintBuffer(printBuffer);
		

		// glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
		// -------------------------------------------------------------------------------
		glfwSwapBuffers(window);
		glfwPollEvents();
	}

	glfwTerminate();
	return 0;
}

// process all input: query GLFW whether relevant keys are pressed/released this frame and react accordingly
void processInput(GLFWwindow *window)
{
	if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
		glfwSetWindowShouldClose(window, true);

	if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS)
		camera.ProcessKeyboard(FORWARD, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS)
		camera.ProcessKeyboard(BACKWARD, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS)
		camera.ProcessKeyboard(LEFT, deltaTime);
	if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS)
		camera.ProcessKeyboard(RIGHT, deltaTime);

	if (glfwGetKey(window, GLFW_KEY_B) == GLFW_PRESS && !blinnKeyPressed)
	{
		blinn = !blinn;
		blinnKeyPressed = true;
	}
	if (glfwGetKey(window, GLFW_KEY_B) == GLFW_RELEASE)
	{
		blinnKeyPressed = false;
	}
}

// glfw: whenever the window size changed (by OS or user resize) this callback function executes
void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
	// make sure the viewport matches the new window dimensions; note that width and 
	// height will be significantly larger than specified on retina displays.
	glViewport(0, 0, width, height);
}

// glfw: whenever the mouse moves, this callback is called
void mouse_callback(GLFWwindow* window, double xpos, double ypos)
{
	if (firstMouse)
	{
		lastX = xpos;
		lastY = ypos;
		firstMouse = false;
	}

	float xoffset = xpos - lastX;
	float yoffset = lastY - ypos; // reversed since y-coordinates go from bottom to top

	lastX = xpos;
	lastY = ypos;

	camera.ProcessMouseMovement(xoffset, yoffset);
}

// glfw: whenever the mouse scroll wheel scrolls, this callback is called
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset)
{
	camera.ProcessMouseScroll(yoffset);
}

// utility function for loading a 2D texture from file
unsigned int loadTexture(char const* path, bool gammaCorrection)
{
	unsigned int textureID;
	glGenTextures(1, &textureID);

	int width, height, nrComponents;
	unsigned char* data = stbi_load(path, &width, &height, &nrComponents, 0);
	if (data)
	{
		GLenum internalFormat;
		GLenum dataFormat;
		if (nrComponents == 1)
		{
			internalFormat = dataFormat = GL_RED;
		}
		else if (nrComponents == 3)
		{
			internalFormat = gammaCorrection ? GL_SRGB : GL_RGB;
			dataFormat = GL_RGB;
		}
		else if (nrComponents == 4)
		{
			internalFormat = gammaCorrection ? GL_SRGB_ALPHA : GL_RGBA;
			dataFormat = GL_RGBA;
		}

		glBindTexture(GL_TEXTURE_2D, textureID);
		glTexImage2D(GL_TEXTURE_2D, 0, internalFormat, width, height, 0, dataFormat, GL_UNSIGNED_BYTE, data);
		glGenerateMipmap(GL_TEXTURE_2D);

		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		stbi_image_free(data);
	}
	else
	{
		std::cout << "Texture failed to load at path: " << path << std::endl;
		stbi_image_free(data);
	}

	return textureID;
}

//创建shader storage buffer object(SSBO)
GLuint createPrintBuffer(unsigned size)
{
	GLuint printBuffer;
	glCreateBuffers(1, &printBuffer);
	glNamedBufferData(printBuffer, size * sizeof(unsigned), nullptr, GL_STREAM_READ);
	return printBuffer;
}

//删除SSBO
void deletePrintBuffer(GLuint printBuffer)
{
	glDeleteBuffers(1, &printBuffer);
}

//绑定SSBO，在 glUseProgram 后 draw 前调用
void bindPrintBuffer(GLuint program, GLuint printBuffer)
{
	// reset the buffer; only first value relevant (writing position / size of output), rest is filled up to the index this states
	unsigned beginIterator = 0u;
	glNamedBufferSubData(printBuffer, 0, sizeof(unsigned), &beginIterator);

	GLint binding;
	GLenum prop = GL_BUFFER_BINDING;
	// get the binding that our printf buffer happened to be given
	glGetProgramResourceiv(program, GL_SHADER_STORAGE_BLOCK, glGetProgramResourceIndex(program, GL_SHADER_STORAGE_BLOCK, "printfBuffer"), 1, &prop, sizeof(binding), nullptr, &binding);

	// bind to whatever slot we happened to get
	glBindBufferBase(GL_SHADER_STORAGE_BUFFER, binding, printBuffer);
}

//从VRAM获取SSBO输出数据，转换为std::string
std::string getPrintBufferString(GLuint printBuffer)
{
	//获取输出数据尺寸和buffer指针
	unsigned printedSize, bufferSize;
	glGetNamedBufferSubData(printBuffer, 0, sizeof(unsigned), &printedSize);
	glGetNamedBufferParameteriv(printBuffer, GL_BUFFER_SIZE, (GLint*)&bufferSize);
	bufferSize /= sizeof(unsigned);

	//防止读取越界
	if (printedSize > bufferSize)
		printedSize = bufferSize;

	//用vector作为print buffer的内存拷贝区
	std::vector<unsigned> printfData(printedSize + 1);
	printfData[0] = printedSize;

	//读取数据
	glGetNamedBufferSubData(printBuffer, sizeof(unsigned), GLsizei((printfData.size() - 1) * sizeof(unsigned)), printfData.data() + 1);

	std::string result;

	// to hold the temporary results of formatting
	char intermediate[1024];

	// this loop parses the formatting of the result
	for (size_t i = 1; i < printfData[0]; ++i) {
		// % indicates the beginning of a formatted input
		if (printfData[i] == '%') {
			// if followed by another %, we're actually supposed to print '%'
			if (printfData[i + 1] == '%') {
				result += "%";
				i++;
			}
			// otherwise we'll be printing numbers
			else {
				// first parse out the possible vector size
				int vecSize = 1;
				std::string format;
				while (std::string(1, printfData[i]).find_first_of("eEfFgGdiuoxXaA") == std::string::npos) {
					if (printfData[i] == '^') {
						vecSize = printfData[i + 1] - '0';
						i += 2;
					}
					else {
						format = format + std::string(1, printfData[i]);
						i++;
					}
				}
				format += printfData[i];

				// determine whether we'll have to do type conversion
				bool isFloatType = std::string(1, printfData[i]).find_first_of("diuoxX") == std::string::npos;

				// print to a temporary buffer, add result to string (for vectors add parentheses and commas as in "(a, b, c)") 
				if (vecSize > 1) {
					result += "(";
				}

				for (int j = 0; j < vecSize; ++j) {
					i++;
					if (isFloatType)
						snprintf(intermediate, 1024, format.c_str(), *((float*)&printfData[i]));
					else
						snprintf(intermediate, 1024, format.c_str(), printfData[i]);
					result += std::string(intermediate);
					if (vecSize > 1 && j < vecSize - 1) {
						result += ", ";
					}
				}

				if (vecSize > 1) result += ")";
			}
		}
		else {
			// otherwise it's a single character, just add it to the result
			result += std::string(1, printfData[i] == 0 ? '0' : printfData[i]);
		}
	}
	// ... and we're done.
	return result;
}