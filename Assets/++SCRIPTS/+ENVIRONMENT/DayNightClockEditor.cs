#if UNITY_EDITOR
// Custom Inspector
using UnityEditor;
using UnityEngine;

namespace GangstaBean.Environment
{

    public class DayNightClockEditor : UnityEditor.Editor
    {
        private SerializedProperty timeTextProp;
        private SerializedProperty daytimeRadialImageProp;
        private SerializedProperty nighttimeRadialImageProp;
        private SerializedProperty outlineImageProp;
        private SerializedProperty backgroundImageProp;
        private SerializedProperty dayIconProp;
        private SerializedProperty nightIconProp;
        private SerializedProperty use24HourFormatProp;
        private SerializedProperty showAmPmProp;
        private SerializedProperty showDayProp;
        private SerializedProperty daytimeTextColorProp;
        private SerializedProperty nighttimeTextColorProp;
        private SerializedProperty daytimeOutlineColorProp;
        private SerializedProperty nighttimeOutlineColorProp;
        private SerializedProperty daytimeBackgroundColorProp;
        private SerializedProperty nighttimeBackgroundColorProp;
        private SerializedProperty daytimeStartProp;
        private SerializedProperty nighttimeStartProp;
        private SerializedProperty currentDayProp;

        private bool showTimeSettings = true;
        private bool showVisualSettings = true;

        private void OnEnable()
        {
            timeTextProp = serializedObject.FindProperty("timeText");
            daytimeRadialImageProp = serializedObject.FindProperty("daytimeRadialImage");
            nighttimeRadialImageProp = serializedObject.FindProperty("nighttimeRadialImage");
            outlineImageProp = serializedObject.FindProperty("outlineImage");
            backgroundImageProp = serializedObject.FindProperty("backgroundImage");
            dayIconProp = serializedObject.FindProperty("dayIcon");
            nightIconProp = serializedObject.FindProperty("nightIcon");
            use24HourFormatProp = serializedObject.FindProperty("use24HourFormat");
            showAmPmProp = serializedObject.FindProperty("showAmPm");
            showDayProp = serializedObject.FindProperty("showDay");
            daytimeTextColorProp = serializedObject.FindProperty("daytimeTextColor");
            nighttimeTextColorProp = serializedObject.FindProperty("nighttimeTextColor");
            daytimeOutlineColorProp = serializedObject.FindProperty("daytimeOutlineColor");
            nighttimeOutlineColorProp = serializedObject.FindProperty("nighttimeOutlineColor");
            daytimeBackgroundColorProp = serializedObject.FindProperty("daytimeBackgroundColor");
            nighttimeBackgroundColorProp = serializedObject.FindProperty("nighttimeBackgroundColor");
            daytimeStartProp = serializedObject.FindProperty("daytimeStart");
            nighttimeStartProp = serializedObject.FindProperty("nighttimeStart");
            currentDayProp = serializedObject.FindProperty("currentDay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // UI References Section
            EditorGUILayout.LabelField("UI References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(timeTextProp);
            EditorGUILayout.PropertyField(daytimeRadialImageProp);
            EditorGUILayout.PropertyField(nighttimeRadialImageProp);
            EditorGUILayout.PropertyField(outlineImageProp);
            EditorGUILayout.PropertyField(backgroundImageProp);
            EditorGUILayout.PropertyField(dayIconProp);
            EditorGUILayout.PropertyField(nightIconProp);

            EditorGUILayout.Space();

            // Time Format Settings
            showTimeSettings = EditorGUILayout.Foldout(showTimeSettings, "Time Format Settings", true);
            if (showTimeSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(use24HourFormatProp);

                // Only show AM/PM setting if using 12-hour format
                if (!use24HourFormatProp.boolValue)
                {
                    EditorGUILayout.PropertyField(showAmPmProp);
                }

                EditorGUILayout.PropertyField(showDayProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Visual Settings
            showVisualSettings = EditorGUILayout.Foldout(showVisualSettings, "Visual Settings", true);
            if (showVisualSettings)
            {
                EditorGUI.indentLevel++;

                // Day/Night threshold
                EditorGUILayout.LabelField("Day/Night Thresholds");
                EditorGUILayout.PropertyField(daytimeStartProp, new GUIContent("Daytime Start"));
                EditorGUILayout.PropertyField(nighttimeStartProp, new GUIContent("Nighttime Start"));
                EditorGUILayout.PropertyField(currentDayProp, new GUIContent("Current Day"));

                // Convert to human-readable time for reference
                float daytimeHours = daytimeStartProp.floatValue * 24f;
                int daytimeHoursInt = Mathf.FloorToInt(daytimeHours);
                int daytimeMinutes = Mathf.FloorToInt((daytimeHours - daytimeHoursInt) * 60f);

                float nighttimeHours = nighttimeStartProp.floatValue * 24f;
                int nighttimeHoursInt = Mathf.FloorToInt(nighttimeHours);
                int nighttimeMinutes = Mathf.FloorToInt((nighttimeHours - nighttimeHoursInt) * 60f);

                EditorGUILayout.HelpBox(
                    string.Format("Daytime starts at {0:D2}:{1:D2}\nNighttime starts at {2:D2}:{3:D2}",
                        daytimeHoursInt, daytimeMinutes, nighttimeHoursInt, nighttimeMinutes),
                    MessageType.Info
                );

                EditorGUILayout.Space();

                // Colors
                EditorGUILayout.LabelField("Colors");
                EditorGUILayout.PropertyField(daytimeTextColorProp);
                EditorGUILayout.PropertyField(nighttimeTextColorProp);
                EditorGUILayout.PropertyField(daytimeOutlineColorProp);
                EditorGUILayout.PropertyField(nighttimeOutlineColorProp);
                EditorGUILayout.PropertyField(daytimeBackgroundColorProp);
                EditorGUILayout.PropertyField(nighttimeBackgroundColorProp);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
