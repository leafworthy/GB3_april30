
#if UNITY_EDITOR

using __SCRIPTS._OBJECT;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS
{
	[CustomEditor(typeof(LootOpenInteraction))]
	public class LootDropEditor : Editor
	{
		public void OnSceneGUI()
		{
			LootOpenInteraction myTarget = (LootOpenInteraction) target;

			myTarget.dropPosition = Handles.FreeMoveHandle(myTarget.transform.position + myTarget.dropPosition,
				0.2f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero, Handles.CircleHandleCap) - myTarget.transform.position;

			if (!GUI.changed) return;
			Undo.RecordObject(target, "Updated Drop Position");
			EditorUtility.SetDirty(target);
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			LootOpenInteraction myScript = (LootOpenInteraction) target;

		}
	}
}
#endif