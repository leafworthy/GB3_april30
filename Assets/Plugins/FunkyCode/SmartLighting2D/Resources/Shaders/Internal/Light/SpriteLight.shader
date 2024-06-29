Shader "Light2D/Internal/Light/SpriteLight"
{
    Properties
    {
        _MainTex ("Lightmap Texture", 2D) = "white" {}
        _Sprite ("Sprite Texture", 2D) = "black" {}
        _Rotation ("Sprite Rotation", Float) = 0
        _LinearColor ("Linear Color", Float) = 0
        _Outer("Outer", Float) = 0
        _Inner("Inner", Float) = 0
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

        // Blend One One
        // Blend One OneMinusSrcAlpha
        // Blend OneMinusDstColor One // Soft additive
        Blend One One
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

                float _LinearColor;
                sampler2D _Sprite;
                float _Rotation;

                float _Outer;
                float _Inner;

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
                    float2 xy : TEXCOORD1;
                };

                v2f vert (appdata_t v)
                {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.xy.x = v.texcoord.x - 0.5;
                    o.xy.y = v.texcoord.y - 0.5;

                    return o;
                }

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

                fixed4 frag (v2f i) : SV_Target
                {
                    float lightmap = tex2D(_MainTex, i.texcoord).r;

                    float2 spriteUV = TransformToCamera(i.texcoord - float2(0.5, 0.5), _Rotation);

                    fixed4 sprite = tex2D(_Sprite, spriteUV + float2(0.5, 0.5));
                    sprite.rgb *= sprite.a;

                    float dir = ((atan2(i.xy.y, i.xy.x) - _Rotation) * 57.2958 + 450 + 360) % 360;

                    float pointValue = max(0, min(1, (_Inner * 0.5 - abs(dir - 180) + _Outer) / _Outer));
                    
                    fixed4 output = fixed4(1, 1, 1, 1);

                    output.rgb = (2 - lightmap * 2) * i.color.a * 2;
    
                    output *= sprite;

                    output.rgb *= pointValue;

                    output.rgb *= i.color;
   
                    return output;
                }
                ENDCG
            }
        }
    }
}