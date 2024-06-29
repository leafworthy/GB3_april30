Shader "Light2D/Internal/MeshModeAdditive"
{
    Properties
    {
        _MainTex ("Lightmap Texture", 2D) = "white" {}
        _Sprite ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.5, 0.5, 0.5, 0.5)
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

        Blend SrcAlpha One
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
                #pragma target 2.0

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                sampler2D _Sprite;
                fixed4 _Color;
                
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

                float4 _MainTex_ST;

                v2f vert (appdata_t v)
                {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
            
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    tex.r = 1 - tex.r;

                    tex.r *= _Color.a;
                    tex.g = tex.r;
                    tex.b = tex.r;

                    fixed4 col;
                    col.rgb = _Color.rgb * tex.rgb * i.color.rgb;

                    col *= tex2D(_Sprite, i.texcoord);
                    col.a = tex.a;
                
                    return col;
                }
                
                ENDCG
            }
        }
    }
}