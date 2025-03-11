Shader "TextMeshPro/Scrolling Fill"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        
        _ScrollTexture ("Scroll Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 1.0
        _ScrollDirection ("Scroll Direction", Vector) = (0,-1,0,0)
        
        // TextMeshPro specific properties
        _FaceDilate ("Face Dilate", Range(-1,1)) = 0
        _OutlineSoftness ("Outline Softness", Range(0,1)) = 0
        _OutlineWidth ("Outline Width", Range(0,1)) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        
        _GradientScale ("Gradient Scale", float) = 5
        _ScaleRatioA ("Scale RatioA", float) = 1
        _ScaleRatioB ("Scale RatioB", float) = 1
        _ScaleRatioC ("Scale RatioC", float) = 1
        
        _VertexOffsetX ("Vertex OffsetX", float) = 0
        _VertexOffsetY ("Vertex OffsetY", float) = 0
        
        _ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
        _MaskSoftnessX ("Mask SoftnessX", float) = 0
        _MaskSoftnessY ("Mask SoftnessY", float) = 0
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _CullMode ("Cull Mode", Float) = 0
        _ColorMask ("Color Mask", Float) = 15
        
        _WeightNormal ("Weight Normal", float) = 0
        _WeightBold ("Weight Bold", float) = 0.5
        
        _PerspectiveFilter ("Perspective Correction", Range(0, 1)) = 0.875
        _Sharpness ("Sharpness", Range(-1,1)) = 0
    }
    
    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull [_CullMode]
        ZWrite Off
        Lighting Off
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex VertShader
            #pragma fragment PixShader
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            // Define any missing functions from TMPro_Properties.cginc
            float2 UnpackUV(float uv)
            { 
                float2 output;
                output.x = floor(uv / 4096);
                output.y = uv - 4096 * output.x;
                return output * 0.001953125; // 1/512
            }
            
            struct vertex_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            
            struct pixel_t
            {
                float4 vertex : SV_POSITION;
                fixed4 faceColor : COLOR;
                fixed4 outlineColor : COLOR1;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 param : TEXCOORD2;    // weight, scale, bias
                float4 mask : TEXCOORD3;     // position in object space
                float2 scrollUV : TEXCOORD4; // scroll texture coords
            };
            
            // Properties
            sampler2D _MainTex;
            fixed4 _FaceColor;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _FaceDilate;
            float _OutlineSoftness;
            float _GradientScale;
            float _ScaleRatioA;
            float _VertexOffsetX;
            float _VertexOffsetY;
            float4 _ClipRect;
            float _MaskSoftnessX;
            float _MaskSoftnessY;
            float _WeightNormal;
            float _WeightBold;
            float _ScaleX;
            float _ScaleY;
            float _PerspectiveFilter;
            float _Sharpness;
            
            // Scroll properties
            sampler2D _ScrollTexture;
            float4 _ScrollTexture_ST;
            float _ScrollSpeed;
            float4 _ScrollDirection;
            
            pixel_t VertShader(vertex_t input)
            {
                float bold = step(input.texcoord1.y, 0);
                
                float4 vert = input.vertex;
                vert.x += _VertexOffsetX;
                vert.y += _VertexOffsetY;
                float4 vPosition = UnityObjectToClipPos(vert);
                
                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                
                float scale = rsqrt(dot(pixelSize, pixelSize));
                scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
                if(UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));
                
                float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
                weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;
                
                // Generate UV for the Masking Texture
                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                
                // Support for texture tiling and offset
                float2 textureUV = float2(input.texcoord0.x, input.texcoord0.y);
                float2 scrollUV = TRANSFORM_TEX(textureUV, _ScrollTexture);
                
                pixel_t output;
                output.vertex = vPosition;
                output.faceColor = _FaceColor;
                output.outlineColor = _OutlineColor;
                output.texcoord0 = textureUV;
                output.texcoord1 = float2(scale, weight);
                output.param = float4(weight, scale, 0.5, 0.5);
                output.mask = float4(input.vertex.xy, maskUV);
                output.scrollUV = scrollUV;
                
                return output;
            }
            
            fixed4 PixShader(pixel_t input) : SV_Target
            {
                // Read the font texture sample
                float c = tex2D(_MainTex, input.texcoord0).a;
                
                // Calculate soft mask
                float sd = saturate((c - 0.5) * input.param.y + 0.5);
                
                // Get the scrolled UV
                float2 scrolledUV = input.scrollUV;
                scrolledUV += _Time.y * _ScrollSpeed * _ScrollDirection.xy;
                
                // Handle clipping
                float2 maskUV = input.mask.zw;
                float2 edgeAdjust = float2(_MaskSoftnessX, _MaskSoftnessY) * 0.5 * input.param.y;
                float2 position = input.mask.xy;
                float2 edgeMin = _ClipRect.xy - edgeAdjust;
                float2 edgeMax = _ClipRect.zw + edgeAdjust;
                float2 softMask = float2(1, 1) - saturate(float2(
                    abs(position.x - (edgeMin.x + edgeMax.x) * 0.5) - (edgeMax.x - edgeMin.x) * 0.5,
                    abs(position.y - (edgeMin.y + edgeMax.y) * 0.5) - (edgeMax.y - edgeMin.y) * 0.5
                ) / edgeAdjust);
                
                float mask = softMask.x * softMask.y;
                
                // Sample the scroll texture at the scrolled UV coordinates
                float4 scrollTexture = tex2D(_ScrollTexture, scrolledUV);
                
                // Combine face color with scroll texture, masked by the character shape
                float4 faceColor = float4(scrollTexture.rgb, sd * scrollTexture.a * mask);
                
                return faceColor;
            }
            ENDCG
        }
    }
    
    Fallback "TextMeshPro/Mobile/Distance Field"
}