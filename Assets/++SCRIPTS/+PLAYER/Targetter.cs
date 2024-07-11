using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Targetter : MonoBehaviour
{
	private Life targetterLife => GetComponent<Life>();
	private Life specialTarget;
	private Life currentTarget;
	private Life currentObstacle;

	private List<Life> potentialObjectTargets = new();
	private List<Life> potentialObstacles = new();

	#region unity events

	private void Update()
	{
		currentTarget = GetClosestTargetWithinAggroRange(transform.position);
		currentObstacle = GetClosestObstacleWithinAttackRange();
	}

	private void OnDrawGizmos()
	{
		DrawAttackRange();
		DrawAggroRange();
		DrawCurrentTarget();
		DrawCurrentObstacle();
	}

	private void DrawCurrentTarget()
	{
		if (currentTarget == null) return;
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, currentTarget.transform.position);
	}

	private void DrawCurrentObstacle()
	{
		if (currentObstacle == null) return;
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, currentObstacle.transform.position);
	}

	private void DrawAttackRange()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, targetterLife.AttackRange);
	}

	private void DrawAggroRange()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, targetterLife.AggroRange);
	}

	#endregion

	#region private functions

	private List<Life> GetTargetsWithinAggroRange(LayerMask layer)
	{
		var list = Physics2D.OverlapCircleAll(transform.position, targetterLife.AggroRange, layer)?.ToList();
		return list?.Select(x => x.GetComponentInChildren<Life>()).ToList();
	}

	private Life GetClosestTargetTo(List<Life> targets, Vector2 position)
	{
		if (targets.Count <= 0) return null;
		var closest = targets[0];
		foreach (var target in targets)
		{
			if (target.GetComponentInChildren<ZombieAttractor>() == null) continue;
			var life = target.GetComponentInChildren<Life>();
			if (life == null) continue;
			if (life.IsDead()) continue;

			closest = GetClosest(position, target, closest);
		}

		return closest.GetComponentInChildren<Life>();
	}

	private static Life GetClosest(Vector2 position, Life target, Life closest)
	{
		var distance = Vector3.Distance(target.transform.position, position);
		var currentClosestDistance = Vector3.Distance(closest.transform.position, position);
		if (distance < currentClosestDistance) closest = target;
		return closest;
	}

	private Life GetClosestObstacleTo(List<Life> targets, Vector2 position)
	{
		if (targets.Count <= 0) return null;
		var closest = null as Life;
		var currentClosestDistance = float.MaxValue;
		foreach (var target in targets)
		{
			if(target == null) continue;
			
			var go = target.gameObject;
			Debug.DrawLine(position, target.transform.position, Color.gray);
			var door = go.GetComponentInParent<DoorInteraction>();
			if (door == null) continue;
			if (!doorIsValidTarget(door)) continue;			
			//if (buildingIsInTheWay(doorLife.transform.position)) continue;

			var distance = Vector3.Distance(door.transform.position, position);
			if (distance < currentClosestDistance)
			{
				closest = target;
				Debug.DrawLine(position, closest.transform.position, Color.magenta, .5f);
			}
		}

		if(closest != null)Debug.DrawLine(position, closest.transform.position, Color.white, .5f);

		return closest;
	}

	private static bool doorIsValidTarget(DoorInteraction doorInteractionAnimator) =>
		doorInteractionAnimator != null && !doorInteractionAnimator.isBroken && !doorInteractionAnimator.isOpen;



	private bool buildingIsInTheWay(Vector2 position)
	{
		var hit = Physics2D.Linecast(transform.position, position, ASSETS.LevelAssets.EnemyUnwalkableLayers);
		if (!hit) return false;
		return hit.collider != null;
	}

	private bool isWithinAttackRange(Vector3 target) => Vector3.Distance(transform.position, target) < targetterLife.AttackRange;

	#endregion

	#region public functions

	public Life GetClosestTargetWithinAggroRange(Vector2 position)
	{
		potentialObjectTargets.Clear();
		var potentialTargets = Physics2D.OverlapCircleAll(position, targetterLife.AggroRange, ASSETS.LevelAssets.PlayerLayer).ToList();
		foreach (var player in potentialTargets)
		{
			var playerLife = player.GetComponentInChildren<Life>();
			if (playerLife == null) continue;
			potentialObjectTargets.Add(playerLife);
		}

		potentialObjectTargets.AddRange(ZombieWaves.I.FindAllZombieAttractors());
		return potentialObjectTargets.Count <= 0 ? null : GetClosestTargetTo(potentialObjectTargets, position);
	}

	public Life GetClosestObstacleWithinAttackRange()
	{
		potentialObstacles = GetTargetsWithinAttackRange(ASSETS.LevelAssets.DoorLayer);
		if (potentialObstacles == null) return null;
		if(potentialObstacles.Count <= 0)
		{
			return null;
		}
		return GetClosestObstacleTo(potentialObstacles, transform.position);
	}

	private List<Life> GetTargetsWithinAttackRange(LayerMask layer)
	{ 
		var list = Physics2D.OverlapCircleAll(transform.position, targetterLife.AttackRange, layer)?.ToList();
		return list?.Select(x => x.GetComponentInChildren<Life>()).ToList();
	}

	public bool CanAttackCurrentTarget() {
		if(currentTarget == null) return false;
		return !buildingIsInTheWay(currentTarget.transform.position) && isWithinAttackRange(currentTarget.transform.position) && !(currentTarget == null);
	}

	public bool CanAttackObstacle() => GetCurrentObstacle() != null && !GetCurrentObstacle().IsDead() &&
	                                   isWithinAttackRange(GetCurrentObstacle().transform.position);

	public Life GetCurrentTarget() => specialTarget != null ? specialTarget : currentTarget;

	public void SetSpecialTarget(Life ownerSpawnedPlayerDefence)
	{
		specialTarget = ownerSpawnedPlayerDefence;
	}

	public Life GetCurrentObstacle()
	{
		currentObstacle = GetClosestObstacleWithinAttackRange(); 
		return currentObstacle;
	}

	public Vector2 GetWanderPosition(Vector2 wanderPoint)
	{
		var maxTries = 30;
		for (int i = 0; i < maxTries; i++)
		{
			var point = wanderPoint + Random.insideUnitCircle * targetterLife.AggroRange;
			if (!buildingIsInTheWay(point)) return point;
		}
	return  Vector2.zero ;
	}
	#endregion

	public bool HasLineOfSightWithCurrentTarget() => currentTarget && !buildingIsInTheWay(currentTarget.transform.position);

	public bool HasLineOfSightWith(Vector3 transformPosition) => !buildingIsInTheWay(transformPosition);
}