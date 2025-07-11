using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightTilemap2D.Types;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.SpriteExtension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Night
{
	[CustomEditor(typeof(LightTilemapRoom2D))]
	public class LightTilemapRoom2DEditor : UnityEditor.Editor
	{
		override public void OnInspectorGUI()
		{
			LightTilemapRoom2D script = target as LightTilemapRoom2D;

			script.lightLayer = EditorGUILayout.Popup("Layer (Light)", script.lightLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

			EditorGUILayout.Space();

			script.mapType = (MapType)EditorGUILayout.EnumPopup("Map Type", script.mapType);

			EditorGUILayout.Space();

			script.maskType = (LightTilemapRoom2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.maskType);

			EditorGUILayout.Space();

			script.shaderType = (LightTilemapRoom2D.ShaderType)EditorGUILayout.EnumPopup("Shader Type", script.shaderType);

			script.color = EditorGUILayout.ColorField("Shader Color", script.color);

			EditorGUILayout.Space();

			if (GUILayout.Button("Update"))
			{
				PhysicsShapeManager.Clear();

				script.Initialize();
				LightingManager2D.ForceUpdate();
			}

			if (GUI.changed) {
				// script.Initialize();

				LightingManager2D.ForceUpdate();

				if (!EditorApplication.isPlaying)
				{
					EditorUtility.SetDirty(script);
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				}
			}
		}
	}
}
