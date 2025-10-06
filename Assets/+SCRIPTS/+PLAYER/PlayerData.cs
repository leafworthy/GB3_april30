using UnityEngine;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/PlayerData")]
	public class PlayerData : ScriptableObject
	{
		//public PlayerIndex playerIndex;
		public Color playerColor;
		public bool isPlayer = true;
		public int startingCash = 0;
		public int startingGas = 0;

	}
}
