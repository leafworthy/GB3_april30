#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS
{
	[CustomEditor(typeof(DayNightCycle))]
	public class DayNightCycleEditor : Editor
	{
		private DayNightCycle cycle;
		private SerializedProperty targetLightProp;
		private SerializedProperty cycleDurationProp;
		private SerializedProperty timeOfDayProp;
		private SerializedProperty timeMultiplierProp;
		private SerializedProperty pauseCycleProp;
		private SerializedProperty intensityCurveProp;
		private SerializedProperty intensityMultiplierProp;
		private SerializedProperty colorGradientProp;
		private SerializedProperty showDebugInfoProp;

		private void OnEnable()
		{
			cycle = (DayNightCycle) target;
			targetLightProp = serializedObject.FindProperty("targetLight");
			cycleDurationProp = serializedObject.FindProperty("cycleDuration");
			timeOfDayProp = serializedObject.FindProperty("timeOfDay");
			timeMultiplierProp = serializedObject.FindProperty("timeMultiplier");
			pauseCycleProp = serializedObject.FindProperty("pauseCycle");
			intensityCurveProp = serializedObject.FindProperty("intensityCurve");
			intensityMultiplierProp = serializedObject.FindProperty("intensityMultiplier");
			colorGradientProp = serializedObject.FindProperty("colorGradient");
			showDebugInfoProp = serializedObject.FindProperty("showDebugInfo");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.Space();

			// Time controls
			EditorGUILayout.LabelField("Time Controls", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();

			// Time control buttons
			if (GUILayout.Button("Skip to Morning"))
				cycle.SkipToMorning();

			if (GUILayout.Button("Skip to Noon"))
				cycle.SkipToNoon();

			if (GUILayout.Button("Skip to Sunset"))
				cycle.SkipToSunset();

			if (GUILayout.Button("Skip to Midnight"))
				cycle.SkipToMidnight();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			// Pause/Resume button
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(cycle.pauseCycle ? "Resume Cycle" : "Pause Cycle"))
			{
				cycle.SetPaused(!cycle.pauseCycle);
				EditorUtility.SetDirty(target);
			}

			// Time speed controls
			if (GUILayout.Button("0.5x Speed"))
				cycle.SetTimeMultiplier(0.5f);

			if (GUILayout.Button("1x Speed"))
				cycle.SetTimeMultiplier(1.0f);

			if (GUILayout.Button("2x Speed"))
				cycle.SetTimeMultiplier(2.0f);

			if (GUILayout.Button("5x Speed"))
				cycle.SetTimeMultiplier(5.0f);

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			// Draw the default inspector excluding the custom controls
			DrawPropertiesExcluding(serializedObject);

			serializedObject.ApplyModifiedProperties();

			// Update in editor mode
			if (!Application.isPlaying && cycle.targetLight != null)
			{
				cycle.UpdateLight();
				EditorUtility.SetDirty(cycle.targetLight);
			}
		}
	}
}
#endif