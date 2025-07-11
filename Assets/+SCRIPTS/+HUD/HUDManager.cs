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

		public bool canJoin;

		private LevelManager _levelManager;
		private LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();
		private Players _playerManager;
		private Players playerManager => _playerManager ?? ServiceLocator.Get<Players>();


		public void StartService()
		{
			Vignette.SetActive(false);
			levelManager.OnStartLevel += LevelSceneOnStartLevel;
			levelManager.OnPlayerSpawned += LevelScene_OnPlayerSpawned;
			levelManager.OnGameOver += Level_OnGameOver;
			levelManager.OnWinGame += Level_OnGameOver;
			levelManager.OnStopLevel += t => Level_OnGameOver();

			playerManager.OnPlayerJoins += JoinInGame;
			playerManager.OnPlayerGetUpgrades += OpenUpgradePanel;
			Debug.Log("huds has registered events");
			canJoin = true;
			DisableAllHUDSlots();
			Debug.Log("HUDS enabled and ready for players to join.");

		}



		private void Level_OnGameOver()
		{
			Vignette.SetActive(false);
			canJoin = false;
			playerManager.SetActionMaps(Players.UIActionMap);
			DisableAllHUDSlots();
			//gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			canJoin = false;
			playerManager.OnPlayerJoins -= JoinInGame;
			playerManager.OnPlayerGetUpgrades -= OpenUpgradePanel;
			playerManager.SetActionMaps(Players.UIActionMap);
			Vignette.SetActive(false);
		}

		private void LevelScene_OnPlayerSpawned(Player player)
		{
			Debug.Log("hud" + player.name + " is trying to join in-game with input " + player.input.playerIndex);
			SetHUDSlotPlayer(player);
		}

		private void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			Debug.Log("creating HUD for players, event registering");
			CreateHUDForPlayers(playerManager.AllJoinedPlayers);

			Vignette.SetActive(true);
			canJoin = true;
		}

		private void OpenUpgradePanel(Player player)
		{
			var slot = currentHUDSlots[player.input.playerIndex];
			slot.gameObject.SetActive(true);
			slot.OpenUpgradeSelectMenu(player);
		}

		private void CreateHUDForPlayers(List<Player> players)
		{
			foreach (var player in players)
			{
				SetHUDSlotPlayer(player);
			}
		}

		private void JoinInGame(Player player)
		{
			Debug.Log("Player " + player.name + " is trying to join in-game with input " + player.input.playerIndex);
			if (!canJoin)
			{
				Debug.Log("cant join");
				return;
			}

			// Ensure player stats are properly initialized before setting up HUD
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats(); // Initialize stats for mid-game join

			SetHUDSlotPlayer(player, true);
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
