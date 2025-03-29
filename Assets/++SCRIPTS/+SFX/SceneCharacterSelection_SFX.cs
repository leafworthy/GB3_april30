using UnityEngine;

namespace __SCRIPTS
{
	public class SceneCharacterSelection_SFX : MonoBehaviour
	{
		private GameSceneCharacterSelection gameSceneCharacterSelection;

		private void OnEnable()
		{
			gameSceneCharacterSelection = GetComponent<GameSceneCharacterSelection>();
			GameSceneCharacterSelection.OnPlayerStartsSelecting += SceneCharacterSelection_OnPlayerStartsSelecting;
			GameSceneCharacterSelection.OnPlayerUnjoins += SceneCharacterSelection_OnPlayerUnjoins;
			GameSceneCharacterSelection.OnPlayerMoveLeft += SceneCharacterSelection_OnPlayerMoveLeft;
			GameSceneCharacterSelection.OnPlayerMoveRight += SceneCharacterSelection_OnPlayerMoveRight;
			GameSceneCharacterSelection.OnTryToStartGame += SceneCharacterSelection_OnTryToStartGame;
			GameSceneCharacterSelection.OnSelectCharacter += SceneCharacterSelection_OnSelectCharacter;
			GameSceneCharacterSelection.OnDeselectCharacter += SceneCharacterSelection_OnDeselectCharacter;
		}

		private void OnDisable()
		{
			GameSceneCharacterSelection.OnPlayerStartsSelecting -= SceneCharacterSelection_OnPlayerStartsSelecting;
			GameSceneCharacterSelection.OnPlayerUnjoins -= SceneCharacterSelection_OnPlayerUnjoins;
			GameSceneCharacterSelection.OnPlayerMoveLeft -= SceneCharacterSelection_OnPlayerMoveLeft;
			GameSceneCharacterSelection.OnPlayerMoveRight -= SceneCharacterSelection_OnPlayerMoveRight;
			GameSceneCharacterSelection.OnTryToStartGame -= SceneCharacterSelection_OnTryToStartGame;
			GameSceneCharacterSelection.OnSelectCharacter -= SceneCharacterSelection_OnSelectCharacter;
			GameSceneCharacterSelection.OnDeselectCharacter -= SceneCharacterSelection_OnDeselectCharacter;
		}

		private void SceneCharacterSelection_OnDeselectCharacter() => SFX.sounds.charSelect_deselect_sounds.PlayRandom();
		private void SceneCharacterSelection_OnSelectCharacter() => SFX.sounds.charSelect_select_sounds.PlayRandom();



		private void SceneCharacterSelection_OnTryToStartGame() => SFX.sounds.pickup_speed_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerMoveRight() => SFX.sounds.charSelect_move_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerMoveLeft() => SFX.sounds.charSelect_move_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerUnjoins() => SFX.sounds.charSelect_move_sounds.PlayRandom();

		private void SceneCharacterSelection_OnPlayerStartsSelecting() => SFX.sounds.charSelect_select_sounds.PlayRandom();
	}
}