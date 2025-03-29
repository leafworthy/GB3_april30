using __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Occlusion;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Occlusion
	{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LightTilemapOcclusion2D))]
	public class LightTilemap2DEditor : UnityEditor.Editor {

		override public void OnInspectorGUI() {
			LightTilemapOcclusion2D script = target as LightTilemapOcclusion2D;

			script.tilemapType = (LightTilemapOcclusion2D.MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.tilemapType);

			script.onlyColliders = EditorGUILayout.Toggle("Only Colliders", script.onlyColliders);

			GUISortingLayer.Draw(script.sortingLayer, false);
			
			if (GUILayout.Button("Update")) {
				script.Initialize();
			}

			if (GUI.changed) {
				script.Initialize();

				if (EditorApplication.isPlaying == false) {
					EditorUtility.SetDirty(target);
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
		}
	}
}
