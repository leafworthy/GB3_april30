using __SCRIPTS.UpgradeS;
using TMPro;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerStatsDisplay : ServiceUser
	{
		private Player owner;
		public TextMeshProUGUI PlayerName;
		public TextMeshProUGUI Stat_Kills;
		public TextMeshProUGUI Stat_Time;
		public TextMeshProUGUI Stat_CashGained;
		public TextMeshProUGUI Stat_Gas;
		public TextMeshProUGUI Stat_Upgrades;

		private HideRevealObjects hideRevealObjects;


		private PlayerStats playerStats;

		public void SetPlayer(Player player)
		{
			owner = player;
			PlayerName.text = "Player " + (owner.playerIndex +1);
			playerStats = owner.GetComponent<PlayerStats>();
			if (playerStats == null) return;
			DisplayStats();
			DisplayCharacter();
		}

		private void DisplayCharacter()
		{
			hideRevealObjects = GetComponentInChildren<HideRevealObjects>();
			switch (owner.CurrentCharacter)
			{
				case Character.Karrot:
					hideRevealObjects.Set(0);
					break;
				case Character.Bean:
					hideRevealObjects.Set(0);
					break;
				case Character.Brock:

					hideRevealObjects.Set(1);
					break;
				case Character.Tmato:

					hideRevealObjects.Set(2);
					break;
			}
		}

		private void DisplayStats()
		{
			Stat_Kills.text = playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.Kills).ToString();
			Stat_Time.text = playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.TimeSurvived).ToString("F1");
			Stat_CashGained.text = playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.TotalCash).ToString();
			Stat_Gas.text = playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.Gas).ToString();
			//Stat_Upgrades.text = owner.GetComponent<PlayerUpgrades>().GetTotalUpgradesPurchased().ToString();
		}


	}
}
