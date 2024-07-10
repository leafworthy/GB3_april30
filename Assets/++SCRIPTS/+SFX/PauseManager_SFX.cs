using UnityEngine;

public class PauseManager_SFX : MonoBehaviour
{

	private void OnEnable()
	{
		PauseManager.OnPause += PauseManager_OnPause;
		PauseManager.OnUnpause += PauseManager_OnUnpause;
		PauseManager.OnPlayerPressedSelect += PauseManager_OnPlayerPressedSelect;
		PauseManager.OnPlayerPressedUp += PauseManager_OnPlayerPressedUp;
		PauseManager.OnPlayerPressedDown += PauseManager_OnPlayerPressedDown;
	}

	private void OnDisable()
	{
		PauseManager.OnPause -= PauseManager_OnPause;
		PauseManager.OnUnpause -= PauseManager_OnUnpause;
		PauseManager.OnPlayerPressedSelect -= PauseManager_OnPlayerPressedSelect;
		PauseManager.OnPlayerPressedUp -= PauseManager_OnPlayerPressedUp;
		PauseManager.OnPlayerPressedDown -= PauseManager_OnPlayerPressedDown;
	}

	private void PauseManager_OnPlayerPressedDown() => SFX.sounds.pauseMenu_move_sounds.PlayRandom();

	private void PauseManager_OnPlayerPressedUp() => SFX.sounds.pauseMenu_move_sounds.PlayRandom();

	private void PauseManager_OnPlayerPressedSelect() => SFX.sounds.pauseMenu_select_sounds.PlayRandom();

	private void PauseManager_OnUnpause(Player obj) => SFX.sounds.pauseMenu_stop_sounds.PlayRandom();

	private void PauseManager_OnPause(Player obj) => SFX.sounds.pauseMenu_start_sounds.PlayRandom();
}