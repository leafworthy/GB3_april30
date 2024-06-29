using System;
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

	

	private PlayerStatsHandler playerStatsHandler;
	public void SetPlayer(Player player)
	{
		owner = player;
		playerStatsHandler = owner.playerStats;
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
		Stat_Kills.text = playerStatsHandler.GetStatValue(PlayerStat.StatType.Kills).ToString();
		Stat_AttacksHit.text = playerStatsHandler.GetStatValue(PlayerStat.StatType.AttacksHit).ToString();
		Stat_AttacksTotal.text = playerStatsHandler.GetStatValue(PlayerStat.StatType.AttacksTotal).ToString();
		Stat_CashGained.text = playerStatsHandler.GetStatValue(PlayerStat.StatType.TotalCash).ToString();
		Stat_Accuracy.text = GetAccuracy().ToString();
	}

	private string GetAccuracy()
	{
		attacksTotal = playerStatsHandler.GetStatValue(PlayerStat.StatType.AttacksTotal);
		if (attacksTotal <= 0) return "%" + 0;
		attacksHit = playerStatsHandler.GetStatValue(PlayerStat.StatType.AttacksHit);
		return "%" + (attacksHit*100 / attacksTotal).ToString();
	}


}