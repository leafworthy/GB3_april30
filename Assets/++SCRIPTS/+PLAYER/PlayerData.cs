using UnityEngine;

namespace __SCRIPTS._PLAYER
{
	[CreateAssetMenu(menuName = "My Assets/PlayerData")]
	public class PlayerData : ScriptableObject
	{
		//public PlayerIndex playerIndex;
		public Color playerColor;
		public bool isPlayer = true;
		public int startingCash = 500;
		public int startingGas = 3;

	}
}
