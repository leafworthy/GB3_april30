
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(LootOpener))]
public class LootDropEditor : Editor
{
	public void OnSceneGUI()
	{
		LootOpener myTarget = (LootOpener) target;

		myTarget.dropPosition = Handles.FreeMoveHandle(myTarget.transform.position + myTarget.dropPosition,
			0.2f * HandleUtility.GetHandleSize(myTarget.transform.position), Vector3.zero, Handles.CircleHandleCap) - myTarget.transform.position;

		if (!GUI.changed) return;
		Undo.RecordObject(target, "Updated Drop Position");
		EditorUtility.SetDirty(target);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		LootOpener myScript = (LootOpener) target;

	}
}
#endif