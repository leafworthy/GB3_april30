#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
    [CustomEditor(typeof(IsoSpriteSorting))]
    public class IsoSpriteSortingEditor : UnityEditor.Editor
    {
        private static Texture2D customIcon;

        private void OnEnable()
        {
            // Load the custom icon from the Assets folder
            if (customIcon == null)
            {
                customIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Eye_Icon.png");
            }

            // Set the icon for the target object
            if (customIcon != null)
            {
                EditorGUIUtility.SetIconForObject(target, customIcon);
            }
        }

        public void OnSceneGUI()
        {
            IsoSpriteSorting myTarget = (IsoSpriteSorting)target;

            var fmh_31_13_638687467658385480 = Quaternion.identity; myTarget.SorterPositionOffset = Handles.FreeMoveHandle(
                myTarget.transform.position + myTarget.SorterPositionOffset,
                0.08f * HandleUtility.GetHandleSize(myTarget.transform.position),
                Vector3.zero,
                Handles.DotHandleCap
            ) - myTarget.transform.position;

            if (myTarget.sortType == IsoSpriteSorting.SortType.Line)
            {
                var fmh_41_17_638687467658394920 = Quaternion.identity; myTarget.SorterPositionOffset2 = Handles.FreeMoveHandle(
                    myTarget.transform.position + myTarget.SorterPositionOffset2,
                    0.08f * HandleUtility.GetHandleSize(myTarget.transform.position),
                    Vector3.zero,
                    Handles.DotHandleCap
                ) - myTarget.transform.position;

                Handles.DrawLine(myTarget.transform.position + myTarget.SorterPositionOffset, myTarget.transform.position + myTarget.SorterPositionOffset2);
            }

            if (GUI.changed)
            {
                Undo.RecordObject(target, "Updated Sorting Offset");
                EditorUtility.SetDirty(target);
            }
        }

        public override void OnInspectorGUI()
        {
            // Draw the default Inspector
            base.OnInspectorGUI();

            // Add custom buttons below the Inspector fields
            IsoSpriteSorting myScript = (IsoSpriteSorting)target;

            if (GUILayout.Button("Sort Visible Scene"))
            {
                // IsoSpriteSorting.SortScene();
            }

            if (GUILayout.Button("Get Renderers"))
            {
                // myScript.GetRenderers();
            }
        }
    }
}
#endif