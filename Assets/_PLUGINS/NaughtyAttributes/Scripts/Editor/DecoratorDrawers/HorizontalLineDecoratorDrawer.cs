using _PLUGINS.NaughtyAttributes.Scripts.Core.DrawerAttributes;
using _PLUGINS.NaughtyAttributes.Scripts.Core.Utility;
using _PLUGINS.NaughtyAttributes.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace _PLUGINS.NaughtyAttributes.Scripts.Editor.DecoratorDrawers
{
	[CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
	public class HorizontalLineDecoratorDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			HorizontalLineAttribute lineAttr = (HorizontalLineAttribute)attribute;
			return EditorGUIUtility.singleLineHeight + lineAttr.Height;
		}

		public override void OnGUI(Rect position)
		{
			Rect rect = EditorGUI.IndentedRect(position);
			rect.y += EditorGUIUtility.singleLineHeight / 3.0f;
			HorizontalLineAttribute lineAttr = (HorizontalLineAttribute)attribute;
			NaughtyEditorGUI.HorizontalLine(rect, lineAttr.Height, lineAttr.Color.GetColor());
		}
	}
}
