using UnityEngine;
using UnityEngine.Serialization;

namespace GangstaBean.Assets
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
		public GameObject LevelTransitionScreen;
	}
}