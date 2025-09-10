using UnityEngine;

namespace __SCRIPTS
{
	public class SceneCharacterSelection_SFX : MonoBehaviour
	{
		private GameSceneCharacterSelection gameSceneCharacterSelection;

		private void OnEnable()
		{
			gameSceneCharacterSelection = GetComponent<GameSceneCharacterSelection>();
			gameSceneCharacterSelection.OnPlayerStartsSelecting += SceneCharacterSelection_OnPlayerStartsSelecting;
			gameSceneCharacterSelection.OnPlayerUnjoins += SceneCharacterSelection_OnPlayerUnjoins;
			gameSceneCharacterSelection.OnPlayerMoveLeft += SceneCharacterSelection_OnPlayerMoveLeft;
			gameSceneCharacterSelection.OnPlayerMoveRight += SceneCharacterSelection_OnPlayerMoveRight;
			gameSceneCharacterSelection.OnTryToStartGame += SceneCharacterSelection_OnTryToStartGame;
			gameSceneCharacterSelection.OnSelectCharacter += SceneCharacterSelection_OnSelectCharacter;
			gameSceneCharacterSelection.OnDeselectCharacter += SceneCharacterSelection_OnDeselectCharacter;
		}

		private void OnDisable()
		{
			gameSceneCharacterSelection.OnPlayerStartsSelecting -= SceneCharacterSelection_OnPlayerStartsSelecting;
			gameSceneCharacterSelection.OnPlayerUnjoins -= SceneCharacterSelection_OnPlayerUnjoins;
			gameSceneCharacterSelection.OnPlayerMoveLeft -= SceneCharacterSelection_OnPlayerMoveLeft;
			gameSceneCharacterSelection.OnPlayerMoveRight -= SceneCharacterSelection_OnPlayerMoveRight;
			gameSceneCharacterSelection.OnTryToStartGame -= SceneCharacterSelection_OnTryToStartGame;
			gameSceneCharacterSelection.OnSelectCharacter -= SceneCharacterSelection_OnSelectCharacter;
			gameSceneCharacterSelection.OnDeselectCharacter -= SceneCharacterSelection_OnDeselectCharacter;
		}

		private void SceneCharacterSelection_OnDeselectCharacter() => Services.sfx.sounds.charSelect_deselect_sounds.PlayRandom();
		private void SceneCharacterSelection_OnSelectCharacter() => Services.sfx.sounds.charSelect_select_sounds.PlayRandom();



		private void SceneCharacterSelection_OnTryToStartGame() => Services.sfx.sounds.pickup_speed_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerMoveRight() => Services.sfx.sounds.charSelect_move_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerMoveLeft() => Services.sfx.sounds.charSelect_move_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerUnjoins() => Services.sfx.sounds.charSelect_move_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerStartsSelecting() => Services.sfx.sounds.charSelect_select_sounds.PlayRandom();
	}
}
