using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Buffers
{
	[ExecuteInEditMode]
	public class LightBuffer2D
	{
		public string name = "unknown";

		private Light2D light;

		public Light2D Light
		{
			get => light;

			set
			{
				light = value;

				LightBuffer.UpdateName(this);
			}
		}

		public bool Free => light == null;

		public LightTexture renderTexture;

		public LightTexture translucencyTexture;
		public LightTexture translucencyTextureBlur;

		public LightTexture freeFormTexture;

		public bool updateNeeded = false;

		public static List<LightBuffer2D> List = new List<LightBuffer2D>();

		public LightBuffer2D()
		{
			List.Add(this);
		}

		public static void Clear()
		{
			foreach(LightBuffer2D buffer in new List<LightBuffer2D>(List))
			{
				if (buffer.light)
				{
					buffer.light.Buffer = null;
				}

				buffer.DestroySelf();
			}

			List.Clear();
		}

		public void DestroySelf()
		{
			List.Remove(this);

			if (renderTexture == null)
			{
				return;
			}

			if (renderTexture.renderTexture == null)
			{
				return;
			}
				
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy (renderTexture.renderTexture);
			}
				else
			{
				UnityEngine.Object.DestroyImmediate (renderTexture.renderTexture);
			}
		}

		public void Initiate(Vector2Int textureSize)
		{
			LightBuffer.InitializeRenderTexture(this, textureSize);
		}

		public void Render()
		{
			if (renderTexture.renderTexture == null)
			{
				return;
			}

			if (!updateNeeded)
			{
				return;
			}
			
			updateNeeded = false;

			if (light == null)
			{
				return;
			}

			if (light.translucentLayer > 0)
			{
				if (translucencyTexture == null)
				{
					Vector2Int effectTextureSize = LightingRender2D.GetTextureSize(Lighting2D.Profile.qualitySettings.lightEffectTextureSize);
					
					LightBuffer.InitializeTranslucencyTexture(this, effectTextureSize);
				}
				
				if (translucencyTexture != null)
				{
					RenderTexture previous2 = RenderTexture.active;

					RenderTexture.active = translucencyTexture.renderTexture;

					GL.Clear(false, true, Color.black);
			
					LightBuffer.RenderTranslucency(light);

					RenderTexture.active = previous2;

					// blur texture blit

					float time = Time.realtimeSinceStartup;

					UnityEngine.Material material;

					float textureSize = (float)translucencyTextureBlur.renderTexture.width / 128;

					float strength = light.maskTranslucencyStrength * textureSize * 10;

					switch(light.maskTranslucencyQuality)
					{
						case Light2D.MaskTranslucencyQuality.HighQuality:

							// vertical pass

							material = Lighting2D.materials.GetMaskBlurVertical();

							material.SetFloat("_Strength", strength);

							Graphics.Blit(translucencyTexture.renderTexture, translucencyTextureBlur.renderTexture, material);

							material.SetFloat("_Strength", strength * 0.5f);

							Graphics.Blit(translucencyTextureBlur.renderTexture, translucencyTexture.renderTexture, material);

							material.SetFloat("_Strength", strength * 0.25f);

							Graphics.Blit(translucencyTexture.renderTexture, translucencyTextureBlur.renderTexture, material);
					
							// horizontal pass

							material = Lighting2D.materials.GetMaskBlurHorizontal();

							material.SetFloat("_Strength", strength);

							Graphics.Blit(translucencyTextureBlur.renderTexture, translucencyTexture.renderTexture, material);

							material.SetFloat("_Strength", strength * 0.5f);

							Graphics.Blit(translucencyTexture.renderTexture, translucencyTextureBlur.renderTexture, material);

							material.SetFloat("_Strength", strength * 0.25f);

							Graphics.Blit(translucencyTextureBlur.renderTexture, translucencyTexture.renderTexture, material);		

						break;

						case Light2D.MaskTranslucencyQuality.MediumQuality:

							// vertical pass

							material = Lighting2D.materials.GetMaskBlurVertical();

							material.SetFloat("_Strength", strength);

							Graphics.Blit(translucencyTexture.renderTexture, translucencyTextureBlur.renderTexture, material);

							material.SetFloat("_Strength", strength * 0.5f);

							Graphics.Blit(translucencyTextureBlur.renderTexture, translucencyTexture.renderTexture, material);
					
							// horizontal pass

							material = Lighting2D.materials.GetMaskBlurHorizontal();

							material.SetFloat("_Strength", strength);

							Graphics.Blit(translucencyTexture.renderTexture, translucencyTextureBlur.renderTexture, material);

							material.SetFloat("_Strength", strength * 0.5f);

							Graphics.Blit(translucencyTextureBlur.renderTexture, translucencyTexture.renderTexture, material);

						break;


						case Light2D.MaskTranslucencyQuality.LowQuality:

							// vertical
		
							material = Lighting2D.materials.GetMaskBlurVertical();

							material.SetFloat("_Strength", strength);

							Graphics.Blit(translucencyTexture.renderTexture, translucencyTextureBlur.renderTexture, material);

							// horizontal

							material = Lighting2D.materials.GetMaskBlurHorizontal();

							material.SetFloat("_Strength", strength);

							Graphics.Blit(translucencyTextureBlur.renderTexture, translucencyTexture.renderTexture, material);

						break;
					}
				}
			}

			if (light.lightType == Light2D.LightType.FreeForm)
			{
				if (freeFormTexture == null)
				{
					LightBuffer.InitializeFreeFormTexture(this, new Vector2Int(renderTexture.width, renderTexture.height));
				}

				// render only if there are free form changes

				if (freeFormTexture != null)
				{
					if (light.freeForm.UpdateNeeded)
					{
						light.freeForm.UpdateNeeded = false;

						RenderTexture previous3 = RenderTexture.active;

						RenderTexture.active = freeFormTexture.renderTexture;

						GL.Clear(false, true, Color.black);
				
						LightBuffer.RenderFreeForm(light);

						RenderTexture.active = previous3;
					}
				}
			}

			if (light.lightLayer >= 0)
			{
				RenderTexture previous = RenderTexture.active;

				RenderTexture.active = renderTexture.renderTexture;

				GL.Clear(false, true, Color.black);

				LightBuffer.Render(light);

				RenderTexture.active = previous;
			}
		}
	}
}
