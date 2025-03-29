using __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Day;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.SuperTilemapEditor.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Day
{
	[CustomEditor(typeof(DayLightTilemapCollider2D))]
	public class DayLightTilemapCollider2DEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI()
		{
			DayLightTilemapCollider2D script = target as DayLightTilemapCollider2D;

			script.tilemapType = (MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.tilemapType);

			EditorGUILayout.Space();

			switch(script.tilemapType)
			{
				case MapType.UnityRectangle:

					script.rectangle.shadowType = (ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.rectangle.shadowType);

					EditorGUI.BeginDisabledGroup(script.rectangle.shadowType == ShadowType.None);

					script.shadowLayer = EditorGUILayout.Popup("Shadow Layer (Day)", script.shadowLayer, Lighting2D.Profile.layers.colliderLayers.GetNames());

					switch(script.rectangle.shadowType)
					{
						case ShadowType.Grid:
						case ShadowType.SpritePhysicsShape:
							script.shadowTileType = (ShadowTileType)EditorGUILayout.EnumPopup("Shadow Tile Type", script.shadowTileType);
						break;
					}

					script.height = EditorGUILayout.FloatField("Shadow Distance", script.height);

					if (script.height < 0.1f) {
						script.height = 0.1f;
					}

					script.shadowSoftness = EditorGUILayout.FloatField("Shadow Softness", script.shadowSoftness);

					if (script.shadowSoftness < 0f) {
						script.shadowSoftness = 0f;
					}

					script.shadowTranslucency = EditorGUILayout.Slider("Shadow Translucency", script.shadowTranslucency, 0, 1);

					EditorGUI.EndDisabledGroup();

					EditorGUILayout.Space();

					script.rectangle.maskType = (MaskType)EditorGUILayout.EnumPopup("Mask Type", script.rectangle.maskType);

					script.maskLit = (DayLightTilemapCollider2D.MaskLit)EditorGUILayout.EnumPopup("Mask Lit", script.maskLit);


					EditorGUI.BeginDisabledGroup(script.rectangle.maskType == MaskType.None);

					//if (script.rectangle.maskType == LightTilemapCollider.Rectangle.MaskType.BumpedSprite) {
					//	GUIBumpMapMode.Draw(script.bumpMapMode);
					//}

					EditorGUI.EndDisabledGroup();

				break;

				case MapType.UnityIsometric:
				break;


				case MapType.UnityHexagon:
				break;

				case MapType.SuperTilemapEditor:
					script.superTilemapEditor.shadowTypeSTE = (TilemapCollider2D.ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.superTilemapEditor.shadowTypeSTE);

					script.shadowLayer = EditorGUILayout.Popup("Shadow Layer (Day)", script.shadowLayer, Lighting2D.Profile.layers.colliderLayers.GetNames());

					EditorGUILayout.Space();

					script.superTilemapEditor.maskTypeSTE = (TilemapCollider2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.superTilemapEditor.maskTypeSTE);

					EditorGUI.BeginDisabledGroup(script.superTilemapEditor.maskTypeSTE == TilemapCollider2D.MaskType.None);

					script.maskLayer = EditorGUILayout.Popup("Mask Layer (Day)", script.maskLayer, Lighting2D.Profile.layers.colliderLayers.GetNames());

					if (script.superTilemapEditor.maskTypeSTE == TilemapCollider2D.MaskType.BumpedSprite)
					{
						GUIBumpMapMode.Draw(serializedObject, script);
					}

					EditorGUI.EndDisabledGroup();
				break;
			}

			EditorGUILayout.Space();

			UpdateCollisions(script);

			if (GUI.changed)
			{
				script.Initialize();

				if (!EditorApplication.isPlaying)
				{
					EditorUtility.SetDirty(target);
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
		}

		static void UpdateCollisions(DayLightTilemapCollider2D script)
		{
			if (GUILayout.Button("Update"))
			{
				// PhysicsShapeManager.Clear();

				script.Initialize();

				//LightingSource2D.ForceUpdateAll();
				LightingManager2D.ForceUpdate();
			}
		}
	}
}
