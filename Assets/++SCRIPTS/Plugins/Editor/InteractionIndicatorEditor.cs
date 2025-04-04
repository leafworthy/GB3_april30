#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor
{
	[CustomEditor(typeof(InteractionIndicator))]
	public class InteractionIndicatorEditor : UnityEditor.Editor
	{
		public void OnSceneGUI()
		{
			InteractionIndicator myTarget = (InteractionIndicator) target;

			myTarget.indicatorOffset = Handles.FreeMoveHandle(myTarget.transform.position + myTarget.indicatorOffset,
				0.08f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero, Handles.DotHandleCap) - myTarget.transform.position;

			myTarget.interactionPoint = Handles.FreeMoveHandle(myTarget.transform.position + myTarget.interactionPoint,
				0.08f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero, Handles.CircleHandleCap) - myTarget.transform.position;
		
			if (!GUI.changed) return;
			Undo.RecordObject(target, "Updated Sorting Offset");
			EditorUtility.SetDirty(target);
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			InteractionIndicator myScript = (InteractionIndicator) target;

		}
	}
}

#endif