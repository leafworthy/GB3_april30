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
		private void Start()
		{
			Vignette.SetActive(false);
			Clock.SetActive(false);
			Debug.Log("huds starts");
			LevelManager.OnStartLevel += LevelSceneOnStartLevel;
			LevelManager.OnPlayerSpawned += LevelScene_OnPlayerSpawned;
			LevelManager.OnGameOver += Level_OnGameOver;
			LevelManager.OnWinGame += Level_OnGameOver;
			LevelManager.OnStopLevel += LevelSceneOnStopLevel;
			
			DisableAllHUDSlots();
			
		}

	
	
		private void Level_OnGameOver()
		{
			Vignette.SetActive(false);
			Clock.SetActive(false);
			canJoin = false;
			Players.SetActionMaps(Players.UIActionMap);
			DisableAllHUDSlots();
			gameObject.SetActive(false);
		}

		private void LevelSceneOnStopLevel(GameLevel gameLevel)
		{
			LevelManager.OnStopLevel -= LevelSceneOnStopLevel;
			canJoin  = false;
			Players.OnPlayerJoins -= JoinInGame;
			Players.OnPlayerGetUpgrades -= OpenUpgradePanel;
			Vignette.SetActive(false);
			Clock.SetActive(false);
			DisableAllHUDSlots();
		}

	

		private void LevelScene_OnPlayerSpawned(Player player)
		{
			SetHUDSlotPlayer(player);
		}

		private void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			Debug.Log("made it here");
			CreateHUDForPlayers(Players.AllJoinedPlayers);
			Players.OnPlayerJoins += JoinInGame;
			Players.OnPlayerGetUpgrades += OpenUpgradePanel;
			Vignette.SetActive(true);
			Clock.SetActive(true);
			canJoin = true;
		}

		private void OpenUpgradePanel(Player player)
		{
			var slot = I.currentHUDSlots[(int)player.input.playerIndex];
			slot.gameObject.SetActive(true);
			slot.StartUpgradeSelectMenu(player);
		}


		private void CreateHUDForPlayers(List<Player> players)
		{
			foreach (var player in players)
			{
				Debug.Log("set hud slot");
				SetHUDSlotPlayer(player);
			}
		}

		private void JoinInGame(Player player)
		{
			//if (!LevelManager.I.IsInGame) return;
			if(!canJoin) return;
			Debug.Log("join in game");
			
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