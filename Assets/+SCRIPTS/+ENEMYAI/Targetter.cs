using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS._ENEMYAI
{
	public class Targetter : MonoBehaviour
	{
		private ICanAttack targetterLife => GetComponent<ICanAttack>();

		#region private functions

		public Life GetClosestEnemyInRange(float range)
		{
			var playersInRange = GetActualEnemyTargetsInRange(targetterLife.EnemyLayer, range);
			var closest = GetClosest(playersInRange);
			return closest;
		}

		private List<Life> GetActualEnemyTargetsInRange(LayerMask levelAssetsEnemyLayer, float range)
		{
			var enemiesInRange = GetValidTargetsInRange(levelAssetsEnemyLayer, range);
			var actualEnemiesInRange = enemiesInRange
			                           .Where(x => x != null && !x.IsDead() && x.transform.gameObject != gameObject && x.category == UnitCategory.Enemy)
			                           .ToList();
			return actualEnemiesInRange;
		}

		private Life GetClosest(List<Life> targets)
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

		private List<Life> GetPlayers()
		{
			var playersWithGOs = Services.playerManager.AllJoinedPlayers.Where(x => x.spawnedPlayerDefence != null).ToList();
			var playerLives = playersWithGOs.Select(x => x.spawnedPlayerDefence).Where(x => !x.IsDead()).ToList();

			return playerLives;
		}

		private List<Life> GetValidTargetsInRange(LayerMask layer, float range) =>
			Physics2D.OverlapCircleAll(transform.position, range, layer).Select(x => x.GetComponentInChildren<Life>())
			         .Where(life => life != null && TargetIsNotNullOrDead(life)).ToList();

		private List<Life> GetTargetsInRange(LayerMask layer, float range) => GetValidTargetsInRange(layer, range);

		private bool buildingIsInTheWay(Vector2 position)
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

		private bool TargetIsNotNullOrDead(Life target) => target != null && !target.IsDead();
	}
}
