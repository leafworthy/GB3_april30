Shader "Light2D/Internal/Light/FreeFormOcclusion"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Point ("Free Form Point", Float) = 0
    }

    Category
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }
        
        Blend One OneMinusSrcAlpha
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

                float _Point;
                
                struct appdata_t
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                v2f vert (appdata_t v)
                {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.texcoord);

                    fixed4 col = tex * 2.0f;
                    col.a = (1 - tex.a);

                    col.r *= i.color.a * 2;

                    float x = i.texcoord.x - 0.5;
                    float y = i.texcoord.y - 0.5;

                    float distance = lerp(1, 1 - sqrt(x * x + y * y) * 2, _Point);

                    col.r *= distance;

                    col.g = col.r;
                    col.b = col.r;    

                    col.rgb *= i.color;
                
                    return col;
                }
                ENDCG
            }
        }
    }
}