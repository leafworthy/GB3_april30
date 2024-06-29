using FunkyCode.SmartLighting2D.Scripts.Camera;
using FunkyCode.SmartLighting2D.Scripts.Rendering.Buffers;
using FunkyCode.SmartLighting2D.Scripts.Settings;
using FunkyCode.SmartLighting2D.Scripts.Settings.Presets;

namespace FunkyCode.SmartLighting2D.Scripts.Rendering
{
    public static class Main
	{
        public static void InternalUpdate()
        {
            UpdateMaterials();

			UpdateMainBuffers();
        }

        public static void Render()
		{
			if (Lighting2D.disable)
			{
				return;
			}

            LightingCameras cameras = LightingManager2D.Get().cameras;

			if (cameras.Length < 1)
			{
				return;
			}

			UpdateLoop();
			
			if (LightBuffer2D.List.Count > 0)
			{
				foreach(LightBuffer2D buffer in LightBuffer2D.List)
				{
					buffer.Render();
				}
			}
			
			if (LightMainBuffer2D.List.Count > 0)
			{
				foreach(LightMainBuffer2D buffer in LightMainBuffer2D.List)
				{
					buffer.Render();
				}
			}
		}

        private static void UpdateLoop()
		{
			// colliders

			if (DayLightCollider2D.List.Count > 0)
			{
				for(int id = 0; id < DayLightCollider2D.List.Count; id++)
				{
					DayLightCollider2D.List[id].UpdateLoop();
				}
			}
			
			if (LightCollider2D.List.Count > 0)
			{
				for(int id = 0; id < LightCollider2D.List.Count; id++)
				{
					LightCollider2D.List[id].UpdateLoop();
				}
			}

			// lights

			if (LightSprite2D.List.Count > 0)
			{
				for(int id = 0; id < LightSprite2D.List.Count; id++)
				{
					LightSprite2D.List[id].UpdateLoop();
				}
			}
			
			if (Light2D.List.Count > 0)
			{
				for(int id = 0; id < Light2D.List.Count; id++)
				{
					Light2D.List[id].UpdateLoop();
				}
			}

			// mesh renderers

			if (OnRenderMode.List.Count > 0)
			{
				for(int id = 0; id < OnRenderMode.List.Count; id++)
				{
					OnRenderMode.List[id].UpdateLoop();
				}
			}
		}

        public static void UpdateMainBuffers()
		{
			// should reset materials
			LightmapMaterials.ResetShaders();

			LightmapMaterials.SetDayLight();

            LightingCameras cameras = LightingManager2D.Get().cameras;

			for(int i = 0; i < cameras.Length; i++)
			{
				CameraSettings cameraSetting = cameras.Get(i);

				bool isSceneView = cameraSetting.cameraType == CameraSettings.CameraType.SceneView;

				for(int b = 0; b < cameraSetting.Lightmaps.Length; b++)
				{
					CameraLightmap cameraLightmap = cameraSetting.GetLightmap(b);

					if (cameraLightmap.presetId >= Lighting2D.LightmapPresets.Length)
					{
						continue;
					}

					LightmapPreset lightmapPreset = Lighting2D.LightmapPresets[cameraLightmap.presetId];
	
					LightMainBuffer2D buffer = LightMainBuffer2D.Get(cameraSetting, cameraLightmap, lightmapPreset);

					if (buffer == null)
					{
						continue;
					}
						
					buffer.cameraLightmap.rendering = cameraLightmap.rendering;

					buffer.cameraLightmap.overlay = cameraLightmap.overlay;

					buffer.cameraLightmap.renderLayerId = cameraLightmap.renderLayerId;

					if (buffer.cameraLightmap.customMaterial != cameraLightmap.customMaterial)
					{
						buffer.cameraLightmap.customMaterial = cameraLightmap.customMaterial;

						buffer.ClearMaterial();
					}

					if (buffer.cameraLightmap.overlayMaterial != cameraLightmap.overlayMaterial)
					{
						buffer.cameraLightmap.overlayMaterial = cameraLightmap.overlayMaterial;

						buffer.ClearMaterial();
					}

					if (cameraLightmap.rendering == CameraLightmap.Rendering.Disabled)
					{
						continue;
					}

					UnityEngine.Camera camera = cameraSetting.GetCamera();

					switch(cameraLightmap.output)
					{
						case CameraLightmap.Output.Materials:

							foreach(UnityEngine.Material material in cameraLightmap.GetMaterials().materials)
							{
								if (material == null)
								{
									continue;
								}

								// adding up // Get Free LightMap

								// Get Free ID from material

								// + is scene view
							
								LightmapMaterials.SetMaterial(1, material, camera, buffer.renderTexture);			
							}

						break;

						case CameraLightmap.Output.Shaders:

							// Get Free ID from shaders

							LightmapMaterials.SetShaders(isSceneView, 1, camera, buffer.renderTexture);

						break;

						case CameraLightmap.Output.Pass1:

							LightmapMaterials.SetShaders(isSceneView, 1, camera, buffer.renderTexture);

						break;

						case CameraLightmap.Output.Pass2:

							LightmapMaterials.SetShaders(isSceneView, 2, camera, buffer.renderTexture);

						break;

						case CameraLightmap.Output.Pass3:

							LightmapMaterials.SetShaders(isSceneView, 3, camera, buffer.renderTexture);

						break;

						case CameraLightmap.Output.Pass4:

							LightmapMaterials.SetShaders(isSceneView, 4, camera, buffer.renderTexture);

						break;
					}
				}
			}

			// Update Main Buffers

			if (LightMainBuffer2D.List.Count > 0)
			{
				for(int i = 0; i < LightMainBuffer2D.List.Count; i++)
				{
					LightMainBuffer2D buffer = LightMainBuffer2D.List[i];

					if (buffer != null)
					{
						buffer.Update();
					}
				}

				foreach(LightMainBuffer2D buffer in LightMainBuffer2D.List)
				{
					if (Lighting2D.disable)
					{
						buffer.updateNeeded = false;	

						return;
					}

					CameraSettings cameraSettings = buffer.cameraSettings;
					CameraLightmap cameraBufferPreset = buffer.cameraLightmap;
					
					bool render = cameraBufferPreset.rendering != CameraLightmap.Rendering.Disabled;

					if (render && cameraSettings.GetCamera() != null)
					{
						buffer.updateNeeded = true;
					}
						else
					{
						buffer.updateNeeded = false;
					}
				}
			}
		}
		
		public static void UpdateMaterials()
		{
			if (Lighting2D.materials.Initialize(Lighting2D.QualitySettings.HDR))
			{
				LightMainBuffer2D.Clear();
				LightBuffer2D.Clear();

				Light2D.ForceUpdateAll();
			}
		}
    }
}
