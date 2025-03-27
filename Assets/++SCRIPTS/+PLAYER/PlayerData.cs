using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/PlayerData")]
public class PlayerData : ScriptableObject
{
	//public PlayerIndex playerIndex;
	public Color playerColor;
	public bool isPlayer = true;
	public int startingCash = 1500;
	public int startingGas = 3;

}