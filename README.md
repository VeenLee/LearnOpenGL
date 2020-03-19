# LearnOpenGL

GLFW是一个专门针对OpenGL的C语言库，它提供了一些渲染物体所需的最低限度的接口。它允许用户创建OpenGL上下文，定义窗口参数以及处理用户输入。

由于OpenGL驱动版本众多，它大多数函数的位置都无法在编译时确定下来，需要在运行时查询。所以任务就落在了开发者身上，开发者需要在运行时获取函数地址并将其保存在一个函数指针中供以后使用。取得地址的方法因平台而异，所以非常繁琐。幸运的是，有些库能简化此过程，其中GLAD是目前最新，也是最流行的库。

https://learnopengl-cn.github.io

https://github.com/g-truc/glm 专门为OpenGL量身定做的数学库

https://github.com/nothings/stb/blob/master/stb_image.h

https://github.com/openglredbook/examples
https://github.com/danginsburg/opengles3-book

https://github.com/mmp/pbrt-v2
https://github.com/mmp/pbrt-v3

http://www.opengl-tutorial.org/
https://learnopengl.com/
https://www.jianshu.com/p/6bda18e953f6
http://ogldev.atspace.co.uk/index.html
https://blog.csdn.net/cordova/category_9266966.html
http://www.mbsoftworks.sk/tutorials/opengl4/
http://www.mbsoftworks.sk/tutorials/opengl3/
https://www.jianshu.com/p/b5f8627d9cbc

opengles3-book官方源码有问题，需要做如下修改：
edit opengles3-book-master\Common\Source\Win32\esUtil_win32.c<br/>
after ESContext *esContext = ( ESContext * ) ( LONG_PTR ) GetWindowLongPtr ( hWnd, GWL_USERDATA );<br/>
Add:<br/>
if (esContext == NULL)<br/>
{<br/>
    return DefWindowProc(hWnd, uMsg, wParam, lParam);<br/>
}
