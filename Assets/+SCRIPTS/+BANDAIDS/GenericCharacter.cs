using UnityEngine;

namespace __SCRIPTS
{
	[System.Serializable,CreateAssetMenu(menuName = "My Assets/GenericCharacter")]
	public class GenericCharacter: ScriptableObject
	{
		public string displayName;
		public Sprite sprite;
		public int faceIndex;
		public Color tintColor;
		public Vector2 leftArmOffset;
		public Vector2 rightArmOffset;
		public Vector2 leftLegOffset;
		public Vector2 rightLegOffset;
		public Vector2 faceOffset;
	}
}
