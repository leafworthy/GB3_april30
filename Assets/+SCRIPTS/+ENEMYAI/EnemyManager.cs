using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class EnemyManager : MonoBehaviour, IService
	{
		List<Life> _allEnemies = new();
		public event Action<Player, float, Life> OnPlayerKillsEnemy;


		public void StartService()
		{
		}


		public GameObject SpawnNewEnemy(GameObject enemyPrefab, EnemySpawner.EnemyType enemyType, Vector3 position, int enemyTier)
		{
			var newEnemy = Services.objectMaker.Make(enemyPrefab, position);
			ConfigureNewEnemy(newEnemy, enemyType, enemyTier);
			return newEnemy;
		}

		public void SpawnNewNPC(GameObject NPCPrefab, Vector3 position)
		{
			var newNPC = Services.objectMaker.Make(NPCPrefab, position);
			ConfigureNewNPC(newNPC);
		}

		void CollectEnemy(GameObject enemy)
		{
			var enemyDefence = enemy.gameObject.GetComponent<Life>();

			if (enemyDefence == null) return;
			if (_allEnemies.Contains(enemyDefence)) return;
			enemyDefence.OnKilled += EnemyKilled;
			enemyDefence.OnDead += EnemyDead;
			_allEnemies.Add(enemyDefence);
		}

		void EnemyDead(Attack attack)
		{
			Services.lootTable.DropLoot(attack.DestinationLife.transform.position);
		}

		public void ClearEnemies()
		{
			foreach (var enemy in _allEnemies)
			{
				enemy.OnKilled -= EnemyKilled;
				enemy.OnDead -= EnemyDead;
			}

			_allEnemies.Clear();
		}

		void EnemyKilled(Player killer, Life life)
		{
			var experienceGained = DetermineExperienceGained(life);
			OnPlayerKillsEnemy?.Invoke(killer, experienceGained, life);
		}

		float DetermineExperienceGained(Life life)
		{
			var enemyStats = life.transform.GetComponent<Life>();
			if (enemyStats == null) return 0;
			return enemyStats.Stats.Data.experienceGiven;
		}

		public void ConfigureNewEnemy(GameObject enemy, EnemySpawner.EnemyType enemyType, int enemyTier = 0)
		{
			// Set up the Life component
			var life = enemy.GetComponent<Life>();
			if (life != null)
			{
				life.SetPlayer(Services.playerManager.enemyPlayer);
				life.SetEnemyTypeAndTier(enemyType, enemyTier);
			}

			CollectEnemy(enemy);
		}

		public void ConfigureNewNPC(GameObject npc)
		{
			var life = npc.GetComponent<Life>();
			if (life != null) life.SetPlayer(Services.playerManager.NPCPlayer);
			var npcScript = npc.GetComponent<NPC_AI>();
			npcScript.OnRescued += NPC_OnRescued;

			CollectEnemy(npc);
		}

		void NPC_OnRescued(NPC_AI npc)
		{
			var tinter = npc.gameObject.GetComponent<Tinter>();
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				Services.playerStatsManager.ChangeStat(player, PlayerStat.StatType.Rescues, 1);
			}

			tinter?.StartFadeOut();
		}
	}
}
