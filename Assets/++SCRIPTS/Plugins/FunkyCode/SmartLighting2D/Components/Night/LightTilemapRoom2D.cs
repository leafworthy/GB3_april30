﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemapRoom2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.SuperTilemapEditor.Components;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night
{
	[ExecuteInEditMode]
	public class LightTilemapRoom2D : MonoBehaviour
	{
		public int lightLayer = 0;
		public enum MaskType {Sprite}  // Separate For Each Map Type!
		public enum ShaderType {ColorMask, MultiplyTexture};
	
		public MapType mapType = MapType.UnityRectangle;
		public MaskType maskType = MaskType.Sprite;
		public ShaderType shaderType = ShaderType.ColorMask;
		public Color color = Color.black;

		public TilemapRoom2D superTilemapEditor = new TilemapRoom2D();
		public Rectangle rectangle = new Rectangle();

		public LightingTilemapRoomTransform lightingTransform = new LightingTilemapRoomTransform();
	
		public static List<LightTilemapRoom2D> List = new List<LightTilemapRoom2D>();

		public void OnEnable()
		{
			List.Add(this);

			LightingManager2D.Get();

			rectangle.SetGameObject(gameObject);
			superTilemapEditor.SetGameObject(gameObject);

			Initialize();
		}

		public void OnDisable()
		{
			List.Remove(this);
		}

		public Base GetCurrentTilemap()
		{
			switch(mapType)
			{
				case MapType.SuperTilemapEditor:
					return(superTilemapEditor);
				case MapType.UnityRectangle:
					return(rectangle);
			}
			return(null);
		}

		public void Initialize()
		{
			TilemapEvents.Initialize();
			
			GetCurrentTilemap().Initialize();
		}

		public void Update()
		{
			lightingTransform.Update(this);

			if (lightingTransform.UpdateNeeded)
			{

				GetCurrentTilemap().ResetWorld();

				Light2D.ForceUpdateAll();
			}
		}

		public TilemapProperties GetTilemapProperties()
		{
			return(GetCurrentTilemap().Properties);
		}

		public List<LightTile> GetTileList()
		{
			return(GetCurrentTilemap().MapTiles);
		}

		public float GetRadius()
		{
			return(GetCurrentTilemap().GetRadius());
		}

		void OnDrawGizmosSelected()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected)
			{
				return;
			}

			DrawGizmos();
		}

		private void OnDrawGizmos()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always)
			{
				return;
			}

			DrawGizmos();
		}

		private void DrawGizmos()
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			// Gizmos.color = new Color(1f, 0.5f, 0.25f);

			Gizmos.color = new Color(0, 1f, 1f);

			switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds)
			{
				case EditorGizmosBounds.Enabled:

					GizmosHelper.DrawRect(transform.position, GetCurrentTilemap().GetRect());

				break;
			}
		}
	}
}
