using UnityEngine;
using UnityEditor;

public class DistributeHorizontally : EditorWindow
{
	[MenuItem("Edit/Distribute Horizontally &h")]
	static void Distribute()
	{
		Transform[] selected = Selection.transforms;

		if (selected.Length < 3)
		{
			Debug.LogWarning("Select at least 3 objects to distribute");
			return;
		}

		// Record undo
		Undo.RecordObjects(selected, "Distribute Horizontally");

		// Sort by X position
		System.Array.Sort(selected, (a, b) => a.position.x.CompareTo(b.position.x));

		float leftmost = selected[0].position.x;
		float rightmost = selected[selected.Length - 1].position.x;
		float spacing = (rightmost - leftmost) / (selected.Length - 1);

		// Distribute objects
		for (int i = 1; i < selected.Length - 1; i++)
		{
			Vector3 newPos = selected[i].position;
			newPos.x = leftmost + (spacing * i);
			selected[i].position = newPos;
		}
	}
}
