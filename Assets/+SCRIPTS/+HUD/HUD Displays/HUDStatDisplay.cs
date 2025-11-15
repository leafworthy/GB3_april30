using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{

	public class HUDStatDisplay : MonoBehaviour, INeedPlayer
	{
		public PlayerStat.StatType statType;
		public TMP_Text displayText;
		public GameObject statIcon;
		private Player owner;
		public bool hasMax;

		private float CurrentAmount => Services.playerStatsManager.GetStatAmount(owner, statType);

		private void Start()
		{
			Services.levelManager.OnStartLevel += (t) => UpdateDisplay();
			Services.levelManager.OnLevelSpawnedPlayerFromLevel += (t) => UpdateDisplay();
		}


		public void SetPlayer(Player newPlayer)
		{
			owner = newPlayer;
			var playerStats = owner.GetComponent<PlayerStats>();
			if(playerStats == null)
			{

				return;
			}

			playerStats.OnPlayerStatChange += Players_PlayerStatChange;
			playerStats.OnStatsReset += UpdateDisplay;
			UpdateDisplay();
		}

		private void Players_PlayerStatChange(Player player, PlayerStat playerStat)
		{
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			if (owner == null) return;

			var currentAmount = CurrentAmount;
			// Don't display negative values (which indicate uninitialized stats)
			if (currentAmount < 0) currentAmount = 0;

			if(hasMax)
			{
				displayText.text = currentAmount.ToString() +"/" + Services.playerStatsManager.MaxGas.ToString();
			}
			else
			{
				displayText.text = currentAmount.ToString();
			}

		}
	}
}
