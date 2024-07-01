using __SCRIPTS._COMMON;
using __SCRIPTS._UI;
using UnityEngine;

namespace __SCRIPTS._SFX
{
	public class SceneCharacterSelection_SFX : MonoBehaviour
	{
		private SceneCharacterSelection sceneCharacterSelection;

		private void OnEnable()
		{
			sceneCharacterSelection = GetComponent<SceneCharacterSelection>();
			SceneCharacterSelection.OnPlayerStartsSelecting += SceneCharacterSelection_OnPlayerStartsSelecting;
			SceneCharacterSelection.OnPlayerUnjoins += SceneCharacterSelection_OnPlayerUnjoins;
			SceneCharacterSelection.OnPlayerMoveLeft += SceneCharacterSelection_OnPlayerMoveLeft;
			SceneCharacterSelection.OnPlayerMoveRight += SceneCharacterSelection_OnPlayerMoveRight;
			SceneCharacterSelection.OnTryToStartGame += SceneCharacterSelection_OnTryToStartGame;
			SceneCharacterSelection.OnSelectCharacter += SceneCharacterSelection_OnSelectCharacter;
			SceneCharacterSelection.OnDeselectCharacter += SceneCharacterSelection_OnDeselectCharacter;
		}

		private void OnDisable()
		{
			SceneCharacterSelection.OnPlayerStartsSelecting -= SceneCharacterSelection_OnPlayerStartsSelecting;
			SceneCharacterSelection.OnPlayerUnjoins -= SceneCharacterSelection_OnPlayerUnjoins;
			SceneCharacterSelection.OnPlayerMoveLeft -= SceneCharacterSelection_OnPlayerMoveLeft;
			SceneCharacterSelection.OnPlayerMoveRight -= SceneCharacterSelection_OnPlayerMoveRight;
			SceneCharacterSelection.OnTryToStartGame -= SceneCharacterSelection_OnTryToStartGame;
			SceneCharacterSelection.OnSelectCharacter -= SceneCharacterSelection_OnSelectCharacter;
			SceneCharacterSelection.OnDeselectCharacter -= SceneCharacterSelection_OnDeselectCharacter;
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