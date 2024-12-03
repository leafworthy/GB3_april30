using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IHaveInspectorColor), true), CanEditMultipleObjects]
public class IHaveInspectorEditor : Editor
{
	private static Texture2D custom_icon;
	public override void OnInspectorGUI()
	{
		// Update the serialized object's representation
		serializedObject.Update();

		var component = (IHaveInspectorColor) target;
		// Load a custom icon
		
			custom_icon = (Texture2D) AssetDatabase.LoadAssetAtPath(component.GetIconPath(), typeof(Texture2D));
			if (custom_icon != null)
			{
				EditorGUIUtility.SetIconForObject(target, custom_icon);
			}
		 


		// Set the icon for the script
	
		// Iterate through all visible properties
		SerializedProperty property = serializedObject.GetIterator();
		bool expanded = true;
		while (property.NextVisible(expanded))
		{
			expanded = false;

			// Reserve space for the property including its children
			Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(property, true));

			if (property.name == "m_Script")
			{
				// Get the background color from the component
				Color backgroundColor = component.GetBackgroundColor();

				// Draw the colored background for the "Script" line
				EditorGUI.DrawRect(propertyRect, backgroundColor);

				// Save the default content color, then set it to the background color
				var originalColor = GUI.contentColor;
				GUI.contentColor = backgroundColor;

				// Draw the "Script" field (preserves the clickable link)
				EditorGUI.PropertyField(propertyRect, property, new GUIContent(property.displayName));

				// Restore the original content color
				GUI.contentColor = originalColor;
			}
			else
			{
				// Draw other properties as usual
				EditorGUI.PropertyField(propertyRect, property, true);
			}
		}

		// Apply any modified properties
		serializedObject.ApplyModifiedProperties();
	}
}