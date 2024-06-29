Shader "Light2D/Internal/Day/SoftShadow"
{        
	Properties
	{
	}

	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		BlendOp Min

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			#include "UnityCG.cginc"
		
			struct vertice
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct pixel
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			pixel vert(vertice v)
			{
				pixel p;

				float4 vertex = v.vertex;
				vertex.z = 0;
		
				p.vertex = UnityObjectToClipPos(vertex);
				
				p.color = v.color;

				if (p.color.g > 0.1)
				{
					if (v.vertex.z > 2)
					{
						p.texcoord = float2(0, 1);
					}
						else if (v.vertex.z > 1)
					{
						p.texcoord = float2(1, 1);
					}
						else if (v.vertex.z > 0)
					{
						p.texcoord = float2(1, 0);
					}
						else
					{
						p.texcoord = float2(0, 0);
					}
				}
					else if (p.color.r > 0.1)
				{
					if (v.vertex.z > 0)
					{
						p.texcoord = float2(1, 0);
					}
						else
					{
						p.texcoord = float2(0, 0);
					}
				}
					else
				{
					p.texcoord = v.texcoord;
				}
		
				return p;
			}

			fixed4 frag(pixel p) : SV_Target
			{
				fixed4 color = float4(1, 1, 1, 1);

				float x = p.texcoord.x - 0.5;
				float y = p.texcoord.y - 0.5;

				if (p.color.b > 0.1)
				{
					color.rgb = float3(0, 0, 0);
				}

				if (p.color.r > 0.1)
				{
					color.rgb = p.texcoord.x;
				}

				if (p.color.g > 0.1)
				{
					color.rgb = sqrt(x * x + y * y) * 2;
				}

				color.rgb += p.color.a;

				return color;
			}

			ENDCG
		}
	}
}