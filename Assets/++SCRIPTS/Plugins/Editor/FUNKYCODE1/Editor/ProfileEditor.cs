using __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEditor;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor
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
