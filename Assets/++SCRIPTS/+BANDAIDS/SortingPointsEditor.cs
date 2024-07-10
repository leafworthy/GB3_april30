#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SortingPoints))]
public class SortingPointsEditor : Editor
{
	public void OnSceneGUI()
	{
		SortingPoints myTarget = (SortingPoints) target;

		var fmh_27_4_638052632644429690 = Quaternion.identity; myTarget.openSortingPoint1 = Handles.FreeMoveHandle(
			myTarget.transform.position + myTarget.openSortingPoint1,
			0.08f * HandleUtility.GetHandleSize(myTarget.transform.position),
			Vector3.zero,
			Handles.DotHandleCap
		) - myTarget.transform.position;
		
		var fmh_35_5_638052632644453830 = Quaternion.identity; myTarget.openSortingPoint2 = Handles.FreeMoveHandle(
			myTarget.transform.position + myTarget.openSortingPoint2,
			0.08f * HandleUtility.GetHandleSize(myTarget.transform.position),
			Vector3.zero,
			Handles.DotHandleCap
		) - myTarget.transform.position;
		Handles.DrawLine(myTarget.transform.position + myTarget.openSortingPoint1,
			myTarget.transform.position + myTarget.openSortingPoint2);
		

		if (GUI.changed)
		{
			Undo.RecordObject(target, "Updated Sorting Offset");
			EditorUtility.SetDirty(target);
		}
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		SortingPoints myScript = (SortingPoints) target;
		
	}
}

#endif