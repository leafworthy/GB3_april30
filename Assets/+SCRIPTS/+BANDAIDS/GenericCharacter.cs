using System.Collections.Generic;
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
		public Color bodyTintColor;
		public Vector2 leftArmOffset;
		public Vector2 rightArmOffset;
		public Vector2 leftLegOffset;
		public Vector2 rightLegOffset;
		public Vector2 faceOffset;
		public List<SpriteRenderer> toTintRenderers;
	}

	[System.Serializable, CreateAssetMenu(menuName = "My Assets/GenericBody")]
	public class GenericBody : ScriptableObject
	{
		public string displayName;
		public Sprite bodySprite;
		public Vector2 leftArmOffset;
		public Vector2 rightArmOffset;
		public Vector2 leftLegOffset;
		public Vector2 rightLegOffset;
		public Vector2 faceOffset;
		public List<int> faceIndexes;
		public Color tintColor = Color.white;
		public Color bodyTintColor = Color.white;
	}

}
