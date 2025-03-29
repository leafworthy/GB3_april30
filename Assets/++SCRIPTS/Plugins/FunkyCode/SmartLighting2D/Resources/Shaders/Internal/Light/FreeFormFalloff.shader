Shader "Light2D/Internal/Light/FreeFormFalloff"
{
    Category
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        BlendOp max
        Cull Off
        Lighting Off
        ZWrite Off

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
        
                #include "UnityCG.cginc"

                sampler2D _MainTex;
            
                float4 _MainTex_ST;

                
                float _Strength;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                };

                v2f vert (appdata_t v)
                {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;

                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    float color = 0;

                    if (i.color.a > 0)
                    {
                        // point

                        float x = i.color.x - 0.5;
                        float y = i.color.y - 0.5;

                        color = 1 - sqrt(x * x + y * y) * 2;

                        float draw = step(0, color);

                        color = lerp(color, ((1 / cos(color)) - 1) * 1.175, _Strength);

                        color *= draw;
                    }
                        else
                    {
                        // edge

                        color = i.color.r;

                        color = lerp(color, ((1 / cos(color)) - 1) * 1.175, _Strength);
                    }
                    
                    return color;
                }
                ENDCG
            }
        }
    }
}