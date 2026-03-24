using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class HUDManager : MonoBehaviour, IService
	{
		List<HUDSlot> currentHUDSlots = new();
		public GameObject Vignette;

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
			ListenToEvents();
			DisableAllHUDSlots();
		}

		void ListenToEvents()
		{
			Services.levelManager.OnStartLevel += LevelSceneOnStartLevel;
			Services.levelManager.OnLevelSpawnedPlayerFromLevel += LevelSceneOnLevelSpawnedPlayerFromLevel;
			Services.levelManager.OnLevelSpawnedPlayerFromPlayerSetupMenu += LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu;
			Services.levelManager.OnStopLevel += Level_OnGameOver;
			Services.playerManager.OnPlayerJoins += PlayerOnPlayerJoins;
		}

		void StopListeningToEvents()
		{
			Services.levelManager.OnStartLevel -= LevelSceneOnStartLevel;
			Services.levelManager.OnLevelSpawnedPlayerFromLevel -= LevelSceneOnLevelSpawnedPlayerFromLevel;
			Services.levelManager.OnLevelSpawnedPlayerFromPlayerSetupMenu -= LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu;
			Services.levelManager.OnStopLevel -= Level_OnGameOver;
			Services.playerManager.OnPlayerJoins -= PlayerOnPlayerJoins;
		}

		void LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu(Player player)
		{
			ShowHUD();
		}

		void LevelSceneOnLevelSpawnedPlayerFromLevel(Player player)
		{
			Debug.Log("[HUD MANAGER] spawned player from level for player " + player.playerIndex);
			SetHUDSlotPlayer(player);
		}

		void Level_OnGameOver()
		{
			Vignette.SetActive(false);
			Services.playerManager.SetActionMaps(Players.UIActionMap);
			isInGame = false;
			DisableAllHUDSlots();
		}

		void OnDisable()
		{
			Debug.LogWarning("HUD DISABLED");

			Vignette?.SetActive(false);
			StopListeningToEvents();
		}

		void PlayerOnPlayerJoins(Player player)
		{
			if (!isInGame) return;
			Debug.Log("[HUD MANAGER] player joined while in game, set hud slot player for " + player.name);
			SetHUDSlotPlayer(player, true);
		}

		void LevelSceneOnStartLevel()
		{
			Debug.Log("hud on join level");
			ShowHUD();
			isInGame = true;
			Vignette.SetActive(true);
		}

		void SetHUDSlotPlayer(Player newPlayer, bool withMenu = false)
		{
			Debug.Log("set hud slot newPlayer " + newPlayer.name);
			if (newPlayer == null) return;
			var slot = newPlayer.currentHUDSlot ??= GetFirstOpenSlot();
			if (slot == null)
			{
				Debug.Log("slot null");
				return;
			}

			slot.gameObject.SetActive(true);

			if (withMenu)
				slot.OpenCharacterSetupMenu(newPlayer);
			else
				slot.OpenCharacterHUD(newPlayer);
		}

		HUDSlot GetFirstOpenSlot()
		{
			if (currentHUDSlots.Count == 0) currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();

			foreach (var hudSlot in currentHUDSlots.Where(hudSlot => hudSlot.currentHUDSlotState == HUDSlot.HUDSlotState.not))
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
				if (hudSlot == null) continue;
				hudSlot.gameObject.SetActive(false);
			}
		}

		public void SetBossLifeHealthbarVisible(bool value)
		{
			if (bossHealthbar == null) return;
			bossHealthbarGameObject.SetActive(value);
		}

		public void ResetHUD()
		{
			currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();
			foreach (var hudSlot in currentHUDSlots)
			{
				if (hudSlot == null) continue;
				hudSlot.ResetHUDSlot();
				hudSlot.gameObject.SetActive(false);
			}

			DisableAllHUDSlots();
			Vignette.SetActive(false);
			SetBossLifeHealthbarVisible(false);
		}
	}
}
