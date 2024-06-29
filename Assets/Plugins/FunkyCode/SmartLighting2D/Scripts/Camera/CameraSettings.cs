using FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace FunkyCode.SmartLighting2D.Scripts.Camera
{
	[System.Serializable]
	public struct CameraSettings
	{
		public enum CameraType
		{
			MainCamera,
			Custom,
			SceneView
		};

		public static int initCount = 0;

		public int id;

		[SerializeField]
		private CameraLightmap[] lightmaps;

		public CameraLightmap[] Lightmaps
		{
			get
			{
				if (lightmaps == null)
				{
					lightmaps = new CameraLightmap[1];

					LightingManager2D manager = LightingManager2D.Get();
					LightingCameras cameras = manager.cameras;

					cameras.Set(id, this);
				}

				return(lightmaps);
			}

			set => lightmaps = value;
		}

		public CameraLightmap GetLightmap(int index)
		{
			CameraLightmap buffer = lightmaps[index];
			buffer.id = index;
			return(buffer);
		}

		public CameraType cameraType;
		public UnityEngine.Camera customCamera;

		public string GetTypeName()
		{
			switch(cameraType)
			{
				case CameraType.SceneView:

					return("Scene View");

				case CameraType.MainCamera:

					return("Main Camera Tag");

				case CameraType.Custom:

					return("Custom");

				default:

					return("Unknown");
			}
		}

		public int GetLayerId(int bufferId)
		{
			CameraLightmap lightmap = GetLightmap(bufferId);

			if (lightmap.overlayLayerType == CameraLightmap.OverlayLayerType.UnityLayer)
			{
				return(lightmap.renderLayerId);
			}
				else
			{
				UnityEngine.Camera camera = GetCamera();

				if (camera != null && cameraType == CameraType.SceneView)
				{
					return(Lighting2D.ProjectSettings.editorView.sceneViewLayer);
				}
					else
				{
					return(Lighting2D.ProjectSettings.editorView.gameViewLayer);
				}
			}
		}

		public CameraSettings(int id)
		{
			this.id = id;

			cameraType = CameraType.MainCamera;

			customCamera = null;
			
			lightmaps = new CameraLightmap[1];

			lightmaps[0] = new CameraLightmap(0);

			initCount ++;
		}

		public UnityEngine.Camera GetCamera()
		{
			UnityEngine.Camera camera = null;

			switch(cameraType)
			{
				case CameraType.MainCamera:

					camera = UnityEngine.Camera.main;

					if (camera != null)
					{
						if (!camera.orthographic)
						{
							return(null);
						}
					}

					return(UnityEngine.Camera.main);

				case CameraType.Custom:

					camera = customCamera;

					if (camera != null)
					{
						if (!camera.orthographic)
						{
							return(null);
						}
					}

					return(customCamera);


				case CameraType.SceneView:
				
					#if UNITY_EDITOR

						SceneView sceneView = SceneView.lastActiveSceneView;

						if (sceneView != null)
						{
							camera = sceneView.camera;

							#if UNITY_2019_1_OR_NEWER
							
								if (!SceneView.lastActiveSceneView.sceneLighting)
								{
									camera = null;
								}

							#else
							
								if (!SceneView.lastActiveSceneView.m_SceneLighting)
								{
									camera = null;
								}

							#endif
						}
		
						if (camera != null && !camera.orthographic)
						{
							camera = null;
						}

						if (camera != null)
						{
							if (!camera.orthographic)
							{
								return(null);
							}
						}

						return(camera);

					#else
					
						return(null);

					#endif
					
			}

			return(null);
		}

		/*
		public bool Equals(CameraSettings obj) {
			return this.bufferID == obj.bufferID && this.customCamera == obj.customCamera && this.cameraType == obj.cameraType;
		}*/

		public override int GetHashCode()
		{
			return this.GetHashCode();
		}
	}
}
