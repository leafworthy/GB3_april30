using TMPro;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerStatsDisplay : MonoBehaviour
	{
		Player owner;
		public TextMeshProUGUI PlayerName;
		public TextMeshProUGUI Stat_Kills;
		public TextMeshProUGUI Stat_CashGained;
		public TextMeshProUGUI Stat_Rescues;

		HideRevealObjects hideRevealObjects;

		PlayerStats playerStats;

		public void SetPlayer(Player player)
		{
			owner = player;
			PlayerName.text = "player " + (owner.playerIndex + 1);
			playerStats = owner.GetComponent<PlayerStats>();
			if (playerStats == null) return;
			DisplayStats();
			DisplayCharacter();
		}

		void DisplayCharacter()
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

		void DisplayStats()
		{
			Stat_Kills.text = Services.playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.Kills).ToString();
			Stat_CashGained.text = Services.playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.TotalCash).ToString();
			Stat_Rescues.text = Services.playerStatsManager.GetStatAmount(owner, PlayerStat.StatType.Rescues).ToString();
		}
	}
}
