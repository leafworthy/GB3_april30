﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Night
	{
	[CustomEditor(typeof(LightRoom2D))]
	public class LightRoom2DEditor : UnityEditor.Editor {
		override public void OnInspectorGUI() {
			LightRoom2D script = target as LightRoom2D;

			script.lightLayer = EditorGUILayout.Popup("Layer (Light)", script.lightLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

			script.shape.type = (LightRoom2D.RoomType)EditorGUILayout.EnumPopup("Room Type", script.shape.type);

			script.color = EditorGUILayout.ColorField("Color", script.color);

			Update();

			if (GUI.changed) {
				if (EditorApplication.isPlaying == false) {
					EditorUtility.SetDirty(script);
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

					LightingManager2D.ForceUpdate();
				}
			}
		}

		void Update() {
			LightRoom2D script = target as LightRoom2D;

			if (GUILayout.Button("Update")) {
				script.Initialize();
			}
		}
	}
}
