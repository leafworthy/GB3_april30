﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.Camera;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightSprite2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Sprite_Mesh;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night
{
	[ExecuteInEditMode]
	public class LightSprite2D : MonoBehaviour
	{
		public enum Type {Light, Mask};
		public enum SpriteMode {Custom, SpriteRenderer};

		public int lightLayer = 0;

		public Type type = Type.Light;
		public SpriteMode spriteMode = SpriteMode.Custom;
		public Sprite sprite = null;

		public Color color = new Color(0.5f, 0.5f, 0.5f, 1f);
		
		public bool flipX = false;
		public bool flipY = false;

		public LightSpriteTransform lightSpriteTransform = new LightSpriteTransform();

		public LightSpriteShape lightSpriteShape = new LightSpriteShape();

		public MeshMode meshMode = new MeshMode();

		public GlowMode glowMode = new GlowMode();

		public VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

		public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

		public static List<LightSprite2D> List = new List<LightSprite2D>();

		private SpriteRenderer spriteRendererComponent;

		public void OnEnable()
		{
			List.Add(this);

			LightingManager2D.Get();
		}

		public void OnDisable()
		{
			List.Remove(this);
		}

		public bool InCamera(Camera camera) {
			Rect cameraRect = CameraTransform.GetWorldRect(camera);

			return(cameraRect.Overlaps(lightSpriteShape.GetWorldRect()));
		}

		private static Sprite defaultSprite = null;

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
			if (GetSpriteOrigin() == null)
			{
				return(null);
			}

			return(GetSpriteOrigin());		
		}

		public Sprite GetSpriteOrigin()
		{
			if (spriteMode == SpriteMode.Custom)
			{
				if (sprite == null)
				{
					sprite = GetDefaultSprite();
				}
				
				return(sprite);
			}
				else
			{
				if (GetSpriteRenderer() == null)
				{
					return(null);
				}

				sprite = spriteRendererComponent.sprite;

				return(sprite);
			}
		}

		public SpriteRenderer GetSpriteRenderer()
		{
			if (spriteRendererComponent == null)
			{
				spriteRendererComponent = GetComponent<SpriteRenderer>();
			}

			return(spriteRendererComponent);
		}

		public void UpdateLoop()
		{
			if (spriteMode == SpriteMode.SpriteRenderer)
			{
				SpriteRenderer sr = GetSpriteRenderer();

				if (sr != null)
				{
					spriteRenderer.flipX = sr.flipX;
					spriteRenderer.flipY = sr.flipY;		
				}
			}
				else
			{
				spriteRenderer.flipX = flipX;
				spriteRenderer.flipY = flipY;	
			}

			spriteRenderer.sprite = GetSprite();
			spriteRenderer.color = color;

			if (meshMode.enable)
			{
				DrawMesh();
			}

			lightSpriteShape.Set(spriteRenderer, transform, lightSpriteTransform);

			lightSpriteShape.Update();
		}

		public void DrawMesh()
		{
			if (!meshMode.enable)
			{
				return;
			}

			LightingMeshRenderer lightingMesh = MeshRendererManager.Pull(this);

			if (lightingMesh != null)
			{
				lightingMesh.UpdateLightSprite(this, meshMode);
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
			
			// Gizmos.DrawIcon(transform.position, "light", true);

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

			GizmosHelper.DrawPolygon(lightSpriteShape.GetSpriteWorldPolygon(), transform.position);

			Gizmos.color = new Color(0, 1f, 1f);

			switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds)
			{
				case EditorGizmosBounds.Enabled:
				
					GizmosHelper.DrawRect(transform.position, lightSpriteShape.GetWorldRect()); 

				break;
			}
		}
	}
}
