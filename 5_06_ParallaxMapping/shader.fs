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
    //1����ͳ���Ӳ�ӳ��
    ////�õ���ǰfragment�ĸ߶�H(A)
    //float height =  texture(depthMap, texCoords).r;
    ////�������ά����P��viewDir������x��yԪ�������߿ռ��У��ȳ�������zԪ�أ�����fragment�ĸ߶ȶ�����������
    ////����ͬʱ�����һ��heightScale��uniform����0.1�滻ΪheightScale��������һЩ����Ŀ��ƣ���Ϊ�Ӳ�Ч�����û��һ�����Ų���ͨ�������ǿ��
    ////��һ���ط���Ҫע�⣬����viewDir.xy����viewDir.z����ΪviewDir�����Ǿ����˱�׼���ģ�viewDir.z��ֵ����0.0��1.0֮�䡣��viewDir����ƽ���ڱ���ʱ������zԪ�ؽӽ���0.0�������᷵�ر�viewDir��ֱ�ڱ����ʱ������P���������ԣ��ӱ����ϣ������������棬�����нǶȵؿ���ƽ��ʱ�����ǻ����̶ȵ�����P�Ĵ�С���Ӷ��������������ƫ�ƣ����������ӽ��ϻ��ø������ʵ�ȡ�
    //vec2 p = viewDir.xy / viewDir.z * (height * 0.1);
    ////�����������ȥ��ά����P��������յľ���λ����������
    //return texCoords - p;



    //2���������������Ӳ�ӳ��(Steep Parallax Mapping)
    ////��ʹ�÷���1ʱ�����������һ���Ƚ�ˮƽ�ĽǶ��ϣ����߸߶�ͼ�����һ�󣬱���ͻ���ʾ��һ����Ϳ���������������ԭ�������ǵĲ�����ʽ̫�ֲ��ˣ�ֻ�Ǽ�����һ��������Ͳ����ˡ����ֲִڵĲ�����ʽ�ڸ߶ȱ仯���һ��߽Ƕȵ��������¾ͱ����ˡ�����ķ���Ҳ�ܼ򵥣���Ȼ����һ��̫�٣����Ƕ�������β������ˣ����ֶ�β����ķ�ʽ�����������Ӳ���ͼ��
    ////�����Ӳ�ӳ��Ļ���˼���ǽ�����ȷ�Χ����Ϊ����㡣����P����������ÿһ������������꣬���ϵ��±���ÿһ�㣬��ÿ���������ཻ��������ͼ����ֵ���жԱȡ�������������ֵС�������ͼ�Ĳ���ֵ���ͼ���������һ�㣬ֱ����һ�����ȴ����ཻ�������ͼ����ֵ����ʱ�������ڣ�����λ�Ƶģ������·���ʹ����һ����P�Ľ�����������������������ͼ������
    //const float numLayers = 10; //��������������Ϊ�̶�10��
    ////��������������ĽǶȵ��������������ڴ�ֱ��ʱʹ�ø��ٵ��������뷨�߼н�����ʱ��������������
    ////const float minLayers = 8;
    ////const float maxLayers = 32;
    ////float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));
    //float layerDepth = 1.0 / numLayers; //ÿһ������
    //float currentLayerDepth = 0.0; //��ǰ�����
    //vec2 p = viewDir.xy * 0.1; //�������������������Χ����Ϊ�Ӳ�Ч�����û��һ�����Ų���ͨ�������ǿ��
    //vec2 deltaTexCoords = p / numLayers; //ÿһ������ı仯ֵ
    //vec2 currentTexCoords = texCoords; //��ʼ��������
    //float currentDepthMapValue = texture(depthMap, currentTexCoords).r; //��ʼ���
    //while(currentLayerDepth < currentDepthMapValue)
    //{
    //    currentTexCoords -= deltaTexCoords; //p������ָ���۾��ģ�������ֵ��Ҫ��������仯��������-deltaTexCoords
    //    currentDepthMapValue = texture(depthMap, currentTexCoords).r;
    //    currentLayerDepth += layerDepth;
    //}
    //return currentTexCoords;


    //�����Ӳ���ͼͬ�����Լ������⡣��Ϊ��������ǻ������޵����������ģ����ǻ��������Ч���Լ�ͼ��֮�������ԵĶϲ㡣
    //���ǿ���ͨ�����������ķ�ʽ����������⣬���Ǻܿ�ͻỨ�Ѻܶ����ܡ�


    //��Щּ���޸��������ķ�������ʹ�õ��ڱ���ĵ�һ����P�Ľ���λ�ã������������ӽ�����Ȳ���в�ֵ�ҳ���ƥ��B�ġ�
    //���������еĽ���������������Ӳ���ͼ(Relief Parallax Mapping)���Ӳ��ڱ���ͼ(Parallax Occlusion Mapping)��
    //�����Ӳ���ͼ����ȷһЩ�����Ǳ��Ӳ��ڱ���ͼ���ܿ������ࡣ��Ϊ�Ӳ��ڱ���ͼ��Ч����ǰ�߲�൫��Ч�ʸ��ߣ�������ַ�ʽ������ʹ�á�


    //3���Ӳ��ڱ���ͼ(Parallax Occlusion Mapping)
    //�Ӳ��ڱ���ͼ��ԭ������ȡ�뽻�����ڵ�������Ĳ���ȺͲ�����ȣ�������������ֵ��Ȩ�أ�����Ȩ�ز���������������֮��ĳ��λ�õ���������
    //��������������ĽǶȵ�����������
    const float minLayers = 8;
    const float maxLayers = 32;
    float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));
    float layerDepth = 1.0 / numLayers; //ÿһ������
    float currentLayerDepth = 0.0; //��ǰ�����
    //ÿ�����������ƫ����(from vector P)
    vec2 P = viewDir.xy / viewDir.z * heightScale;
    vec2 deltaTexCoords = P / numLayers;
    vec2  currentTexCoords = texCoords; //��ʼ��������
    float currentDepthMapValue = texture(depthMap, currentTexCoords).r; //��ʼ���
      
    while(currentLayerDepth < currentDepthMapValue)
    {
        //����P�ķ������ƶ���������
        currentTexCoords -= deltaTexCoords;
        //��ȡ��һ����ײ���������괦�������ͼֵ
        currentDepthMapValue = texture(depthMap, currentTexCoords).r;
        //��ȡ��һ������
        currentLayerDepth += layerDepth;
    }

    //https://segmentfault.com/a/1190000003920502
    //��ֵ���㹫ʽ��
    //nextHeight = H(T3) - currentLayerHeight
    //prevHeight = H(T2) - (currentLayerHeight - layerHeight)
    //weight = nextHeight / (nextHeight - prevHeight)
    //Tp = T(T2) * weight + T(T3) * (1.0 - weight)
    
    //��ײǰ����������(�������)
    vec2 prevTexCoords = currentTexCoords + deltaTexCoords;
    //��ײ�����������
    vec2 afterTexCoords = currentTexCoords;
    //��һ����ײ�����ڲ�Ĳ����������ͼֵ֮�С��0
    float afterDepthDistance = currentDepthMapValue - currentLayerDepth;
    //��һ����ײ�������ͼֵ�����ڲ�Ĳ���֮�����0
    float beforeDepthDistance = texture(depthMap, prevTexCoords).r - (currentLayerDepth - layerDepth);
 
    //ͨ����ֵ�ó��������꣬��ֵ�������������V��H(T2)��H(T3)�߶ȵ����ߵĽ�����
    float weight = afterDepthDistance / (afterDepthDistance - beforeDepthDistance); //���ý��С��0���������������ھ���ֵ���
    vec2 finalTexCoords = prevTexCoords * weight + afterTexCoords * (1.0 - weight);
    return finalTexCoords;

    //�Ӳ��ڱ�ӳ�����ʹ����Խ��ٵĲ������������ܺõĽ�������Ӳ��ڱ�ӳ��ȸ����Ӳ�ӳ������������߶�ͼ�е�Сϸ�ڣ�Ҳ�������ڸ߶�ͼ���ݲ�������ȵı仯ʱ�õ�����Ľ����
    //���б�ʹ��󣬲�ֵ���������������������ײ�����߽��㣬��ƫ�����Ҳ�



    //4�������Ӳ���ͼ��Relief Parallax Mapping��
    ////https://www.jianshu.com/p/98c137baf855
    ////�����Ӳ���ͼ��ԭ��Ͷ����Ӳ���ͼ���ƣ��������������������ڻ�ȡ���˽��������������ڵ����������������Ϣ֮�������Ӳ��ڱ���ͼ���ƣ����ٶ�����бƽ������õķ�����2�ֽ���������ȷ�����������������֮��ȡ��������е�λ�ã���������������������Ϣ�������������ϢС�ڲ���ȣ���ô����е������ȡ��ԭ�е�������㣻�����������Ϣ���ڲ���ȣ���ô����е������ȡ��ԭ�е�������㡣Ȼ�����ȡ�е㣬�����Ƚϣ��������һ������֮�󣬲���������������ͷǳ��ӽ���ʵ�����ˡ�
    ////��������������ĽǶȵ�����������
    //const float minLayers = 8;
    //const float maxLayers = 32;
    //float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));
    //float layerDepth = 1.0 / numLayers; //ÿһ������
    //float currentLayerDepth = 0.0; //��ǰ�����
    //vec2 p = viewDir.xy * 0.1; //�������������������Χ����Ϊ�Ӳ�Ч�����û��һ�����Ų���ͨ�������ǿ��
    //vec2 deltaTexCoords = p / numLayers; //ÿһ������ı仯ֵ
    //vec2 currentTexCoords = texCoords; //��ʼ��������
    //float currentDepthMapValue = texture(depthMap, currentTexCoords).r; //��ʼ���
    //while(currentLayerDepth < currentDepthMapValue)
    //{
    //    currentTexCoords -= deltaTexCoords; //p������ָ���۾��ģ�������ֵ��Ҫ��������仯��������-deltaTexCoords
    //    currentDepthMapValue = texture(depthMap, currentTexCoords).r;
    //    currentLayerDepth += layerDepth;
    //}
    ////��ʼ���ж��ֽ���
    //vec2 dtex = deltaTexCoords / 2; //������ȡ��
    //float deltaLayerDepth = layerDepth / 2; //��Ȳ���ȡ��
    ////���㵱ǰ������Ͳ����
    //currentTexCoords += dtex;
    //currentLayerDepth -= deltaLayerDepth;
    //const int numSearches = 10; //����10��2�ֽ���
    //for (int i = 0; i < numSearches; ++i)
    //{
    //    //ÿ������������Ȳ����������
    //    dtex /= 2;
    //    deltaLayerDepth /= 2;
    //    //������ǰ����
    //    float currentDepthMapValue = texture(depthMap, currentTexCoords).r;
    //    if (currentDepthMapValue > currentLayerDepth)
    //    {
    //        //�����ǰ��ȴ��ڲ���ȣ�����ƽ�
    //        currentTexCoords  -= dtex;
    //        currentLayerDepth += deltaLayerDepth;
    //    }
    //    else {
    //        //�����ǰ���С�ڲ���ȣ����ұƽ�
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