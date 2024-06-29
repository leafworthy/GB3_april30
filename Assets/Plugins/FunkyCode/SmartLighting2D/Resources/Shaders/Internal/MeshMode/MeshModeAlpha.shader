
Shader "Light2D/Internal/MeshModeAlpha"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Sprite ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend One OneMinusSrcAlpha

        Cull Off Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color    : COLOR;
            };

            fixed4 _Color;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color    : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _Sprite;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                color.r = 1 - color.r;
                color.g = color.r;
                color.b = color.r;

                color *= i.color * i.color.a;
                color *= tex2D(_Sprite, i.uv);

                color.a *= (color.r + color.b + color.g) / 3;
               

               // color.rgb *= i.color.a;
                    
                return color;
            }
            ENDCG
        }
    }
}
