using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerStats: MonoBehaviour
	{
		private Player owner;
		public List<PlayerStat> playerStats = new();
		private PlayerStat stat;
		private bool hasInit;
		public  event Action OnStatsReset;
		public  event Action<Player,PlayerStat> OnPlayerStatChange;

		public void Start()
		{
			var player = GetComponent<Player>();
			if (player == null)
			{

			}
			owner = player;
			EnemyManager.I.OnPlayerKillsEnemy += EnemiesOnPlayerKillsEnemy;
			LevelManager.I.OnStartLevel += (t) => ResetStats();
			InitStats();
		}

		public void InitStats()
		{
			if (hasInit) return;

			hasInit = true;
		ResetStats();
		}

		private void AddStat(PlayerStat _stat)
		{
			if (playerStats.Contains(_stat))
			{

				return;
			}
			playerStats.Add(_stat);
			OnPlayerStatChange?.Invoke(owner, _stat);
		}
		public void ResetStats()
		{
			playerStats.Clear();

			AddStat(new PlayerStat(PlayerStat.StatType.Kills, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.TimeSurvived, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.Accuracy, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.AttacksHit, 0));
			AddStat(new PlayerStat(PlayerStat.StatType.AttacksTotal, 0));
			if (owner == null) return;
			AddStat(new PlayerStat(PlayerStat.StatType.TotalCash, owner.GetStartingCash()));
			AddStat(new PlayerStat(PlayerStat.StatType.Gas, 0));

			OnStatsReset?.Invoke();
		}


		private void EnemiesOnPlayerKillsEnemy(Player player, Life life)
		{if (player != owner) return;

			ChangeStat(PlayerStat.StatType.Kills, 1);
		}






		public float GetStatValue(PlayerStat.StatType statType)
		{
			InitStats();
			var _stat = GetStat(statType);
			if(_stat != null)
			{
				return _stat.GetStatAmount();
			}
			else
			{
				return 0;
			}
		}



		public void ChangeStat(PlayerStat.StatType type, float change)
		{
			InitStats();
			var changingStat = GetStat(type);
			changingStat.IncreaseStat(change);
			OnPlayerStatChange?.Invoke(owner, changingStat);

		}

		private PlayerStat GetStat(PlayerStat.StatType type)
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
