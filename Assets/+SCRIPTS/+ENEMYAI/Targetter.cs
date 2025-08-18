using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GangstaBean.Core;

namespace __SCRIPTS._ENEMYAI
{
	public class Targetter : ServiceUser, IPoolable
	{
		private void Awake()
		{
			InitializeTargetter();
		}

		private void InitializeTargetter()
		{
			wanderPoint = transform.position;
		}

		private Life targetterLife => GetComponent<Life>();
		public Vector3 WanderPoint
		{
			get => wanderPoint;
			private set => wanderPoint = value;
		}
		public float WanderRadius = 50;
		private Life specialTarget;
		private Vector3 wanderPoint;

		#region private functions

		private Life GetClosest(List<Life> targets)
		{
			Life closest = null;
			float minDistance = float.MaxValue;
			// Cache transform.position
			Vector2 pos = transform.position;

			for (int i = 0; i < targets.Count; i++)
			{
				Life target = targets[i];
				// Use SqrMagnitude to avoid the square root cost
				float distance = Vector2.SqrMagnitude(target.transform.position - (Vector3)pos);
				if (distance < minDistance)
				{
					minDistance = distance;
					closest = target;
				}
			}

			return closest;
		}

		//private Life GetClosest(List<Life> targets) => targets.OrderBy(t => Vector2.Distance(t.transform.position, transform.position)).FirstOrDefault();
		public Life GetClosestAttackableObstacle()
		{
			var obstacles = GetValidObstaclesInRange( AssetManager.LevelAssets.DoorLayer, targetterLife.PrimaryAttackRange);
			var closest = GetClosest(obstacles);

			// Debug logging to help identify door targeting issues
			if (obstacles.Count > 0)
			{
			}

			return closest;
		}
		public Life GetClosestAttackablePlayer() => GetClosest(GetAttackableTargetsInRange( AssetManager.LevelAssets.PlayerLayer, targetterLife.PrimaryAttackRange));
		public Life GetClosestPlayerInAggroRange() => GetClosest(GetAggroTargets( AssetManager.LevelAssets.PlayerLayer));
		public Life GetClosestPlayer() => GetClosest(GetPlayers());

		private List<Life> GetPlayers()
		{
			var playersWithGOs = playerManager.AllJoinedPlayers.Where(x => (x.spawnedPlayerLife != null)).ToList();
			var playerLives = playersWithGOs.Select(x => x.spawnedPlayerLife).Where(x => !x.IsDead()).ToList();


			return playerLives;
		}

		private List<Life> GetValidTargetsInRange(LayerMask layer, float range) =>
			Physics2D.OverlapCircleAll(transform.position, range, layer).Select(x => x.GetComponentInChildren<Life>())
			         .Where(life => life != null && TargetIsValid(life)).ToList();


		private List<Life> GetValidObstaclesInRange(LayerMask layer, float range)
		{
			var colliders = Physics2D.OverlapCircleAll(transform.position, range, layer);
			var validObstacles = new List<Life>();

			foreach (var collider in colliders)
			{
				// Try multiple ways to find Life component on door objects
				var life = collider.GetComponentInChildren<Life>();
				if (life == null) life = collider.GetComponent<Life>();
				if (life == null) life = collider.GetComponentInParent<Life>();

				if (life != null && ObstacleIsValid(life))
				{
					// Additional validation: ensure we can actually reach the door
					if (CanReachObstacle(life))
					{
						validObstacles.Add(life);
					}
				}
			}

			return validObstacles;
		}

		private bool CanReachObstacle(Life obstacle)
		{
			// Simple distance check - door should be reachable within attack range
			float distance = Vector2.Distance(transform.position, obstacle.transform.position);
			return distance <= targetterLife.PrimaryAttackRange;
		}
		private List<Life> GetAttackableTargetsInRange(LayerMask layer, float range) =>
			Physics2D.OverlapCircleAll(transform.position, range, layer).Select(x => x.GetComponentInChildren<Life>())
			         .Where(life => life != null && TargetIsValid(life) && HasLineOfSightWith(life.transform.position)).ToList();

		private List<Life> GetAggroTargets(LayerMask layer) => GetValidTargetsInRange(layer, targetterLife.AggroRange);

		private bool buildingIsInTheWay(Vector2 position)
		{
			var hit = Physics2D.Linecast(transform.position, position, AssetManager.LevelAssets.EnemyUnwalkableLayers);
			if (!hit) return false;
			return hit.collider != null;
		}

		private bool isWithinAttackRange(Vector3 target) => Vector3.Distance(transform.position, target) < targetterLife.PrimaryAttackRange;

		#endregion

		#region public functions

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

		#endregion

		public bool HasLineOfSightWith(Vector3 transformPosition) => !buildingIsInTheWay(transformPosition);

		private bool CanAttack(Life target)
		{
			if (!TargetIsValid(target)) return false;

			if (!isWithinAttackRange(target.transform.position)) return false;

			return HasLineOfSightWith(target.transform.position);
		}

		private bool TargetIsValid(Life target)
		{
			if (target != null && !target.IsDead()) return true;
			return false;
		}

		private bool ObstacleIsValid(Life target)
		{
			if (target == null || target.IsDead())
			{
				return false;
			}

			if (!target.IsObstacle)
			{
				return false;
			}

			// Try multiple ways to find the door component for robustness
			var door = target.GetComponentInParent<DoorInteraction>();
			if (door == null) door = target.GetComponent<DoorInteraction>();
			if (door == null) door = target.GetComponentInChildren<DoorInteraction>();

			if (door == null)
			{
				// TODO: Replace with structured logging
				// Debug.LogWarning($"{gameObject.name}: Found obstacle {target.name} but no DoorInteraction component");
				return false;
			}

			if (door.isBroken)
			{
				return false;
			}

			if (door.isOpen)
			{
				return false;
			}

			return true;
		}

		public bool FoundTargetInAggroRange()
		{
			var target = GetClosestPlayerInAggroRange();
			return target != null;
		}

		public void OnPoolSpawn()
		{
			// Reinitialize targetter when spawned from pool
			InitializeTargetter();
		}

		public void OnPoolDespawn()
		{
			// Clean up when returning to pool
			specialTarget = null;
		}
	}
}
