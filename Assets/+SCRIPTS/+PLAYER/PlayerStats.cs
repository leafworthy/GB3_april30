using System;
using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerStats : MonoBehaviour
	{
		Player owner;
		public List<PlayerStat> playerStats = new();
		PlayerStat stat;
		bool hasInit;
		public event Action OnStatsReset;
		public event Action<Player, PlayerStat> OnPlayerStatChange;
		LevelManager _levelManager;
		LevelManager levelManager => _levelManager ?? ServiceLocator.Get<LevelManager>();
		EnemyManager _enemyManager;
		EnemyManager enemyManager => _enemyManager ?? ServiceLocator.Get<EnemyManager>();

		public void Start()
		{
			var player = GetComponent<Player>();
			owner = player;
			enemyManager.OnPlayerKillsEnemy += EnemiesOnPlayerKillsEnemy;
			levelManager.OnStartLevel += t => ResetStats();
			InitStats();
		}

		public void InitStats()
		{
			if (hasInit) return;

			hasInit = true;
			ResetStats();
		}

		void AddStat(PlayerStat _stat)
		{
			if (playerStats.Contains(_stat)) return;
			playerStats.Add(_stat);
			OnPlayerStatChange?.Invoke(owner, _stat);
		}

		public void ResetStats()
		{
			if (owner == null) return;
			playerStats.Clear();

			AddStat(new PlayerStat(PlayerStat.StatType.Kills, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.TimeSurvived, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.Accuracy, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.AttacksHit, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.AttacksTotal, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.TotalCash, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.Gas, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.Experience, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.Level, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.Rescues, 0));

			OnStatsReset?.Invoke();
		}

		void EnemiesOnPlayerKillsEnemy(Player player, float experienceGained, IGetAttacked life)
		{
			if (player != owner) return;

			ChangeStat(PlayerStat.StatType.Kills, 1);
			ChangeStat(PlayerStat.StatType.Experience, experienceGained);
		}

		public float GetStatValue(PlayerStat.StatType statType)
		{
			InitStats();
			var _stat = GetStat(statType);
			if (_stat != null) return _stat.GetStatAmount();

			return 0;
		}

		public void ChangeStat(PlayerStat.StatType type, float change)
		{
			InitStats();
			var changingStat = GetStat(type);
			changingStat.IncreaseStat(change);
			OnPlayerStatChange?.Invoke(owner, changingStat);
		}

		PlayerStat GetStat(PlayerStat.StatType type)
		{
			InitStats();
			stat = playerStats.FirstOrDefault(t => t.type == type);
			return stat;
		}

		public void SetStatValue(PlayerStat.StatType statType, float value)
		{
			InitStats();
			var changingStat = GetStat(statType);
			changingStat.SetStat(value);
			OnPlayerStatChange?.Invoke(owner, changingStat);
		}
	}
}
