using UnityEngine;

namespace __SCRIPTS
{
	public class PauseManager_SFX : MonoBehaviour
	{

		private void OnEnable()
		{
			PauseManager.I.OnPause += PauseManager_OnPause;
			PauseManager.I.OnUnpause += PauseManager_OnUnpause;
			PauseManager.I.OnPlayerPressedSelect += PauseManager_OnPlayerPressedSelect;
			PauseManager.I.OnPlayerPressedUp += PauseManager_OnPlayerPressedUp;
			PauseManager.I.OnPlayerPressedDown += PauseManager_OnPlayerPressedDown;
		}

		private void OnDisable()
		{
			PauseManager.I.OnPause -= PauseManager_OnPause;
			PauseManager.I.OnUnpause -= PauseManager_OnUnpause;
			PauseManager.I.OnPlayerPressedSelect -= PauseManager_OnPlayerPressedSelect;
			PauseManager.I.OnPlayerPressedUp -= PauseManager_OnPlayerPressedUp;
			PauseManager.I.OnPlayerPressedDown -= PauseManager_OnPlayerPressedDown;
		}

		private void PauseManager_OnPlayerPressedDown() => SFX.I.sounds.pauseMenu_move_sounds.PlayRandom();

		private void PauseManager_OnPlayerPressedUp() => SFX.I.sounds.pauseMenu_move_sounds.PlayRandom();
		private void PauseManager_OnPlayerPressedSelect() => SFX.I.sounds.pauseMenu_select_sounds.PlayRandom();

		private void PauseManager_OnUnpause(Player obj) => SFX.I.sounds.pauseMenu_stop_sounds.PlayRandom();

		private void PauseManager_OnPause(Player obj) => SFX.I.sounds.pauseMenu_start_sounds.PlayRandom();
	}
}