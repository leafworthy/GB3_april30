#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(InLineEditorAttribute), true)]
    public class InLineEditorDrawer : GgPropertyDrawer
    {
        #region Variables
        
        private Editor inLineEditor;

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region GgPropertyHeight

        protected override float GgPropertyHeight(SerializedProperty property, float propertyHeight, float approxFieldWidth)
        {
            inLineEditor = Editor.CreateEditor(property.objectReferenceValue);
            return propertyHeight - (singleLineHeight + standardSpacing);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnGgGUI

        protected override void OnGgGUI(Rect position, SerializedProperty property, GUIContent label, GgGUIDefaults defaultCache)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                TryDrawInLineEditor(property, label);
            }
            else
            {
                GgGUI.CustomPropertyField(position, property, label);
            }
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private void TryDrawInLineEditor(SerializedProperty property, GUIContent label)
        {
            property.isExpanded = EditorExtensions.BeginFoldoutObjectGroupNestable(property, label, property.isExpanded, EditorStyles.helpBox);
            if (property.isExpanded)
            {
                if (property.objectReferenceValue && (property.objectReferenceValue is Component || property.objectReferenceValue is ScriptableObject))
                {
                    if (inLineEditor)
                    {
                        EditorExtensions.DrawInspectorLine(InspectorExtensions.backgroundShadowColor, -2);
                        inLineEditor.OnInspectorGUI(true);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Error: InLineEditor asset cannot be created.", MessageType.Error);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Warning: Reference object asset is null.", MessageType.Warning);
                }
            }
            EditorExtensions.EndFoldoutGroupNestable();
        }

        #endregion

    } // class end
}

#endif