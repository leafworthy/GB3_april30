using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats: MonoBehaviour 
{
	private Player owner;
	public List<PlayerStat> playerStats = new();
	private PlayerStat stat;
	private bool hasReset;
	public static event Action<Player,PlayerStat> OnPlayerStatChange;
	
	public void Start()
	{
		var player = GetComponent<Player>();
		if (player == null)
		{
			Debug.LogError("PlayerStats script must be attached to a Player object.");
		}
		owner = player;
		EnemyManager.OnPlayerDamagesEnemy += EnemiesOnPlayerDamagesEnemy;
		EnemyManager.OnPlayerKillsEnemy += EnemiesOnPlayerKillsEnemy;
		Attack.OnAnyAttack += PlayerAttacksOnAttack;
		LevelManager.OnStartLevel += (t) => ResetStats();
		ResetStats();
	}

	public void ResetStats()
	{
		if (hasReset) return;
		hasReset = true;
		playerStats.Clear();
		playerStats.Add(new PlayerStat(PlayerStat.StatType.Kills, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.Accuracy, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.AttacksHit, 0));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.AttacksTotal, 0));
		if (owner == null) return;
		playerStats.Add(new PlayerStat(PlayerStat.StatType.TotalCash, owner.GetStartingCash()));
		playerStats.Add(new PlayerStat(PlayerStat.StatType.Gas, owner.GetStartingGas()));
		
	}
	

	private void EnemiesOnPlayerKillsEnemy(Player player, Life life)
	{if (player != owner) return;
		
		ChangeStat(PlayerStat.StatType.Kills, 1);
	}

	private void PlayerAttacksOnAttack(Attack attack)
	{
		if (attack.Owner != owner) return;
		ChangeStat(PlayerStat.StatType.AttacksTotal, 1);
	}

	private void EnemiesOnPlayerDamagesEnemy(Attack attack)
	{
		ResetStats();
		ChangeStat(PlayerStat.StatType.AttacksHit, 1);
	}


	public float GetStatValue(PlayerStat.StatType statType)
	{
		ResetStats();
		var _stat = GetStat(statType);
		if(_stat != null)
		{
			Debug.Log("found stat");
			return _stat.GetStatAmount();
		}
		else
		{
			Debug.Log("didn't find stat");
			return 0;
		}
	}



	public void ChangeStat(PlayerStat.StatType type, float change)
	{
		ResetStats();
		var changingStat = GetStat(type);
		changingStat.ChangeStat(change);
		Debug.Log("change stat here");
		OnPlayerStatChange?.Invoke(owner, changingStat);
		
	}

	private PlayerStat GetStat(PlayerStat.StatType type)
	{
		ResetStats();
		stat = playerStats.FirstOrDefault(t => t.type == type);
		if(stat != null) Debug.Log("not null stat");
		return stat;
	}

	public void SetStatValue(PlayerStat.StatType statType, float value)
	{
		ResetStats();
		var changingStat = GetStat(statType);
		changingStat.SetStat(value); 
		OnPlayerStatChange?.Invoke(owner, changingStat);
	}
}