using UnityEditor;
using UnityEngine;

public class BatchRenameTool : EditorWindow
{
	private string prefix = "NewName_";
	private int startIndex = 1;

	[MenuItem("Tools/Batch Rename")]
	public static void ShowWindow()
	{
		GetWindow<BatchRenameTool>("Batch Rename");
	}

	void OnGUI()
	{
		GUILayout.Label("Batch Rename Settings", EditorStyles.boldLabel);

		prefix = EditorGUILayout.TextField("Prefix:", prefix);
		startIndex = EditorGUILayout.IntField("Start Index:", startIndex);

		if (GUILayout.Button("Rename Selected GameObjects"))
		{
			RenameSelectedGameObjects();
		}
	}

	void RenameSelectedGameObjects()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		for (int i = 0; i < selectedObjects.Length; i++)
		{
			selectedObjects[i].name = prefix + (startIndex + i);
		}
	}
}
