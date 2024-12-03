using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IHaveInspectorColor), true), CanEditMultipleObjects]
public class IconChanger : Editor
{
	private static Texture2D customIcon;

	public override void OnInspectorGUI()
	{
		// Load a custom icon
		if (customIcon == null)
		{
			customIcon = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Icon.png", typeof(Texture2D));
		}

		// Set the icon for the script
		if (customIcon != null)
		{
			EditorGUIUtility.SetIconForObject(target, customIcon);
		}

		// Draw the default inspector
		DrawDefaultInspector();
	}
}