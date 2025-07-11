using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.Presets;
using UnityEngine;
using Texture = __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects.Texture;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Day
{	
	public static class Main
	{
		static Pass pass = new Pass();

		public static void Draw(UnityEngine.Camera camera, LightmapPreset lightmapPreset)
		{
			if (!IsDrawing(camera, lightmapPreset))
			{
				return;
			}

			LightmapLayer[] layerSettings = lightmapPreset.dayLayers.Get();
		
			for(int i = 0; i < layerSettings.Length; i++)
			{
				LightmapLayer dayLayer = layerSettings[i];

				LayerSorting sorting = dayLayer.sorting;

				if (!pass.Setup(dayLayer, camera))
				{
					continue;
				}

				if (sorting == LayerSorting.None)
				{
					NoSort.Draw(pass);
				}
					else
				{
					pass.SortObjects();

					Sorted.Draw(pass);
				}
			}
			
			ShadowAlpha(camera);
		}

		public static bool IsDrawing(UnityEngine.Camera camera, LightmapPreset lightmapPreset)
		{
			if (Lighting2D.DayLightingSettings.alpha == 0) // <=
			{
				return(false);
			}

			if (lightmapPreset == null)
			{
				return(false);
			}

			LightmapLayer[] layerSettings = lightmapPreset.dayLayers.Get();

			if (layerSettings.Length < 1)
			{
				return(false);
			}

			return(true);
		}

		private static void ShadowAlpha(UnityEngine.Camera camera)
		{
			Color color = new Color(0, 0, 0,  (1f - Lighting2D.DayLightingSettings.alpha));

			if (color.a > 0)
			{
				color.r = 1f;
				color.g = 1f;
				color.b = 1f;
					
				UnityEngine.Material material = Lighting2D.materials.GetAlphaBlend();
				material.mainTexture = null;		
								
				GLExtended.color = color;

				Texture.Quad.Draw(material, Vector2.zero, LightingRender2D.GetSize(camera), camera.transform.eulerAngles.z, 0);
			}
		}
	}
}
