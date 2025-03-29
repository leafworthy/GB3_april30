// custom property values
float _Lit;

// game lightmaps
sampler2D _GameTexture1; float4 _GameRect1; float _GameRotation1;
sampler2D _GameTexture2; float4 _GameRect2; float _GameRotation2;
sampler2D _GameTexture3; float4 _GameRect3; float _GameRotation3;
sampler2D _GameTexture4; float4 _GameRect4; float _GameRotation4;

// scene lightmaps
sampler2D _SceneTexture1; float4 _SceneRect1; float _SceneRotation1;
sampler2D _SceneTexture2; float4 _SceneRect2; float _SceneRotation2;
sampler2D _SceneTexture3; float4 _SceneRect3; float _SceneRotation3;
sampler2D _SceneTexture4; float4 _SceneRect4; float _SceneRotation4;

// day lighting
float _Day_Direction;
float _Day_Height;

// normal map & bump mapping
sampler2D _NormalMap;

// properties are not clear enough
float _LightIntensity;
float _Specular;

float _OcclusionLit;

float _OcclusionOffset; 
float _OcclusionDepth;

// height map
sampler2D _HeightMap;

///// Id API

sampler2D GetGameLightmap(int id)
{
    switch(id)
    {
        case 1:
            return(_GameTexture1);

        case 2:
            return(_GameTexture2);

        case 3:
            return(_GameTexture3);

        case 4:
            return(_GameTexture4);
    }

    return(_GameTexture1);
}

float4 GetGameRect(int id)
{
    switch(id)
    {
        case 1:
            return(_GameRect1);

        case 2:
            return(_GameRect2);

        case 3:
            return(_GameRect3);

        case 4:
            return(_GameRect4);
    }

    return(float4(0, 0, 0, 0));
}

float GetLightmapRotation(int id) // lightmap? not game
{ 
    switch(id)
    {
        case 1:
            return(_GameRotation1);

        case 2:
            return(_GameRotation2);

        case 3:
            return(_GameRotation3);

        case 4:
            return(_GameRotation4);
    }

    return(0);
}

sampler2D GetSceneLightmap(int id)
{
    switch(id)
    {
        case 1:
            return(_SceneTexture1);

        case 2:
            return(_SceneTexture2);

        case 3:
            return(_SceneTexture3);

        case 4:
            return(_SceneTexture4);
    }
}

float4 GetSceneRect(int id)
{
    switch(id)
    {
        case 1:
            return(_SceneRect1);

        case 2:
            return(_SceneRect2);

        case 3:
            return(_SceneRect3);

        case 4:
            return(_SceneRect4);
    }

    return(float4(0, 0, 0, 0));
}

float GetSceneRotation(int id)
{
    switch(id)
    {
        case 1:
            return(_SceneRotation1);

        case 2:
            return(_SceneRotation2);

        case 3:
            return(_SceneRotation3);

        case 4:
            return(_SceneRotation4);
    }

    return(0);
}

///// core functions /////

float2 TransformToCamera(float2 pos, float rotation)
{
    float c = cos(-rotation);
    float s = sin(-rotation);

    float x = pos.x;
    float y = pos.y;

    pos.x = x * c - y * s;
    pos.y = x * s + y * c;

    return(pos);
}

bool InCamera (float2 pos, float2 rectSize)
{
    float2 rectPos = -rectSize * 0.5;

    return !(pos.x < rectPos.x || pos.x > rectPos.x + rectSize.x || pos.y < rectPos.y || pos.y > rectPos.y + rectSize.y);
}

// ID wrapper

bool InGameCamera(int id, float2 worldPos)
{
    float4 rect = GetGameRect(id);

    if (rect.z <= 0) return(false);

    return(InCamera(TransformToCamera(worldPos - float2(rect.x, rect.y),  GetLightmapRotation(id)), float2(rect.z, rect.w)));
}

bool InSceneCamera(int id, float2 worldPos)
{
    float4 rect = GetSceneRect(id);

    if (rect.z <= 0) return(false);

    return(InCamera(TransformToCamera(worldPos - float2(rect.x, rect.y),  GetSceneRotation(id)), float2(rect.z, rect.w)));
}

//

float3 LightmapGame(int id, float2 texcoord)
{
    return(tex2D (GetGameLightmap(id), texcoord));
}

float3 LightmapScene(int id, float2 texcoord)
{
    return(tex2D (GetSceneLightmap(id), texcoord));
}

// day lighting functions

float2 SL2D_DayOffset(float value)
{
    return float2(cos(_Day_Direction) * value, sin(_Day_Direction) * value);
}  

float2 SL2D_DayOffsetByHeightMap(float2 uv)
{
    float offset = 0.5 - tex2D(_HeightMap, uv).r;

    return float2(cos(_Day_Direction) * offset, sin(_Day_Direction) * offset);
}  

///// lightmap color functions /////

float3 SL2D_Light(float2 worldPos) // all lightmaps
{ 
    float rotation;
    float2 cameraSize, localPosition;
    float3 color = float3(1, 1, 1);

    // game view

    float4 rect = GetGameRect(1);

    if (rect.z > 0)
    {
        rotation = GetLightmapRotation(1);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y), rotation);

        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapGame(1, (localPosition + cameraSize / 2) / cameraSize);
        }
    }
 
    rect = GetGameRect(2);

    if (rect.z > 0)
    {
        rotation = GetLightmapRotation(2);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);
        
        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapGame(2, (localPosition + cameraSize / 2) / cameraSize);
        }
    } 

    rect = GetGameRect(3);

    if (rect.z > 0)
    {
        rotation = GetLightmapRotation(3);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);
        
        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapGame(3, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    rect = GetGameRect(4);

    if (rect.z > 0)
    {
        rotation = GetLightmapRotation(4);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);
        
        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapGame(4, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    // scene view

    rect = GetSceneRect(1);

    if (rect.z > 0)
    {
        rotation = GetSceneRotation(1);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y), rotation);

        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapScene(1, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    rect = GetSceneRect(2);

    if (rect.z > 0)
    {
        rotation = GetSceneRotation(2);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);
        
        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapScene(2, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    rect = GetSceneRect(3);

    if (rect.z > 0)
    {
        rotation = GetSceneRotation(3);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);
        
        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapScene(3, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    rect = GetSceneRect(4);

    if (rect.z > 0)
    {
        rotation = GetSceneRotation(4);

        cameraSize = float2(rect.z, rect.w);
        localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);
        
        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapScene(4, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    return(color);
}

///// extended lighting functions - all lightmaps /////

float3 SL2D_Lit(float2 worldPos)
{
    float3 lightPixel = SL2D_Light(worldPos);

    lightPixel = lerp(lightPixel, float3(1, 1, 1), 1 - _Lit);

    return lightPixel;
}

float SL2D_FogOfWar(float2 worldPos)
{
    float3 lightPixel = SL2D_Light(worldPos);

    return (lightPixel.r + lightPixel.g + lightPixel.b) / 3;
}

///// individual pass functions /////

float3 SL2D_Pass(int id, float2 worldPos)
{
    float3 color = float3(1, 1, 1);

    float4 rect = GetGameRect(id);

    if (rect.z > 0)
    {
        float rotation = GetLightmapRotation(id);

        float2 cameraSize = float2(rect.z, rect.w);
        float2 localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);

        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapGame(id, (localPosition + cameraSize / 2) / cameraSize);
        }
    }
        
    // scene

    rect = GetSceneRect(id);

    if (rect.z > 0)
    {
        float rotation = GetSceneRotation(id);

        float cameraSize = float2(rect.z, rect.w);
        float localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);

        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapScene(id, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    return(color);
}

float SL2D_Depth(int id, float2 worldPos)
{
    float depth = 0;

    float4 rect = GetGameRect(id);

    if (rect.z > 0)
    {
        float rotation = GetLightmapRotation(id);

        float2 cameraSize = float2(rect.z, rect.w);
        float2 localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);

        if (InCamera(localPosition, cameraSize))
        {
            depth = LightmapGame(id, (localPosition + cameraSize / 2) / cameraSize).r;
        }
    }
        
    // scene

    rect = GetSceneRect(id);

    if (rect.z > 0)
    {
        float rotation = GetSceneRotation(id);

        float cameraSize = float2(rect.z, rect.w);
        float localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);

        if (InCamera(localPosition, cameraSize))
        {
            depth = LightmapScene(id, (localPosition + cameraSize / 2) / cameraSize).r;
        }
    }

    return(depth * 255 - 101);
}

// black outside

float3 SL2D_Pass_Black(int id, float2 worldPos)
{
    float3 color = float3(0, 0, 0);

    float4 rect = GetGameRect(id);

    if (rect.z > 0)
    {
        float rotation = GetLightmapRotation(id);

        float2 cameraSize = float2(rect.z, rect.w);
        float2 localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);

        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapGame(id, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    rect = GetSceneRect(id);

    if (rect.z > 0)
    {
        float rotation = GetSceneRotation(id);

        float cameraSize = float2(rect.z, rect.w);
        float localPosition = TransformToCamera(worldPos - float2(rect.x, rect.y),  rotation);

        if (InCamera(localPosition, cameraSize))
        {
            color = LightmapScene(id, (localPosition + cameraSize / 2) / cameraSize);
        }
    }

    return(color);
}

///// extended individual pass functions /////

float3 SL2D_Lit_Pass(int id, float2 worldPos)
{
    float3 lightPixel = SL2D_Pass(id, worldPos);

    lightPixel = lerp(lightPixel, float3(1, 1, 1), 1 - _Lit);

    return lightPixel;
}

float3 SL2D_DayBump(float2 uv)
{
    float3 normalDirection = UnpackNormal(tex2D(_NormalMap, uv)).xyz;

    normalDirection = float3(mul(float4(normalDirection.xyz, 1.0f), unity_WorldToObject).xyz);
    normalDirection.z *= -1;
    normalDirection = normalize(normalDirection);

    float lightHorizontal = cos(_Day_Direction);
    float lightVertical = sin(_Day_Direction);
    float3 lightDirection = normalize(float3(lightHorizontal, lightVertical,- 0.75f * _Day_Height * 2));

    float normalDotLight = dot(normalDirection, lightDirection);

    float specularLevel = 0.0f;

    if (normalDotLight > 0.0f)
    {
        float specular = _LightIntensity * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), float3(0.0f, 0.0f, -1.0f))), 10);
        
        specularLevel = lerp(0, specular, _Specular);
    }

    float diffuseLevel = _LightIntensity * max(0.0f, normalDotLight);

    float3 diffuseAndSpecular = diffuseLevel + specularLevel;

    return diffuseAndSpecular;
}

// light normal map functions (only red channel)
float3 SL2D_Bump_Pass(int id, float2 worldPos, float2 texcoord)
{
    float delta = _OcclusionOffset;

    // light pixel
    float3 lightPixelLeft = SL2D_Pass(id, worldPos + float2(-delta, 0));
    float3 lightPixelRight = SL2D_Pass(id, worldPos + float2(delta, 0));

    float3 lightPixelUp = SL2D_Pass(id, worldPos + float2(0, delta));
    float3 lightPixelDown = SL2D_Pass(id, worldPos + float2(0, -delta));

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
        
        specularLevel = lerp(0, specular, 0.5); // _Specular
    }

    float3 diffuseAndSpecular = diffuseLevel + specularLevel; 

    float3 result = diffuseAndSpecular;

    float l = (lightPixelLeft + lightPixelRight + lightPixelUp + lightPixelDown) * 0.25;
    float3 lightPixel = result * l + 1; //?? 

    // lightPixel = lerp(lightPixel, float4(1, 1, 1, 1), 1 - _Lit);

    return lightPixel;
}