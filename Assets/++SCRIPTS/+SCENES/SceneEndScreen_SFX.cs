using UnityEngine;

public class SceneEndScreen_SFX : MonoBehaviour
{
	private GameSceneEndGameScene gameSceneEndGameScreen;

	private void OnEnable()
	{
		gameSceneEndGameScreen = GetComponent<GameSceneEndGameScene>();
		gameSceneEndGameScreen.OnPlayerPressedSelect += GameSceneEndGameScreenOnPlayerPressedSelect;
		gameSceneEndGameScreen.OnPlayerPressedUp += GameSceneEndGameScreenOnPlayerPressedUp;
		gameSceneEndGameScreen.OnPlayerPressedDown += GameSceneEndGameScreenOnPlayerPressedDown;
	}

	private void GameSceneEndGameScreenOnPlayerPressedDown()
	{
		SFX.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void GameSceneEndGameScreenOnPlayerPressedUp()
	{
		SFX.sounds.pauseMenu_move_sounds.PlayRandom();
	}

	private void GameSceneEndGameScreenOnPlayerPressedSelect()
	{
		SFX.sounds.pauseMenu_select_sounds.PlayRandom();
	}
}