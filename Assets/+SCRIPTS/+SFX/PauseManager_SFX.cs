using UnityEngine;

namespace __SCRIPTS
{
	public class PauseManager_SFX : ServiceUser
	{

		private void OnEnable()
		{
			pauseManager.OnPause += PauseManager_OnPause;
			pauseManager.OnUnpause += PauseManager_OnUnpause;
			pauseManager.OnPlayerPressedSelect += PauseManager_OnPlayerPressedSelect;
			pauseManager.OnPlayerPressedUp += PauseManager_OnPlayerPressedUp;
			pauseManager.OnPlayerPressedDown += PauseManager_OnPlayerPressedDown;
		}

		private void OnDisable()
		{
			pauseManager.OnPause -= PauseManager_OnPause;
			pauseManager.OnUnpause -= PauseManager_OnUnpause;
			pauseManager.OnPlayerPressedSelect -= PauseManager_OnPlayerPressedSelect;
			pauseManager.OnPlayerPressedUp -= PauseManager_OnPlayerPressedUp;
			pauseManager.OnPlayerPressedDown -= PauseManager_OnPlayerPressedDown;
		}

		private void PauseManager_OnPlayerPressedDown() => sfx.sounds.pauseMenu_move_sounds.PlayRandom();

		private void PauseManager_OnPlayerPressedUp() => sfx.sounds.pauseMenu_move_sounds.PlayRandom();
		private void PauseManager_OnPlayerPressedSelect() => sfx.sounds.pauseMenu_select_sounds.PlayRandom();

		private void PauseManager_OnUnpause(Player obj) => sfx.sounds.pauseMenu_stop_sounds.PlayRandom();

		private void PauseManager_OnPause(Player obj) => sfx.sounds.pauseMenu_start_sounds.PlayRandom();
	}
}
