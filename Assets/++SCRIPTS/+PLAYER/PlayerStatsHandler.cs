using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerStatsHandler 
{
	private Player owner;
	private List<PlayerStat> playerStats = new();
	private PlayerStat stat;

	public static event Action<Player,PlayerStat> OnPlayerStatChange;
	
	public PlayerStatsHandler(Player player)
	{
		owner = player;
		EnemyManager.OnPlayerDamagesEnemy += EnemiesOnPlayerDamagesEnemy;
		EnemyManager.OnPlayerKillsEnemy += EnemiesOnPlayerKillsEnemy;
		Attack.OnAnyAttack += PlayerAttacksOnAttack;
		LevelGameScene.OnStart += () => ResetStats(owner);
		ResetStats(owner);
	}


	private void ResetStats(Player player)
	{
		playerStats.Add(new PlayerStat(PlayerStat.StatType.Kills, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.Accuracy, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.AttacksHit, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.AttacksTotal, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.TotalCash, player.GetStartingCash()));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.Gas, player.GetStartingGas()));
		
	}
	

	private void EnemiesOnPlayerKillsEnemy(Player player, Life life)
	{
		if (player != owner) return;
		ChangeStat(PlayerStat.StatType.Kills, 1);
	}

	private void PlayerAttacksOnAttack(Attack attack)
	{
		if (attack.Owner != owner) return;
		ChangeStat(PlayerStat.StatType.AttacksTotal, 1);
	}

	private void EnemiesOnPlayerDamagesEnemy(Attack attack)
	{
		ChangeStat(PlayerStat.StatType.AttacksHit, 1);
	}


	public float GetStatValue(PlayerStat.StatType statType)
	{
		return GetStat(statType).value;
	}



	public void ChangeStat(PlayerStat.StatType type, float change)
	{
		var changingStat = GetStat(type);
		changingStat.ChangeStat(change);
		OnPlayerStatChange?.Invoke(owner, changingStat);
		
	}

	private PlayerStat GetStat(PlayerStat.StatType type)
	{
		stat = playerStats.FirstOrDefault(t => t.type == type);
		return stat;
	}
	
}