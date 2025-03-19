using TMPro;
using UnityEngine;

public class PlayerStatsDisplay : MonoBehaviour
{
	private Player owner;
	public TextMeshProUGUI Stat_Kills;
	public TextMeshProUGUI Stat_AttacksHit;
	public TextMeshProUGUI Stat_AttacksTotal;
	public TextMeshProUGUI Stat_CashGained;
	public TextMeshProUGUI Stat_Accuracy;

	private HideRevealObjects hideRevealObjects;

	private float attacksTotal;
	private float attacksHit;

	private PlayerStats playerStats;

	public void SetPlayer(Player player)
	{
		owner = player;
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

				hideRevealObjects.Set(1);
				break;
		}
	}

	private void DisplayStats()
	{
		Stat_Kills.text = PlayerStatsManager.I.GetStatAmount(owner, PlayerStat.StatType.Kills).ToString();
		Stat_AttacksHit.text = PlayerStatsManager.I.GetStatAmount(owner, PlayerStat.StatType.AttacksHit).ToString();
		Stat_AttacksTotal.text = PlayerStatsManager.I.GetStatAmount(owner, PlayerStat.StatType.AttacksTotal).ToString();
		Stat_CashGained.text = PlayerStatsManager.I.GetStatAmount(owner, PlayerStat.StatType.TotalCash).ToString();
		Stat_Accuracy.text = GetAccuracy();
	}

	private string GetAccuracy()
	{
		attacksTotal = PlayerStatsManager.I.GetStatAmount(owner, PlayerStat.StatType.AttacksTotal);
		if (attacksTotal <= 0) return "%" + 0;
		attacksHit = PlayerStatsManager.I.GetStatAmount(owner, PlayerStat.StatType.AttacksHit);
		return "%" + (attacksHit * 100 / attacksTotal);
	}
}