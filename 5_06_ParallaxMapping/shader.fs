#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 TangentLightPos;
    vec3 TangentViewPos;
    vec3 TangentFragPos;
} fs_in;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D depthMap;

uniform float heightScale;

vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir)
{
    //1、传统的视差映射
    ////得到当前fragment的高度H(A)
    //float height =  texture(depthMap, texCoords).r;
    ////计算出二维向量P，viewDir向量的x和y元素在切线空间中，先除以它的z元素，再用fragment的高度对它进行缩放
    ////可以同时引入额一个heightScale的uniform，将0.1替换为heightScale，来进行一些额外的控制，因为视差效果如果没有一个缩放参数通常会过于强烈
    ////有一个地方需要注意，就是viewDir.xy除以viewDir.z。因为viewDir向量是经过了标准化的，viewDir.z的值会在0.0到1.0之间。当viewDir大致平行于表面时，它的z元素接近于0.0，除法会返回比viewDir垂直于表面的时候更大的P向量。所以，从本质上，相比正朝向表面，当带有角度地看向平面时，我们会更大程度地缩放P的大小，从而增加纹理坐标的偏移，这样做在视角上会获得更大的真实度。
    //vec2 p = viewDir.xy / viewDir.z * (height * 0.1);
    ////用纹理坐标减去二维向量P来获得最终的经过位移纹理坐标
    //return texCoords - p;



    //2、多样本，陡峭视差映射(Steep Parallax Mapping)
    ////当使用方法1时，如果视线在一个比较水平的角度上，或者高度图的落差一大，表面就会显示的一塌糊涂。出现这种情况的原因是我们的采样方式太粗糙了，只是计算了一个采样点就采样了。这种粗糙的采样方式在高度变化剧烈或者角度刁钻的情况下就崩盘了。解决的方法也很简单，既然采样一次太少，我们多采样几次不就行了？这种多次采样的方式被称作陡峭视差贴图。
    ////陡峭视差映射的基本思想是将总深度范围划分为多个层。沿着P向量方向在每一层采样纹理坐标，从上到下遍历每一层，把每个层深与相交点的深度贴图采样值进行对比。如果这个层的深度值小于深度贴图的采样值，就继续采样下一层，直到有一层的深度大于相交点深度贴图采样值，此时这个点就在（经过位移的）表面下方。使用这一层与P的交点所在纹理坐标进行深度贴图采样。
    //const float numLayers = 10; //将整个深度区域分为固定10层
    ////根据相机看向表面的角度调整样本数量，在垂直看时使用更少的样本，与法线夹角增大时增加样本数量：
    ////const float minLayers = 8;
    ////const float maxLayers = 32;
    ////float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));
    //float layerDepth = 1.0 / numLayers; //每一层的深度
    //float currentLayerDepth = 0.0; //当前层深度
    //vec2 p = viewDir.xy * 0.1; //修正纹理坐标的完整范围，因为视差效果如果没有一个缩放参数通常会过于强烈
    //vec2 deltaTexCoords = p / numLayers; //每一层纹理的变化值
    //vec2 currentTexCoords = texCoords; //起始纹理坐标
    //float currentDepthMapValue = texture(depthMap, currentTexCoords).r; //起始深度
    //while(currentLayerDepth < currentDepthMapValue)
    //{
    //    currentTexCoords -= deltaTexCoords; //p向量是指向眼睛的，而纹理值需要往反方向变化，所以是-deltaTexCoords
    //    currentDepthMapValue = texture(depthMap, currentTexCoords).r;
    //    currentLayerDepth += layerDepth;
    //}
    //return currentTexCoords;


    //陡峭视差贴图同样有自己的问题。因为这个技术是基于有限的样本数量的，我们会遇到锯齿效果以及图层之间有明显的断层。
    //我们可以通过增加样本的方式减少这个问题，但是很快就会花费很多性能。


    //有些旨在修复这个问题的方法：不使用低于表面的第一层与P的交点位置，而是在两个接近的深度层进行插值找出更匹配B的。
    //两种最流行的解决方法叫做浮雕视差贴图(Relief Parallax Mapping)和视差遮蔽贴图(Parallax Occlusion Mapping)，
    //浮雕视差贴图更精确一些，但是比视差遮蔽贴图性能开销更多。因为视差遮蔽贴图的效果和前者差不多但是效率更高，因此这种方式更经常使用。


    //3、视差遮蔽贴图(Parallax Occlusion Mapping)
    //视差遮蔽贴图的原理，就是取与交点相邻的两个层的层深度和采样深度，计算出两个深度值的权重，根据权重采样两个纹理坐标之间某个位置的纹理坐标
    //根据相机看向表面的角度调整样本数量
    const float minLayers = 8;
    const float maxLayers = 32;
    float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));
    float layerDepth = 1.0 / numLayers; //每一层的深度
    float currentLayerDepth = 0.0; //当前层深度
    //每层纹理坐标的偏移量(from vector P)
    vec2 P = viewDir.xy / viewDir.z * heightScale;
    vec2 deltaTexCoords = P / numLayers;
    vec2  currentTexCoords = texCoords; //起始纹理坐标
    float currentDepthMapValue = texture(depthMap, currentTexCoords).r; //起始深度
      
    while(currentLayerDepth < currentDepthMapValue)
    {
        //沿着P的反方向移动纹理坐标
        currentTexCoords -= deltaTexCoords;
        //获取下一个碰撞点纹理坐标处的深度贴图值
        currentDepthMapValue = texture(depthMap, currentTexCoords).r;
        //获取下一层的深度
        currentLayerDepth += layerDepth;
    }

    //https://segmentfault.com/a/1190000003920502
    //插值计算公式：
    //nextHeight = H(T3) - currentLayerHeight
    //prevHeight = H(T2) - (currentLayerHeight - layerHeight)
    //weight = nextHeight / (nextHeight - prevHeight)
    //Tp = T(T2) * weight + T(T3) * (1.0 - weight)
    
    //碰撞前的纹理坐标(反向操作)
    vec2 prevTexCoords = currentTexCoords + deltaTexCoords;
    //碰撞后的纹理坐标
    vec2 afterTexCoords = currentTexCoords;
    //下一个碰撞点所在层的层深与深度贴图值之差，小于0
    float afterDepthDistance = currentDepthMapValue - currentLayerDepth;
    //上一个碰撞点深度贴图值与所在层的层深之差，大于0
    float beforeDepthDistance = texture(depthMap, prevTexCoords).r - (currentLayerDepth - layerDepth);
 
    //通过插值得出纹理坐标，插值结果是在视向量V和H(T2)和H(T3)高度的连线的交点上
    float weight = afterDepthDistance / (afterDepthDistance - beforeDepthDistance); //所得结果小于0，负数减正数等于绝对值相加
    vec2 finalTexCoords = prevTexCoords * weight + afterTexCoords * (1.0 - weight);
    return finalTexCoords;

    //视差遮蔽映射可以使用相对较少的采样次数产生很好的结果。但视差遮蔽映射比浮雕视差映射更容易跳过高度图中的小细节，也更容易在高度图数据产生大幅度的变化时得到错误的结果。
    //如果斜率过大，插值结果，即视线向量与两碰撞点连线交点，会偏向于右侧



    //4、浮雕视差贴图（Relief Parallax Mapping）
    ////https://www.jianshu.com/p/98c137baf855
    ////浮雕视差贴图的原理和陡峭视差贴图类似，不过它更聪明。它是在获取到了交点左右两个相邻点的纹理坐标和深度信息之后（这点和视差遮蔽贴图类似），再对其进行逼近，采用的方法是2分渐进。就是确定了左右两个坐标点之后，取两坐标的中点位置，用这个坐标来采样深度信息，如果这个深度信息小于层深度，那么这个中点坐标就取代原有的左坐标点；如果这个深度信息大于层深度，那么这个中点坐标就取代原有的右坐标点。然后继续取中点，再做比较，如此往复一定次数之后，采样到的纹理坐标就非常接近真实坐标了。
    ////根据相机看向表面的角度调整样本数量
    //const float minLayers = 8;
    //const float maxLayers = 32;
    //float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));
    //float layerDepth = 1.0 / numLayers; //每一层的深度
    //float currentLayerDepth = 0.0; //当前层深度
    //vec2 p = viewDir.xy * 0.1; //修正纹理坐标的完整范围，因为视差效果如果没有一个缩放参数通常会过于强烈
    //vec2 deltaTexCoords = p / numLayers; //每一层纹理的变化值
    //vec2 currentTexCoords = texCoords; //起始纹理坐标
    //float currentDepthMapValue = texture(depthMap, currentTexCoords).r; //起始深度
    //while(currentLayerDepth < currentDepthMapValue)
    //{
    //    currentTexCoords -= deltaTexCoords; //p向量是指向眼睛的，而纹理值需要往反方向变化，所以是-deltaTexCoords
    //    currentDepthMapValue = texture(depthMap, currentTexCoords).r;
    //    currentLayerDepth += layerDepth;
    //}
    ////开始进行二分渐进
    //vec2 dtex = deltaTexCoords / 2; //纹理步长取半
    //float deltaLayerDepth = layerDepth / 2; //深度步长取半
    ////计算当前的纹理和层深度
    //currentTexCoords += dtex;
    //currentLayerDepth -= deltaLayerDepth;
    //const int numSearches = 10; //进行10次2分渐进
    //for (int i = 0; i < numSearches; ++i)
    //{
    //    //每次纹理步长和深度步长都会减半
    //    dtex /= 2;
    //    deltaLayerDepth /= 2;
    //    //采样当前纹理
    //    float currentDepthMapValue = texture(depthMap, currentTexCoords).r;
    //    if (currentDepthMapValue > currentLayerDepth)
    //    {
    //        //如果当前深度大于层深度，往左逼近
    //        currentTexCoords  -= dtex;
    //        currentLayerDepth += deltaLayerDepth;
    //    }
    //    else {
    //        //如果当前深度小于层深度，往右逼近
    //        currentTexCoords  += dtex;
    //        currentLayerDepth -= deltaLayerDepth;
    //    }
    //}
    //return currentTexCoords;
}

void main()
{           
    // offset texture coordinates with Parallax Mapping
    vec3 viewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);
    vec2 texCoords = ParallaxMapping(fs_in.TexCoords,  viewDir);

    if(texCoords.x > 1.0 || texCoords.y > 1.0 || texCoords.x < 0.0 || texCoords.y < 0.0)
        discard;

    // obtain normal from normal map
    vec3 normal = texture(normalMap, texCoords).rgb;
    normal = normalize(normal * 2.0 - 1.0);   
   
    // get diffuse color
    vec3 color = texture(diffuseMap, texCoords).rgb;
    // ambient
    vec3 ambient = 0.1 * color;
    // diffuse
    vec3 lightDir = normalize(fs_in.TangentLightPos - fs_in.TangentFragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;
    // specular    
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

    vec3 specular = vec3(0.2) * spec;
    FragColor = vec4(ambient + diffuse + specular, 1.0);
}