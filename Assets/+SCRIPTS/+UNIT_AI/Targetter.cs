using System.Collections.Generic;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class Targetter : MonoBehaviour
	{
		ICanAttack targetterLife => _targetterLife ??= GetComponent<ICanAttack>();
		ICanAttack _targetterLife;

		// Reused across queries to avoid per-call allocations in the targeting hot path
		// (this runs per-enemy every FixedUpdate via SimpleEnemyAI / CrimsonAI / NPC_AI).
		readonly List<Collider2D> _overlapResults = new();
		ContactFilter2D _overlapFilter;

		#region private functions

		public Life GetClosestEnemyInRange(float range)
		{
			var playersInRange = GetActualEnemyTargetsInRange(targetterLife.EnemyLayer, range);
			var closest = GetClosest(playersInRange);
			return closest;
		}

		List<Life> GetActualEnemyTargetsInRange(LayerMask levelAssetsEnemyLayer, float range)
		{
			var enemiesInRange = GetValidTargetsInRange(levelAssetsEnemyLayer, range);
			var actualEnemiesInRange = new List<Life>();
			for (var i = 0; i < enemiesInRange.Count; i++)
			{
				var x = enemiesInRange[i];
				if (x != null && !x.IsDead() && x.transform.gameObject != gameObject && x.category == UnitCategory.Enemy)
					actualEnemiesInRange.Add(x);
			}

			return actualEnemiesInRange;
		}

		Life GetClosest(List<Life> targets)
		{
			Life closest = null;
			var minDistance = float.MaxValue;

			foreach (var target in targets)
			{
				var distance = Vector2.SqrMagnitude(target.transform.position - transform.position);
				if (!(distance < minDistance)) continue;
				minDistance = distance;
				closest = target;
			}

			return closest;
		}

		public Life GetClosestPlayerWithinRange(float range) => GetClosest(GetTargetsInRange(Services.assetManager.LevelAssets.PlayerLayer, range));
		public Life GetClosestPlayer() => GetClosest(GetPlayers());

		List<Life> GetPlayers()
		{
			var playerLives = new List<Life>();
			var allPlayers = Services.playerManager.AllJoinedPlayers;
			for (var i = 0; i < allPlayers.Count; i++)
			{
				var defence = allPlayers[i].spawnedPlayerDefence;
				if (defence != null && !defence.IsDead())
					playerLives.Add(defence);
			}

			return playerLives;
		}

		List<Life> GetValidTargetsInRange(LayerMask layer, float range)
		{
			// Match OverlapCircleAll's default behaviour (respect the global trigger query setting).
			_overlapFilter.useTriggers = Physics2D.queriesHitTriggers;
			_overlapFilter.useLayerMask = true;
			_overlapFilter.layerMask = layer;
			Physics2D.OverlapCircle(transform.position, range, _overlapFilter, _overlapResults);

			var validTargets = new List<Life>();
			for (var i = 0; i < _overlapResults.Count; i++)
			{
				var life = _overlapResults[i].GetComponentInChildren<Life>();
				if (life != null && TargetIsNotNullOrDead(life))
					validTargets.Add(life);
			}

			return validTargets;
		}

		List<Life> GetTargetsInRange(LayerMask layer, float range) => GetValidTargetsInRange(layer, range);

		bool buildingIsInTheWay(Vector2 position)
		{
			var hit = Physics2D.Linecast(transform.position, position, Services.assetManager.LevelAssets.EnemyUnwalkableLayers);
			if (!hit) return false;
			return hit.collider != null;
		}

		#endregion

		public Vector2 GetWanderPosition(Vector2 wanderPoint, float wanderDistance)
		{
			var maxTries = 30;
			for (var i = 0; i < maxTries; i++)
			{
				var point = wanderPoint + Random.insideUnitCircle * wanderDistance;
				if (!buildingIsInTheWay(point)) return point;
			}

			return wanderPoint;
		}

		public bool HasLineOfSightWith(Vector3 transformPosition) => !buildingIsInTheWay(transformPosition);

		bool TargetIsNotNullOrDead(Life target) => target != null && !target.IsDead();
	}
}
