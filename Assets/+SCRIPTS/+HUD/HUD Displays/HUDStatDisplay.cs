using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDStatDisplay : MonoBehaviour, INeedPlayer
	{
		public PlayerStat.StatType statType;
		public TMP_Text displayText;
		Player owner;
		public bool hasMax;
		PlayerStats playerStats;

		float CurrentAmount => Services.playerStatsManager.GetStatAmount(owner, statType);

		void OnDisable()
		{
			StopListeningToEvents();
		}

		public void SetPlayer(Player newPlayer)
		{
			owner = newPlayer;
			playerStats = owner.GetComponent<PlayerStats>();
			if (playerStats == null) return;

			ListenToEvents();
			UpdateDisplay();
		}

		void ListenToEvents()
		{
			if (playerStats != null)
			{
				playerStats.OnPlayerStatChange += Players_PlayerStatChange;
				playerStats.OnStatsReset += UpdateDisplay;
			}

			Services.levelManager.OnStartLevel += UpdateDisplay;
			Services.levelManager.OnLevelSpawnedPlayerFromLevel += UpdateDisplay;
		}

		void StopListeningToEvents()
		{
			if (playerStats != null)
			{
				playerStats.OnPlayerStatChange -= Players_PlayerStatChange;
				playerStats.OnStatsReset -= UpdateDisplay;
			}

			Services.levelManager.OnStartLevel -= UpdateDisplay;
			Services.levelManager.OnLevelSpawnedPlayerFromLevel -= UpdateDisplay;
		}

		void UpdateDisplay(Player obj)
		{
			UpdateDisplay();
		}

		void Players_PlayerStatChange(Player player, PlayerStat playerStat)
		{
			UpdateDisplay();
		}

		void UpdateDisplay()
		{
			if (owner == null) return;

			var currentAmount = CurrentAmount;

			if (currentAmount < 0) currentAmount = 0;

			if (hasMax)
				displayText.text = currentAmount + "/" + Services.playerStatsManager.MaxRescues;
			else
				displayText.text = currentAmount.ToString();
		}
	}
}
