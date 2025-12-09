using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/UI Assets")]
	public class UIAssets:ScriptableObject
	{
		public GameObject Menu_Main;
		public GameObject Menu_CharacterSelection;
		public GameObject Menu_InGame;
		public GameObject Menu_Endscreen;
		public GameObject PlayerStatsDisplayPrefab;
		public GameObject HUD;
		public GameObject CursorPrefab;
		[FormerlySerializedAs("Indicator")] public GameObject InteractionIndicator;
		public GameObject InventorySlotPrefab;
		public GameObject SpawnCellPrefab;
		public Sprite Toast_Avatar;
		public Sprite Cone_Avatar;
		public Sprite Corn_Avatar;
		public Sprite Donut_Avatar;
		public Sprite Fruit_Avatar;

	}
}
