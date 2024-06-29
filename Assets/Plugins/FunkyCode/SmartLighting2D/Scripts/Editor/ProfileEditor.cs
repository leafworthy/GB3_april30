using FunkyCode.SmartLighting2D.Scripts.Editor.Settings;
using FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;

namespace FunkyCode.SmartLighting2D.Scripts.Editor
{
	[CustomEditor(typeof(Profile))]
	public class ProfileEditor2 : UnityEditor.Editor
	{
		override public void OnInspectorGUI()
		{
			Profile profile = target as Profile;

			ProfileEditor.DrawProfile(profile);
		}
	}
}
