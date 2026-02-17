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

		public LineBar bossHealthbar;
		public GameObject bossHealthbarGameObject;

		public void StartService()
		{
			Vignette.SetActive(false);
			levelManager.OnStartLevel += LevelSceneOnStartLevel;
			levelManager.OnLevelSpawnedPlayerFromLevel += LevelSceneOnLevelSpawnedPlayerFromLevel;
			levelManager.OnLevelSpawnedPlayerFromPlayerSetupMenu += LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu;
			levelManager.OnGameOver += Level_OnGameOver;
			levelManager.OnWinGame += Level_OnGameOver;
			levelManager.OnStopLevel += t => Level_OnGameOver();

			playerManager.OnPlayerJoins += PlayerOnPlayerJoins;
			DisableAllHUDSlots();
		}

		private void LevelSceneOnLevelSpawnedPlayerFromPlayerSetupMenu(Player player)
		{
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
		}

		private void LevelSceneOnLevelSpawnedPlayerFromLevel(Player player)
		{
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
			SetHUDSlotPlayer(player);
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
			levelManager.OnLevelSpawnedPlayerFromLevel -= LevelSceneOnLevelSpawnedPlayerFromLevel;
			playerManager.OnPlayerJoins -= PlayerOnPlayerJoins;
			levelManager.OnGameOver -= Level_OnGameOver;
			levelManager.OnWinGame -= Level_OnGameOver;

			Vignette.SetActive(false);
		}

		private void PlayerOnPlayerJoins(Player player)
		{
			if (!isInGame) return;
			var playerStats = player.GetComponent<PlayerStats>();
			if (playerStats != null) playerStats.ResetStats();
			SetHUDSlotPlayer(player, true);

		}



		private void LevelSceneOnStartLevel(GameLevel gameLevel)
		{
			isInGame = true;
			Vignette.SetActive(true);
		}

		private void SetHUDSlotPlayer(Player player, bool withMenu = false)
		{

			if (player == null) return;
			var slot = GetFirstOpenSlot();
			if(slot == null)
			{
				return;
			}
			slot.gameObject.SetActive(true);

			if (withMenu)
				slot.OpenCharacterSelectMenu(player);
			else
				slot.SetPlayer(player);
		}

		private HUDSlot GetFirstOpenSlot()
		{
			if (currentHUDSlots.Count == 0)
			{
				currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();
			}

			foreach (var hudSlot in currentHUDSlots.Where(hudSlot => !hudSlot.IsActive))
			{
				return hudSlot;
			}


			return null;
		}

		private void DisableAllHUDSlots()
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
