using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GangstaBean.UI.HUD
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
			Debug.Log("huds starts");
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
			Debug.Log("huds on game over");
		}

		private void OnDisable()
		{
			canJoin  = false;
			Players.I.OnPlayerJoins -= JoinInGame;
			Players.I.OnPlayerGetUpgrades -= OpenUpgradePanel;
			Players.SetActionMaps(Players.UIActionMap);
			Vignette.SetActive(false);
			Clock.SetActive(false);
			Debug.Log("hud stopped");
		}

	

		private void LevelScene_OnPlayerSpawned(Player player)
		{
			Debug.Log("set slot player  " + player.input.playerIndex);
			SetHUDSlotPlayer(player);
		}

		private void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			//gameObject.SetActive(true);
			Debug.Log("made it here");
			CreateHUDForPlayers(Players.AllJoinedPlayers);
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
				Debug.Log("set hud slot");
				SetHUDSlotPlayer(player);
			}
		}

		private void JoinInGame(Player player)
		{
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