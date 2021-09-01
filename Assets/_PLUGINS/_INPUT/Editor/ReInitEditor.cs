using _PLUGINS._INPUT.Scripts;
using UnityEditor;

namespace _PLUGINS._INPUT.Editor {
	[CustomEditor(typeof(ReInit))]
	public class ReInitEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			EditorGUILayout.HelpBox("The automatically generated \"GamePad ReInit\" gameobject " +
			                        "and this script are used to detect if a gamepad has (dis)connected.", MessageType.Info);
		}
	}
}
