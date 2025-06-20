using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDS : Singleton<HUDS>
	{
		private bool isOn;
		private List<HUDSlot> currentHUDSlots = new List<HUDSlot>();

		public GameObject Vignette;
		public GameObject Clock;

		public bool canJoin = false;
		private void OnEnable()
		{

			Vignette.SetActive(false);
			Clock.SetActive(false);
			LevelManager.I.OnStartLevel += LevelSceneOnStartLevel;
			LevelManager.I.OnPlayerSpawned += LevelScene_OnPlayerSpawned;
			LevelManager.I.OnGameOver += Level_OnGameOver;
			LevelManager.I.OnWinGame += Level_OnGameOver;
			LevelManager.I.OnStopLevel += (t)=> Level_OnGameOver();
			canJoin = true;
			DisableAllHUDSlots();
			
		}

	


		private void Level_OnGameOver()
		{
			Vignette.SetActive(false);
			Clock.SetActive(false);
			canJoin = false;
			Players.SetActionMaps(Players.UIActionMap);
			DisableAllHUDSlots();
			//gameObject.SetActive(false);

		}

		private void OnDisable()
		{
			canJoin  = false;
			Players.I.OnPlayerJoins -= JoinInGame;
			Players.I.OnPlayerGetUpgrades -= OpenUpgradePanel;
			Players.SetActionMaps(Players.UIActionMap);
			Vignette.SetActive(false);
			Clock.SetActive(false);

		}

	

		private void LevelScene_OnPlayerSpawned(Player player)
		{

			SetHUDSlotPlayer(player);
		}

		private void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			//gameObject.SetActive(true);

			CreateHUDForPlayers(Players.I.AllJoinedPlayers);
			Players.I.OnPlayerJoins += JoinInGame;
			Players.I.OnPlayerGetUpgrades += OpenUpgradePanel;
			Vignette.SetActive(true);
			Clock.SetActive(true);
			canJoin = true;
		}

		private void OpenUpgradePanel(Player player)
		{
			var slot = I.currentHUDSlots[(int)player.input.playerIndex];
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
			if(!canJoin) return;

			
			// Ensure player stats are properly initialized before setting up HUD
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null)
			{
				playerStats.ResetStats(); // Initialize stats for mid-game join
			}
			
			var slot = I.currentHUDSlots[(int) player.input.playerIndex];
			slot.gameObject.SetActive(true);
			slot.StartCharSelectMenu(player);
		}

		private  void SetHUDSlotPlayer(Player player)
		{
			var slot = I.currentHUDSlots[(int)player.input.playerIndex];
			slot.gameObject.SetActive(true);
			slot.SetPlayer(player);
		}
		private void DisableAllHUDSlots()
		{
			currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();
			foreach (var hudSlot in currentHUDSlots) hudSlot.gameObject.SetActive(false);
		}
		

	}
}
