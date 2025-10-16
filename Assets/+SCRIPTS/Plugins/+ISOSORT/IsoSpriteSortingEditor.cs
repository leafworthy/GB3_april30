#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins._ISOSORT
{
    [CustomEditor(typeof(IsoSpriteSorting))]
    public class IsoSpriteSortingEditor : UnityEditor.Editor
    {
        private static Texture2D customIcon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Eye_Icon.png");

        private void OnEnable()
        {
            if (customIcon != null)
            {
                EditorGUIUtility.SetIconForObject(target, customIcon);
            }
        }

        public void OnSceneGUI()
        {
            IsoSpriteSorting myTarget = (IsoSpriteSorting) target;

            // Get the local-to-world matrix to account for scaling
            Matrix4x4 localToWorld = myTarget.transform.localToWorldMatrix;

            // Calculate the world positions considering scaling
            Vector3 worldPos1 = localToWorld.MultiplyPoint3x4(myTarget.SorterPositionOffset);

            // First handle position
            Vector3 newPos = Handles.FreeMoveHandle(worldPos1, 0.08f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero,
                Handles.DotHandleCap);

            // Convert back to local offset
            myTarget.SorterPositionOffset = myTarget.transform.worldToLocalMatrix.MultiplyPoint3x4(newPos);

            if (myTarget.sortType == IsoSpriteSorting.SortType.Line)
            {
                // Calculate the second world position considering scaling
                Vector3 worldPos2 = localToWorld.MultiplyPoint3x4(myTarget.SorterPositionOffset2);

                // Second handle position
                Vector3 newPos2 = Handles.FreeMoveHandle(worldPos2, 0.08f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero,
                    Handles.DotHandleCap);

                // Convert back to local offset
                myTarget.SorterPositionOffset2 = myTarget.transform.worldToLocalMatrix.MultiplyPoint3x4(newPos2);

                // Draw line with the scaled positions
                Handles.DrawLine(worldPos1, worldPos2);
            }

            if (!GUI.changed) return;
            Undo.RecordObject(target, "Updated Sorting Offset");
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            // Draw the default Inspector
            base.OnInspectorGUI();

            // Add custom buttons below the Inspector fields
            IsoSpriteSorting myScript = (IsoSpriteSorting)target;

            if (GUILayout.Button("Sort Visible Scene"))
            {
                 IsoSpriteSorting.UpdateSorters();
            }

            if (GUILayout.Button("Get Renderersu"))
            {
	            myScript.hasRenderers = false;
                 myScript.GetRenderers();
            }
        }
    }
}
#endif
