using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Camera;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.Camera;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.Light2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Buffers;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.Presets;
using UnityEngine;
using Object = __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Event_Handling.Object;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night
{
	[ExecuteInEditMode]
	public class Light2D : LightingMonoBehaviour
	{
		public enum LightType
		{
			Point,
			Sprite,
			FreeForm,
		}

		public enum LightSprite {Default, Custom};
		public enum LitMode {Everything, MaskOnly}
		public enum WhenInsideCollider {Draw, DontDraw, Ignore}
		public enum Rotation {Disabled, World, Local}
		public enum MaskTranslucencyQuality {Disabled, LowQuality, MediumQuality, HighQuality}

		public LightType lightType = LightType.Sprite;

		// settings
		public int lightPresetId = 0;
		public int eventPresetId = 0;

		// light layer
		public int lightLayer = 0;
		public int occlusionLayer = 0;
		public int translucentLayer = 0;

		public int translucentPresetId = 0;

		public Color color = new Color(.5f, .5f, .5f, 1);

		public float size = 5f;

		public float spotAngleInner = 360;
		public float spotAngleOuter = 360;

		// soft shadow
		public float coreSize = 0.5f;
		public float falloff = 0;

		public float lightStrength = 0;

		// legacy shadow
		public float outerAngle = 15;

		public float lightRadius = 1;

		public float shadowDistanceClose = 0;
		public float shadowDistanceFar = 5;

		public MaskTranslucencyQuality maskTranslucencyQuality = MaskTranslucencyQuality.LowQuality;
		public float maskTranslucencyStrength = 0;

		public Rotation applyRotation = Rotation.Disabled;

		public LightingSourceTextureSize textureSize = LightingSourceTextureSize.px2048;

		public MeshMode meshMode = new MeshMode();
		public BumpMap bumpMap = new BumpMap();

		public WhenInsideCollider whenInsideCollider = WhenInsideCollider.Draw;

		public LightSprite lightSprite = LightSprite.Default;
		public Sprite sprite;
		public bool spriteFlipX = false;
		public bool spriteFlipY = false;

		public LightTransform transform2D;

		public LightFreeForm freeForm;
		public float freeFormFalloff = 1;
		public float freeFormPoint = 1;
		public float freeFormFalloffStrength = 1;
		public FreeFormPoints freeFormPoints = new FreeFormPoints();

		public LightEventHandling eventHandling = new LightEventHandling();

		[System.Serializable]
		public class LightEventHandling
		{
			public Object eventHandlingObject = new Object();
		}

		// Internal
		private List<LightCollider2D> collidersInside = new List<LightCollider2D>();
		private List<LightCollider2D> collidersInsideRemove = new List<LightCollider2D>();

		public static List<Light2D> List = new List<Light2D>();
		private bool inScreen = false;
		public bool drawingEnabled = false;
		public bool drawingTranslucencyEnabled = false;
		private LightBuffer2D buffer = null;
		private static Sprite defaultSprite = null;

		public LightBuffer2D Buffer
		{
			get => buffer;
			set => buffer = value;
		}

		public void AddCollider(LightCollider2D id)
		{
			if (collidersInside.Contains(id))
			{
				if (lightPresetId > 0)
				{
					id.lightOnEnter?.Invoke(this);
				}

				collidersInside.Add(id);
			}
		}

		[System.Serializable]
		public class BumpMap
		{
			public float intensity = 1;
			public float depth = 1;
		}
				public LayerSetting[] GetLightPresetLayers()
		{

		LightPresetList presetList = Lighting2D.Profile.lightPresets;

			if (lightPresetId >= presetList.list.Length)
			{
				return(null);
			}

			LightPreset lightPreset = presetList.Get()[lightPresetId];

			return(lightPreset.layerSetting.Get());
		}

		public LayerSetting[] GetTranslucencyPresetLayers()
		{
			LightPresetList presetList = Lighting2D.Profile.lightPresets;

			if (translucentPresetId >= presetList.list.Length)
			{
				return(null);
			}

			LightPreset lightPreset = presetList.Get()[translucentPresetId];

			return(lightPreset.layerSetting.Get());
		}

		public EventPreset GetEventPreset()
		{
			EventPresetList presetList = Lighting2D.Profile.eventPresets;

			if (eventPresetId >= presetList.list.Length)
			{
				return(null);
			}

			EventPreset lightPreset = presetList.Get()[eventPresetId];

			return(lightPreset);
		}

		static public Sprite GetDefaultSprite()
		{
			if (defaultSprite == null || defaultSprite.texture == null)
			{
				defaultSprite = Resources.Load <Sprite> ("Sprites/gfx_light");
			}

			return(defaultSprite);
		}

		public Sprite GetSprite()
		{
			if (sprite == null || sprite.texture == null)
			{
				sprite = GetDefaultSprite();
			}
			return(sprite);
		}

		public void ForceUpdate()
		{
			if (transform2D == null)
			{
				return;
			}

			transform2D.ForceUpdate();

			freeForm.ForceUpdate();
		}

		static public void ForceUpdateAll()
		{
			foreach(Light2D light in Light2D.List)
			{
				light.ForceUpdate();
			}
		}

		public void OnEnable()
		{
			List.Add(this);

			if (transform2D == null)
			{
				transform2D = new LightTransform();
			}

			if (freeForm == null)
			{
				freeForm = new LightFreeForm();
			}

			LightingManager2D.Get();

			collidersInside.Clear();

			ForceUpdate();
		}

		public void OnDisable()
		{
			List.Remove(this);

			Free();
		}

		public void Free()
		{
			Scripts.Rendering.Buffers.Manager.FreeBuffer(buffer);

			inScreen = false;
		}

		public bool InCameras()
		{
			LightingManager2D manager = LightingManager2D.Get();
			LightingCameras lightingCameras = manager.cameras;

			Rect lightRect = transform2D.WorldRect;

			for(int i = 0; i < lightingCameras.Length; i++)
			{
				Camera camera = manager.GetCamera(i);

				if (camera == null)
				{
					continue;
				}

				Rect cameraRect = CameraTransform.GetWorldRect(camera);

				if (cameraRect.Overlaps(lightRect))
				{
					return(true);
				}
			}

			return(false);
		}

		// light 2D should know what layers id's it is supposed to draw? (include in array)

		public bool IfDrawLightCollider(LightCollider2D lightCollider)
		{
			LayerSetting[] layerSetting = GetLightPresetLayers();

			if (layerSetting == null)
			{
				return(false);
			}

			for(int i = 0; i < layerSetting.Length; i++)
			{
				if (layerSetting[i] == null)
				{
					continue;
				}

				int layerID = layerSetting[i].layerID;

				switch(layerSetting[i].type)
				{
					case LightLayerType.ShadowAndMask:

						if (layerID == lightCollider.shadowLayer || layerID == lightCollider.maskLayer)
						{
							return(true);
						}

					break;

					case LightLayerType.MaskOnly:

						if (layerID == lightCollider.maskLayer)
						{
							return(true);
						}

					break;

					case LightLayerType.ShadowOnly:

						if (layerID == lightCollider.shadowLayer)
						{
							return(true);
						}

					break;
				}
			}

			return(false);
		}

		public Vector2Int GetTextureSize()
		{
			Vector2Int textureSize2D = LightingRender2D.GetTextureSize(textureSize);

			if (Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSourceTextureSize.Custom)
			{
				textureSize2D = LightingRender2D.GetTextureSize(Lighting2D.Profile.qualitySettings.lightTextureSize);
			}

			return(textureSize2D);
		}

		public bool IsPixelPerfect()
		{
			if (Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSourceTextureSize.Custom)
			{
				return(Lighting2D.Profile.qualitySettings.lightTextureSize == LightingSourceTextureSize.PixelPerfect);
			}

			return (textureSize == LightingSourceTextureSize.PixelPerfect);
		}

		public LightBuffer2D GetBuffer()
		{
			if (buffer == null)
			{
				buffer = Scripts.Rendering.Buffers.Manager.PullBuffer (this);
			}

			return(buffer);
		}

		public void UpdateLoop()
		{
			transform2D.Update(this);

			if (lightType == LightType.FreeForm)
			{
				freeForm.Update(this);
			}

			// if camera moves & pixel perfect
			if (IsPixelPerfect())
			{
				transform2D.ForceUpdate();
			}

			UpdateBuffer();

			DrawMeshMode();

			if (eventPresetId > 0)
			{
				EventPreset eventPreset = GetEventPreset();

				if (eventPreset != null)
				{
					eventHandling.eventHandlingObject.Update(this, eventPreset);
				}
			}
		}

		void BufferUpdate()
		{
			transform2D.ClearUpdate();

			if (Lighting2D.disable)
			{
				return;
			}

			if (buffer == null)
			{
				return;
			}

			buffer.updateNeeded = true;
		}

		void UpdateCollidersInside()
		{
			foreach(LightCollider2D collider in collidersInside)
			{
				if (collider == null)
				{
					collidersInsideRemove.Add(collider);
					continue;
				}

				if (!collider.isActiveAndEnabled)
				{
					collidersInsideRemove.Add(collider);
					continue;
				}

				if (!collider.InLight(this))
				{
					collidersInsideRemove.Add(collider);
				}
			}

			foreach(LightCollider2D collider in collidersInsideRemove)
			{
				collidersInside.Remove(collider);

				transform2D.ForceUpdate();

				if (eventPresetId > 0)
				{
					if (collider != null)
					{
						if (collider.lightOnExit != null)
						{
							collider.lightOnExit.Invoke(this);
						}
					}
				}
			}

			collidersInsideRemove.Clear();
		}

		void UpdateBuffer()
		{
			UpdateCollidersInside();

			if (InCameras())
			{
				if (GetBuffer() == null)
				{
					return;
				}

				if (transform2D.UpdateNeeded || !inScreen)
				{
					BufferUpdate();

					inScreen = true;
				}
			}
				else
			{
				if (buffer != null)
				{
					Scripts.Rendering.Buffers.Manager.FreeBuffer(buffer);
				}

				inScreen = false;
			}
		}

		public void DrawMeshMode()
		{
			if (!meshMode.enable)
			{
				return;
			}

			if (buffer == null)
			{
				return;
			}

			if (!isActiveAndEnabled)
			{
				return;
			}

			if (!InCameras())
			{
				return;
			}

			LightingMeshRenderer lightingMesh = MeshRendererManager.Pull(this);

			if (lightingMesh != null)
			{
				lightingMesh.UpdateLight(this, meshMode);
			}
		}

		void OnDrawGizmosSelected()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected)
			{
				return;
			}

			Draw();
		}

		private void OnDrawGizmos()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos == EditorDrawGizmos.Disabled)
			{
				return;
			}

			if (Lighting2D.ProjectSettings.editorView.drawIcons == EditorIcons.Enabled)
			{
				Gizmos.DrawIcon(transform.position, "light_v2", true);
			}

			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always)
			{
				return;
			}

			Draw();
		}

		void Draw()
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			Gizmos.color = new Color(1f, 0.5f, 0.25f);

			if (applyRotation != Rotation.Disabled)
			{
				GizmosHelper.DrawCircle(transform.position, transform2D.rotation, 360, size); // spotAngle
			}
				else
			{
				GizmosHelper.DrawCircle(transform.position, 0, 360, size); // spotAngle
			}

			Gizmos.color = new Color(0, 1f, 1f);

			switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds)
			{
				case EditorGizmosBounds.Enabled:

					GizmosHelper.DrawRect(transform.position, transform2D.WorldRect);

				break;
			}
		}
	}
}
