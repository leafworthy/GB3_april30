Shader "Light2D/Sprites/Fast/LitPass1&2"
{
	Properties
	{
		[HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Lit ("Lit", Range(0,1)) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"= "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {

		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#include "../../../LitShaders/FastLitCore.cginc"
			// #include "Assets/FunkyCode/SmartLighting2D/Resources/Shaders/LitShaders/LitCore.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color    : COLOR;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				fixed4 color    : COLOR;
                float2 worldPos : TEXCOORD1;
			};
			
			sampler2D _MainTex;
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
                
                OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 spritePixel = tex2D (_MainTex, IN.texcoord) * IN.color;

				fixed3 light = max( SL2D_FAST_PASS_LIT_1(IN.worldPos), SL2D_FAST_PASS_LIT_2(IN.worldPos));

				spritePixel.rgb *= light * spritePixel.a;

				return spritePixel;
			}

		    ENDCG
		}
	}
}