using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDManager : MonoBehaviour, IService
	{
		List<HUDSlot> currentHUDSlots = new();

		public GameObject Vignette;

		LevelManager _levelManager;
		LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();
		Players _playerManager;
		Players playerManager => _playerManager ?? ServiceLocator.Get<Players>();
		bool isInGame;

		public LineBar bossHealthbar;
		public GameObject bossHealthbarGameObject;

		public void HideHUD()
		{
			DisableAllHUDSlots();
		}

		public void ShowHUD()
		{
			gameObject.SetActive(true);
		}

		public void StartService()
		{
			Debug.Log("HUDmanager start service");
			Vignette.SetActive(false);
			levelManager.OnStartLevel += LevelSceneOnStartLevel;
			levelManager.OnLevelSpawnedPlayerFromLevel += LevelSceneOnLevelSpawnedPlayerFromLevel;
			levelManager.OnLevelSpawnedPlayerFromPlayerSetupMenu += LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu;
			levelManager.OnGameOver += Level_OnGameOver;
			levelManager.OnStopLevel += t => Level_OnGameOver();

			playerManager.OnPlayerJoins += PlayerOnPlayerJoins;
			DisableAllHUDSlots();
		}

		void LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu(Player player)
		{
			ShowHUD();
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
		}

		void LevelSceneOnLevelSpawnedPlayerFromLevel(Player player)
		{
			Debug.Log("[HUD MANAGER] spawned player from level for player " + player.playerIndex);
			//ShowHUD();
			//var playerStats = player.GetComponent<PlayerStats>();
			//if (playerStats != null) playerStats.ResetStats();
			SetHUDSlotPlayer(player);
		}

		void Level_OnGameOver()
		{
			Vignette.SetActive(false);
			playerManager.SetActionMaps(Players.UIActionMap);
			isInGame = false;
			DisableAllHUDSlots();
		}

		void OnDisable()
		{
			Debug.LogWarning("HUD DISABLED");

			Vignette?.SetActive(false);
		}

		void PlayerOnPlayerJoins(Player player)
		{
			if (!isInGame) return;
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
			Debug.Log("[HUD MANAGER] player joined while in game, set hud slot player for " + player.name);
			SetHUDSlotPlayer(player, true);
		}

		void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			Debug.Log("hud on join level");
			ShowHUD();
			isInGame = true;
			Vignette.SetActive(true);
		}

		void SetHUDSlotPlayer(Player player, bool withMenu = false)
		{
			Debug.Log("set hud slot player " + player.name);
			if (player == null) return;
			var slot = GetFirstOpenSlot();
			if (slot == null)
			{
				Debug.Log("slot null");
				return;
			}

			slot.gameObject.SetActive(true);

			if (withMenu)
				slot.OpenCharacterSelectMenu(player);
			else
				slot.SetPlayer(player);
		}

		HUDSlot GetFirstOpenSlot()
		{
			if (currentHUDSlots.Count == 0) currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();

			foreach (var hudSlot in currentHUDSlots.Where(hudSlot => !hudSlot.IsActive))
			{
				return hudSlot;
			}

			Debug.Log("no open hud slot");

			return null;
		}

		void DisableAllHUDSlots()
		{
			currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();
			foreach (var hudSlot in currentHUDSlots)
			{
				hudSlot.gameObject.SetActive(false);
			}
		}

		public void SetBossLifeHealthbarVisible(bool value)
		{
			if (bossHealthbar == null) return;
			bossHealthbarGameObject.SetActive(value);
		}
	}
}
