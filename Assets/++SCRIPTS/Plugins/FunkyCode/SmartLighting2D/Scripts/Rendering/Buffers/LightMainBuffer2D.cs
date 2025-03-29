using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Camera;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.Presets;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Buffers
{
	[ExecuteInEditMode] 
	public class LightMainBuffer2D
	{
		public enum Type
		{
			RGB24,
			R8,
			RHalf,
			Depth8
		}

		public string name = "Uknown";

		private UnityEngine.Material material = null;

		public bool updateNeeded = false;

		public Type type;

		public LightTexture renderTexture;
		public CameraSettings cameraSettings;
		public CameraLightmap cameraLightmap;

		public static List<LightMainBuffer2D> List = new List<LightMainBuffer2D>();

		public LightMainBuffer2D(Type type, CameraSettings cameraSettings, CameraLightmap cameraLightmap)
		{
			this.type = type;

			this.cameraLightmap = cameraLightmap;
			this.cameraSettings = cameraSettings;

			List.Add(this);
		}

		public static void Clear()
		{
			foreach(LightMainBuffer2D buffer in new List<LightMainBuffer2D>(List))
			{
				buffer.DestroySelf();
			}

			List.Clear();
		}

		public void DestroySelf()
		{
			if (renderTexture != null)
			{
				if (renderTexture.renderTexture != null)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy (renderTexture.renderTexture);
					}
						else
					{
						UnityEngine.Object.DestroyImmediate (renderTexture.renderTexture);
					}
				}
			}

			List.Remove(this);
		}

		public bool IsActive()
		{
			return(List.IndexOf(this) > -1);
		}

		static public LightMainBuffer2D Get(CameraSettings cameraSettings, CameraLightmap lightmap, LightmapPreset lightmapPreset)
		{
			Type type = (Type)lightmapPreset.type;

			if (cameraSettings.GetCamera() == null)
			{
				return(null);
			}

			foreach(LightMainBuffer2D mainBuffer in List)
			{
				if (mainBuffer.type == type &&  mainBuffer.cameraSettings.GetCamera() == cameraSettings.GetCamera() && mainBuffer.cameraLightmap.presetId == lightmap.presetId)
				{
					return(mainBuffer);
				}
			}

			if (Lighting2D.LightmapPresets.Length <= lightmap.presetId)
			{
				UnityEngine.Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

				return(null);
			}

			LightMainBuffer2D buffer = new LightMainBuffer2D(type, cameraSettings, lightmap);

			LightMainBuffer.InitializeRenderTexture(buffer);

			return(buffer);
		}

		public LightmapPreset GetLightmapPreset()
		{
			if (Lighting2D.LightmapPresets.Length <= cameraLightmap.presetId)
			{
				UnityEngine.Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

				return(null);
			}

			return(Lighting2D.LightmapPresets[cameraLightmap.presetId]);
		}

		public void ClearMaterial()
		{
			material = null;
		}

		public UnityEngine.Material GetMaterial()
		{
			if (material == null)
			{
				switch(cameraLightmap.overlayMaterial)
				{
					case CameraLightmap.OverlayMaterial.Multiply:
					
						material = new UnityEngine.Material(Shader.Find("Light2D/Internal/Multiply"));
						
					break;

					case CameraLightmap.OverlayMaterial.Additive:
						
						material = new UnityEngine.Material(Shader.Find("Legacy Shaders/Particles/Additive")); // use light 2D shader?
						
					break;

					case CameraLightmap.OverlayMaterial.Custom:

						material = new UnityEngine.Material(cameraLightmap.GetMaterial());

					break;

					case CameraLightmap.OverlayMaterial.Reference:

						material = cameraLightmap.customMaterial;

					break;
				}
			}
			
			if (material != null)
			{
				if (renderTexture != null)
				{
					material.mainTexture = renderTexture.renderTexture;
				}
					else
				{
					UnityEngine.Debug.LogWarning("render texture null");
				}
			}
			
			return(material);
		}

		public void Update()
		{
			LightMainBuffer.Update(this);
		}

		public void Render()
		{
			if (cameraLightmap.rendering == CameraLightmap.Rendering.Disabled)
			{
				return;
			}

			if (updateNeeded)
			{
				UnityEngine.Camera camera = UnityEngine.Camera.current;

				if (camera != null)
				{
					// return;	
				}
			
				RenderTexture previous = RenderTexture.active;

				if (renderTexture != null)
				{
					RenderTexture.active = renderTexture.renderTexture;

					LightMainBuffer.Render(this);
				}
					else
				{
					UnityEngine.Debug.LogWarning("null render texture in buffer " + cameraSettings.id + " " + cameraLightmap.presetId);
				}

				RenderTexture.active = previous;
			}

			LightMainBuffer.DrawOn(this);
		}

		// apply render to specified camera (post render mode)
		public void OnRenderObject()
		{
			if (Lighting2D.disable)
			{
				return;
			}
		}
	}
}
