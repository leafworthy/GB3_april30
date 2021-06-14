using UnityEngine;


[CreateAssetMenu(menuName = "KeyboardControlSetup", order = 1)]
public class KeyboardControlSetup:ScriptableObject
{
	public KeyboardKey MoveUpKey;
	public KeyboardKey MoveRightKey;
	public KeyboardKey MoveDownKey;
	public KeyboardKey MoveLeftKey;

	public KeyboardKey Attack1Key;
	public KeyboardKey Attack2Key;
	public KeyboardKey Attack3Key;
	public KeyboardKey Attack4Key;

	public KeyboardKey JumpKey;
	public KeyboardKey PauseKey;
	public KeyboardKey DashKey;
	public KeyboardKey Special4Key;
}
