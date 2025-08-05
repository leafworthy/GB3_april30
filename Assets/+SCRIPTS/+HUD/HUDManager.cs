using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDManager : MonoBehaviour, IService
	{
		private bool isOn;
		private List<HUDSlot> currentHUDSlots = new();

		public GameObject Vignette;


		private LevelManager _levelManager;
		private LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();
		private Players _playerManager;
		private Players playerManager => _playerManager ?? ServiceLocator.Get<Players>();
		private bool isInGame;

		public void StartService()
		{
			Vignette.SetActive(false);
			levelManager.OnStartLevel += LevelSceneOnStartLevel;
			levelManager.OnLevelSpawnedPlayer += LevelSceneOnLevelSpawnedPlayer;
			levelManager.OnGameOver += Level_OnGameOver;
			levelManager.OnWinGame += Level_OnGameOver;
			levelManager.OnStopLevel += t => Level_OnGameOver();

			playerManager.OnPlayerJoins += PlayerOnPlayerJoins;
			DisableAllHUDSlots();
		}



		private void Level_OnGameOver()
		{
			Vignette.SetActive(false);
			playerManager.SetActionMaps(Players.UIActionMap);
			isInGame = false;
			DisableAllHUDSlots();
		}

		private void OnDisable()
		{
			levelManager.OnStartLevel -= LevelSceneOnStartLevel;
			levelManager.OnLevelSpawnedPlayer -= LevelSceneOnLevelSpawnedPlayer;
			playerManager.OnPlayerJoins -= PlayerOnPlayerJoins;
			levelManager.OnGameOver -= Level_OnGameOver;
			levelManager.OnWinGame -= Level_OnGameOver;

			playerManager.SetActionMaps(Players.UIActionMap);
			Vignette.SetActive(false);
		}

		private void PlayerOnPlayerJoins(Player player)
		{
			if (!isInGame) return;
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
			SetHUDSlotPlayer(player, true);

		}

		private void LevelSceneOnLevelSpawnedPlayer(Player player)
		{
			Debug.Log("HUD MANAGER: " + player.name + player.input.playerIndex + " has spawned in-game, setting HUD slot." );
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
			SetHUDSlotPlayer(player);
		}

		private void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			Debug.Log("HUD MANAGER: OnStart() creating HUD for players, event registering");
			isInGame = true;
			CreateHUDForPlayers(playerManager.AllJoinedPlayers);

			Vignette.SetActive(true);
		}


		private void CreateHUDForPlayers(List<Player> players)
		{
			foreach (var player in players)
			{
				Debug.Log( "HUD MANAGER: Creating HUD for player: " + player.name + " with input " + player.input.playerIndex);
				SetHUDSlotPlayer(player);
			}
		}

		private void SetHUDSlotPlayer(Player player, bool withMenu = false)
		{
			var slot = currentHUDSlots[player.input.playerIndex];
			slot.gameObject.SetActive(true);

			if (withMenu)
				slot.OpenCharacterSelectMenu(player);
			else
				slot.SetPlayer(player);
		}

		private void DisableAllHUDSlots()
		{
			currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();
			foreach (var hudSlot in currentHUDSlots)
			{
				hudSlot.gameObject.SetActive(false);
			}
		}
	}
}
