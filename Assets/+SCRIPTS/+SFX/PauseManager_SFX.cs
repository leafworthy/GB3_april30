using UnityEngine;

namespace __SCRIPTS
{
	public class PauseManager_SFX : MonoBehaviour
	{

		private void OnEnable()
		{
			Services.pauseManager.OnPause += PauseManager_OnPause;
			Services.pauseManager.OnUnpause += PauseManager_OnUnpause;
			Services.pauseManager.OnPlayerPressedSelect += PauseManager_OnPlayerPressedSelect;
			Services.pauseManager.OnPlayerPressedUp += PauseManager_OnPlayerPressedUp;
			Services.pauseManager.OnPlayerPressedDown += PauseManager_OnPlayerPressedDown;
		}

		private void OnDisable()
		{
			Services.pauseManager.OnPause -= PauseManager_OnPause;
			Services.pauseManager.OnUnpause -= PauseManager_OnUnpause;
			Services.pauseManager.OnPlayerPressedSelect -= PauseManager_OnPlayerPressedSelect;
			Services.pauseManager.OnPlayerPressedUp -= PauseManager_OnPlayerPressedUp;
			Services.pauseManager.OnPlayerPressedDown -= PauseManager_OnPlayerPressedDown;
		}

		private void PauseManager_OnPlayerPressedDown() => Services.sfx.sounds.pauseMenu_move_sounds.PlayRandom();

		private void PauseManager_OnPlayerPressedUp() => Services.sfx.sounds.pauseMenu_move_sounds.PlayRandom();
		private void PauseManager_OnPlayerPressedSelect() => Services.sfx.sounds.pauseMenu_select_sounds.PlayRandom();

		private void PauseManager_OnUnpause(Player obj) => Services.sfx.sounds.pauseMenu_stop_sounds.PlayRandom();

		private void PauseManager_OnPause(Player obj) => Services.sfx.sounds.pauseMenu_start_sounds.PlayRandom();
	}
}
