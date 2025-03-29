// custom property values
float _Lit;

// game lightmaps
sampler2D _GameTexture1; vector _GameRect1; float _GameRotation1;
sampler2D _GameTexture2; vector _GameRect2; float _GameRotation2;
sampler2D _GameTexture3; vector _GameRect3; float _GameRotation3;
sampler2D _GameTexture4; vector _GameRect4; float _GameRotation4;

// scene lightmaps
sampler2D _SceneTexture1; vector _SceneRect1; float _SceneRotation1;
sampler2D _SceneTexture2; vector _SceneRect2; float _SceneRotation2;
sampler2D _SceneTexture3; vector _SceneRect3; float _SceneRotation3;
sampler2D _SceneTexture4; vector _SceneRect4; float _SceneRotation4;

// day lighting
float _Day_Direction;
float _Day_Height;

// normal map & bump mapping
sampler2D _NormalMap;

float _LightIntensity;
float _Specular;

float _OcclusionLit;

float _OcclusionOffset;
float _OcclusionDepth;

float3 SL2D_FAST_PASS_1(float2 worldPos)
{ 
    float4 rect = _GameRect1;
    float rotation = _GameRotation1;
    float2 cameraSize = float2(rect.z, rect.w);
    float2 localPosition = worldPos - float2(rect.x, rect.y);

    float c = cos(-rotation);
    float s = sin(-rotation);

    float x = localPosition.x;
    float y = localPosition.y;

    localPosition.x = x * c - y * s;
    localPosition.y = x * s + y * c;

    return tex2D (_GameTexture1, (localPosition + cameraSize / 2) / cameraSize);
}

float3 SL2D_FAST_PASS_2(float2 worldPos)
{ 
    float4 rect = _GameRect2;
    float rotation = _GameRotation2;
    float2 cameraSize = float2(rect.z, rect.w);
    float2 localPosition = worldPos - float2(rect.x, rect.y);

    float c = cos(-rotation);
    float s = sin(-rotation);

    float x = localPosition.x;
    float y = localPosition.y;

    localPosition.x = x * c - y * s;
    localPosition.y = x * s + y * c;

    return tex2D (_GameTexture2, (localPosition + cameraSize / 2) / cameraSize);
}

float3 SL2D_FAST_PASS_LIT_1(float2 worldPos)
{
    return lerp(SL2D_FAST_PASS_1(worldPos), float3(1, 1, 1), 1 - _Lit);
}

float3 SL2D_FAST_PASS_LIT_2(float2 worldPos)
{
    return lerp(SL2D_FAST_PASS_2(worldPos), float3(1, 1, 1), 1 - _Lit);
}


float3 SL2D_FAST_BUMP_PASS_1(float2 worldPos, float2 texcoord)
{
    float delta = _OcclusionOffset;

    // light pixel
    float3 lightPixelLeft = SL2D_FAST_PASS_1(worldPos + float2(-delta, 0));
    float3 lightPixelRight = SL2D_FAST_PASS_1(worldPos + float2(delta, 0));

    float3 lightPixelUp = SL2D_FAST_PASS_1(worldPos + float2(0, delta));
    float3 lightPixelDown = SL2D_FAST_PASS_1(worldPos + float2(0, -delta));

    float lightHorizontal = (lightPixelRight.r - lightPixelLeft.r) * _OcclusionDepth;
    float lightVertical = (lightPixelUp.r - lightPixelDown.r) * _OcclusionDepth;

    float3 lightDirection = normalize(float3(lightHorizontal, lightVertical, -1));

    float3 normalDirection = UnpackNormal(tex2D(_NormalMap, texcoord)).xyz;
    
    normalDirection = float3(mul(float4(normalDirection.xyz, 1.0f), unity_WorldToObject).xyz);
    normalDirection.z *= -1;
    normalDirection = normalize(normalDirection);

    float normalDotLight = dot(normalDirection, lightDirection);

    float diffuseLevel = max(0.0f, normalDotLight);

    float specularLevel = 0.0f;

    if (normalDotLight > 0.0f)
    {
        float specular = pow(max(0.0, dot(reflect(-lightDirection, normalDirection), float3(0.0f, 0.0f, -1.0f))), 10);
        
        specularLevel = lerp(0, specular, _Specular);
    }

    float3 diffuseAndSpecular = diffuseLevel + specularLevel; 

    float3 result = diffuseAndSpecular;

    float l = (lightPixelLeft + lightPixelRight + lightPixelUp + lightPixelDown) * 0.25;
    float3 lightPixel = result * l; //??  + 1

    // lightPixel = lerp(lightPixel, float4(1, 1, 1, 1), 1 - _Lit);

    return lightPixel;
}