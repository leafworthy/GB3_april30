using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Camera;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.Camera;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Buffers;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager
{
	[ExecuteInEditMode]
	public class LightingManager2D : LightingMonoBehaviour
	{
		private static LightingManager2D instance;

		[SerializeField] public LightingCameras cameras = new();

		public int version = 0;
		public string version_string = "";

		public Profile setProfile;
		public Profile profile;

		// editor foldouts (avoid reseting after compiling script)
		public bool[] foldout_cameras = new bool[10];

		public bool[,] foldout_lightmapPresets = new bool[10, 10];
		public bool[,] foldout_lightmapMaterials = new bool[10, 10];

		// Sets Lighting Main Profile Settings for Lighting2D at the start of the scene
		private static bool initialized = false;

		public Camera GetCamera(int id)
		{
			if (cameras.Length <= id) return null;

			return cameras.Get(id).GetCamera();
		}

		public static void ForceUpdate()
		{
		}

		public static LightingManager2D Get()
		{
			if (instance != null)
			{
				/*if (!instance.isActiveAndEnabled)
				{
					instance.gameObject.SetActive(true);
				}*/

				return instance;
			}

			foreach (var manager in Resources.FindObjectsOfTypeAll<LightingManager2D>())
			{
				instance = manager;

				return instance;
			}

			// create new light manager
			var gameObject = new GameObject("Remade Lighting Manager 2D");

			instance = gameObject.AddComponent<LightingManager2D>();

			instance.transform.position = Vector3.zero;

			instance.version = Lighting2D.VERSION;

			instance.version_string = Lighting2D.VERSION_STRING;

			return instance;
		}


		public void Awake()
		{
			if (cameras == null) cameras = new LightingCameras();

			if (instance != null && instance != this)
			{
				switch (Lighting2D.ProjectSettings.managerInstance)
				{
					case ManagerInstance.Static:
					case ManagerInstance.DontDestroyOnLoad:

						Debug.LogWarning(
							"Smart Lighting2D: Lighting Manager duplicate was found, new instance destroyed.",
							gameObject);

						foreach (var manager in FindObjectsByType<LightingManager2D>(FindObjectsSortMode.None))
							if (manager != instance)
								manager.DestroySelf();

						return; // Cancel Initialization

					case ManagerInstance.Dynamic:

						instance = this;

						Debug.LogWarning(
							"Smart Lighting2D: Lighting Manager duplicate was found, old instance destroyed.",
							gameObject);

						foreach (var manager in FindObjectsByType<LightingManager2D>(FindObjectsSortMode.None))
							if (manager != instance)
								manager.DestroySelf();

						break;
				}
			}

			initialized = false;

			SetupProfile();

			if (Application.isPlaying)
			{
				if (Lighting2D.ProjectSettings.managerInstance == ManagerInstance.DontDestroyOnLoad)
					if (instance != null)
						DontDestroyOnLoad(instance.gameObject);
			}
		}

		private void Update()
		{
			if (Lighting2D.disable) return;

			ForceUpdate(); // for late update method?

			if (profile != null)
			{
				if (Lighting2D.Profile != profile) Lighting2D.UpdateByProfile(profile);
			}
		}

		private void LateUpdate()
		{
			if (Lighting2D.disable) return;

			UpdateInternal();

			if (Lighting2D.Profile.qualitySettings.updateMethod == UpdateMethod.LateUpdate) Main.Render();
		}

		public void SetupProfile()
		{
			if (initialized) return;

			initialized = true;

			profile = Lighting2D.Profile;

			Lighting2D.UpdateByProfile(profile);

			Lighting2D.materials.Reset();
		}

		public void UpdateInternal()
		{
			if (Lighting2D.disable) return;

			for (var id = 0; id < CameraTransform.List.Count; id++) CameraTransform.List[id].Update();

			SetupProfile();

			Main.InternalUpdate();
		}

		public bool IsSceneView() // overlay
		{
			for (var i = 0; i < cameras.Length; i++)
			{
				var cameraSetting = cameras.Get(i);

				if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView)
				{
					for (var b = 0; b < cameraSetting.Lightmaps.Length; b++)
					{
						var bufferPreset = cameraSetting.GetLightmap(b);

						if (bufferPreset.overlay == CameraLightmap.Overlay.Enabled) return true;
					}
				}
			}

			return false;
		}

		private void OnDisable()
		{
			if (profile != null)
			{
				if (Application.isPlaying)
				{
					if (setProfile != profile)
					{
						if (Lighting2D.Profile == profile) Lighting2D.RemoveProfile();
					}
				}
			}

#if UNITY_EDITOR

#if UNITY_2019_1_OR_NEWER

			SceneView.beforeSceneGui -= OnSceneView;
			//SceneView.duringSceneGui -= OnSceneView;

#else
					SceneView.onSceneGUIDelegate -= OnSceneView;

#endif

#endif
		}

		public void UpdateProfile()
		{
			if (setProfile == null) setProfile = Lighting2D.ProjectSettings.Profile;

			if (Application.isPlaying)
				profile = Instantiate(setProfile);
			else
				profile = setProfile;
		}

		private void OnEnable()
		{
			foreach (OnRenderMode onRenderMode in FindObjectsByType< OnRenderMode>(FindObjectsSortMode.None)) onRenderMode.DestroySelf();
//CHANGED findobjectsbytype
			SmartLighting2D.Scripts.Scriptable.LightSprite2D.List.Clear();

			UpdateProfile();
			Main.UpdateMaterials();

			for (var i = 0; i < cameras.Length; i++)
			{
				var cameraSetting = cameras.Get(i);

				for (var b = 0; b < cameraSetting.Lightmaps.Length; b++)
				{
					var bufferPreset = cameraSetting.GetLightmap(b);

					foreach (var material in bufferPreset.GetMaterials().materials)
					{
						if (material == null) continue;

						var camera = cameraSetting.GetCamera();

						if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView)
							LightmapMaterials.ClearMaterial(material);
					}
				}
			}

			Update();
			LateUpdate();

#if UNITY_EDITOR
#if UNITY_2019_1_OR_NEWER
			SceneView.beforeSceneGui += OnSceneView;
			//SceneView.duringSceneGui += OnSceneView;
#else
					SceneView.onSceneGUIDelegate += OnSceneView;
#endif
#endif
		}

		private void OnRenderObject()
		{
			if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) return;

			foreach (var buffer in LightMainBuffer2D.List) LightMainBuffer.DrawPost(buffer);
		}

		private void OnDrawGizmos()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always) return;

			DrawGizmos();
		}

		private void DrawGizmos()
		{
			if (!isActiveAndEnabled) return;

			Gizmos.color = new Color(0, 1f, 1f);

			if (Lighting2D.ProjectSettings.editorView.drawGizmosBounds == EditorGizmosBounds.Enabled)
			{
				for (var i = 0; i < cameras.Length; i++)
				{
					var cameraSetting = cameras.Get(i);

					var camera = cameraSetting.GetCamera();

					if (camera != null)
					{
						var cameraRect = CameraTransform.GetWorldRect(camera);

						GizmosHelper.DrawRect(transform.position, cameraRect);
					}
				}
			}

			for (var i = 0; i < SmartLighting2D.Scripts.Scriptable.LightSprite2D.List.Count; i++)
			{
				var light = SmartLighting2D.Scripts.Scriptable.LightSprite2D.List[i];

				var rect = light.lightSpriteShape.GetWorldRect();

				Gizmos.color = new Color(1f, 0.5f, 0.25f);

				GizmosHelper.DrawPolygon(light.lightSpriteShape.GetSpriteWorldPolygon(), transform.position);

				Gizmos.color = new Color(0, 1f, 1f);
				GizmosHelper.DrawRect(transform.position, rect);
			}
		}

#if UNITY_EDITOR
		private static void OnSceneView(SceneView sceneView)
		{
			var manager = Get();

			if (!manager.IsSceneView()) return;

			if (Application.isPlaying) return;

			ForceUpdate();

			Main.Render();
		}
#endif
	}
}
