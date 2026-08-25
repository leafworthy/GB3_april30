using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class ModeSelector : MonoBehaviour
	{
		//STATE
		MenuButton currentlySelectedCharacterButton;
		public event Action OnPlayerPressedSelect;
		public event Action OnPlayerPressedUp;
		public event Action OnPlayerPressedDown;
		public MenuButtons menuButtons;
		List<Player> playersBeingListenedTo = new();

		void OnEnable()
		{

			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				ListenToPlayer(player);
			}

			menuButtons.InitButtons();
		}

		void Start()
		{menuButtons.InitButtons();
		}

		void ListenToPlayer(Player newPlayer)
		{
			if (playersBeingListenedTo.Contains(newPlayer)) return;
			Debug.Log("Listening to player: " + newPlayer.name);
			playersBeingListenedTo.Add(newPlayer);
			newPlayer.Controller.Select.OnPress += PlayerPressedSelect;
			newPlayer.Controller.UIAxis.OnUp += PlayerPressedUp;
			newPlayer.Controller.UIAxis.OnDown += PlayerPressedDown;
		}

		void OnDisable()
		{
			StopListeningToJoinedPlayers();
			OnPlayerPressedSelect = null;
			OnPlayerPressedUp = null;
			OnPlayerPressedDown = null;
			currentlySelectedCharacterButton = null;
		}

		void StopListeningToJoinedPlayers()
		{

			foreach (var joinedPlayer in playersBeingListenedTo)
			{
				joinedPlayer.Controller.Select.OnPress -= PlayerPressedSelect;
				joinedPlayer.Controller.UIAxis.OnUp -= PlayerPressedUp;
				joinedPlayer.Controller.UIAxis.OnDown -= PlayerPressedDown;
			}

			playersBeingListenedTo.Clear();
		}

		void PlayerPressedDown(IControlAxis controlAxis)
		{

			menuButtons.Down();
			OnPlayerPressedDown?.Invoke();
		}

		void PlayerPressedSelect(NewControlButton newControlButton)
		{
			currentlySelectedCharacterButton = menuButtons.GetCurrentButton();
			OnPlayerPressedSelect?.Invoke();

			switch (currentlySelectedCharacterButton.type)
			{
				case MenuButton.ButtonType.MainMenu:
					SelectArcade();
					break;
				case MenuButton.ButtonType.Damage:
					SelectSurvival();
					break;
				}
		}

		void SelectSurvival()
		{
			StopListeningToJoinedPlayers();
			Services.sfx.sounds.pauseMenu_start_sounds.PlayRandom();
			GameManager.SetGameMode(GameManager.GameMode.Survival);
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.characterSelect);
		}

		void SelectArcade()
		{
			StopListeningToJoinedPlayers();
			Services.sfx.sounds.press_start_sounds.PlayRandom();
			GameManager.SetGameMode(GameManager.GameMode.Arcade);
			Services.sceneLoader.GoToScene(Services.assetManager.Scenes.characterSelect);
		}

		void PlayerPressedUp(IControlAxis controlAxis)
		{

			menuButtons.Up();
			OnPlayerPressedUp?.Invoke();
		}






	}
}
