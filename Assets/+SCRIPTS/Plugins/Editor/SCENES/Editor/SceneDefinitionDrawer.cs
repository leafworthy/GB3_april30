#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor.SCENES.Editor
{
    /// <summary>
    /// Property drawer for SceneDefinition references
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneDefinition))]
    public class SceneDefinitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            // Draw the object field for the SceneDefinition reference
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label, true);
        
            // If a SceneDefinition is assigned, show a preview of its key properties
            if (property.objectReferenceValue != null)
            {
                // Get the SceneDefinition object
                SceneDefinition sceneDefinition = property.objectReferenceValue as SceneDefinition;
                if (sceneDefinition != null)
                {
                    position.y += EditorGUIUtility.singleLineHeight + 2;
                    position.height = EditorGUIUtility.singleLineHeight;
                
                    // Show scene name and display name as read-only info
                    EditorGUI.indentLevel++;
                    GUI.enabled = false;
                    EditorGUI.LabelField(position, "Scene Name", sceneDefinition.sceneName);
                    position.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(position, "Display Name", sceneDefinition.DisplayName);
                    GUI.enabled = true;
                    EditorGUI.indentLevel--;
                
                    // Display the scene image if it exists
                    if (sceneDefinition.sceneImage != null)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2;
                    
                        // Calculate aspect ratio to maintain proportions
                        Texture2D texture = sceneDefinition.sceneImage.texture;
                        float aspectRatio = (float)texture.width / texture.height;
                        float previewHeight = 64;
                        float previewWidth = previewHeight * aspectRatio;
                    
                        // Center the preview horizontally
                        Rect previewRect = new Rect(
                            position.x + (position.width - previewWidth) * 0.5f,
                            position.y,
                            previewWidth,
                            previewHeight
                        );
                    
                        EditorGUI.DrawPreviewTexture(previewRect, texture);
                        position.y += previewHeight + 2;
                        position.height = EditorGUIUtility.singleLineHeight;
                    }
                    else
                    {
                        position.y += EditorGUIUtility.singleLineHeight + 2;
                    }
                
                    // Add button to edit the SceneDefinition
                    if (GUI.Button(position, "Edit Scene Definition"))
                    {
                        Selection.activeObject = sceneDefinition;
                    }
                }
            }
            else
            {
                // Show a dropdown to select from available scenes when no scene is assigned
                position.y += EditorGUIUtility.singleLineHeight + 2;
            
                if (GUI.Button(position, "Select from available scenes"))
                {
                    // Create a context menu with available scenes
                    var menu = new GenericMenu();
                
                    // Try to find scene definitions in the project
                    var assets = Resources.LoadAll<SceneDefinitionAssets>("");
                    if (assets != null && assets.Length > 0)
                    {
                        var sceneAssets = assets[0];
                    
                        // Add scenes from the asset
                        var allScenes = sceneAssets.GetAllScenes();
                        foreach (var scene in allScenes)
                        {
                            if (scene != null)
                            {
                                menu.AddItem(new GUIContent(scene.DisplayName), false, () => {
                                    property.objectReferenceValue = scene;
                                    property.serializedObject.ApplyModifiedProperties();
                                });
                            }
                        }
                    }
                    else
                    {
                        // Find all SceneDefinition assets in the project
                        var guids = AssetDatabase.FindAssets("t:SceneDefinition");
                        var paths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g));
                        var scenes = paths.Select(p => AssetDatabase.LoadAssetAtPath<SceneDefinition>(p))
                                          .Where(s => s != null)
                                          .ToList();
                    
                        if (scenes.Count > 0)
                        {
                            foreach (var scene in scenes)
                            {
                                menu.AddItem(new GUIContent(scene.DisplayName), false, () => {
                                    property.objectReferenceValue = scene;
                                    property.serializedObject.ApplyModifiedProperties();
                                });
                            }
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("No scene definitions found"));
                        }
                    }
                
                    menu.ShowAsContext();
                }
            }
        
            EditorGUI.EndProperty();
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
        
            // Add height for the selection button if no value
            if (property.objectReferenceValue == null)
            {
                height += EditorGUIUtility.singleLineHeight + 2;
            }
            // Add height for preview fields and edit button if there's a value
            else
            {
                SceneDefinition sceneDefinition = property.objectReferenceValue as SceneDefinition;
                height += (EditorGUIUtility.singleLineHeight * 3) + 4;
            
                // Add extra height for sprite preview
                if (sceneDefinition != null && sceneDefinition.sceneImage != null)
                {
                    height += 64 + 4; // 64px for the image plus spacing
                }
            }
        
            return height;
        }
    }
}

#endif