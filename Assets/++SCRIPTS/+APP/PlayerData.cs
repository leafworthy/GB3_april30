using _PLUGINS._INPUT.Scripts;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/PlayerData")]
public class PlayerData : ScriptableObject
{
	public PlayerIndex playerIndex;
	public Color playerColor;
	public bool isPlayer = true;
	public bool isUsingKeyboard;
	public KeyboardControlSetup keyboardControlSetup;
}
