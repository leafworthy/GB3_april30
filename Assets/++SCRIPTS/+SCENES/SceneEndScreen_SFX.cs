using __SCRIPTS._COMMON;
using __SCRIPTS._SFX;
using UnityEngine;

namespace __SCRIPTS._SCENES
{
	public class SceneEndScreen_SFX : MonoBehaviour
	{
		private SceneEndScene sceneEndScreen;

		private void OnEnable()
		{
			sceneEndScreen = GetComponent<SceneEndScene>();
			sceneEndScreen.OnPlayerPressedSelect += SceneEndScreen_OnPlayerPressedSelect;
			sceneEndScreen.OnPlayerPressedUp += SceneEndScreen_OnPlayerPressedUp;
			sceneEndScreen.OnPlayerPressedDown += SceneEndScreen_OnPlayerPressedDown;
		}

		private void SceneEndScreen_OnPlayerPressedDown()
		{
			SFX.sounds.pauseMenu_move_sounds.PlayRandom();
		}

		private void SceneEndScreen_OnPlayerPressedUp()
		{
			SFX.sounds.pauseMenu_move_sounds.PlayRandom();
		}

		private void SceneEndScreen_OnPlayerPressedSelect()
		{
			SFX.sounds.pauseMenu_select_sounds.PlayRandom();
		}
	}
}
