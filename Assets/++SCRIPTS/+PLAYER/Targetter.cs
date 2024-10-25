using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Targetter : MonoBehaviour
{
	private Life targetterLife => GetComponent<Life>();
	private Life specialTarget;
	public Life CurrentTarget { get; private set; }
	public Life CurrentObstacle { get; private set; }

	private List<Life> potentialObjectTargets = new();
	private List<Life> potentialObstacles = new();

	#region private functions

	private Life GetClosest(List<Life> targets) => targets.OrderBy(t => Vector2.Distance(t.transform.position, transform.position)).FirstOrDefault();
	public Life GetClosestDoorInAttackRange() => GetClosest(GetAttackableTargetsInRange(ASSETS.LevelAssets.DoorLayer, targetterLife.AttackRange));
	public Life GetClosestPlayerInAttackRange() => GetClosest(GetAttackableTargetsInRange(ASSETS.LevelAssets.PlayerLayer, targetterLife.AttackRange));
	public Life GetClosestPlayerInAggroRange() => GetClosest(GetAggroTargets(ASSETS.LevelAssets.PlayerLayer));

	private List<Life> GetAttackableTargetsInRange(LayerMask layer, float range) =>
		Physics2D.OverlapCircleAll(transform.position, range, layer).Select(x => x.GetComponentInChildren<Life>())
		         .Where(life => life != null && CanAttack(life)).ToList();

	private List<Life> GetAggroTargets(LayerMask layer) => GetAttackableTargetsInRange(layer, targetterLife.AggroRange);

	private List<Life> GetAttackableTargetsAtAttackRange(LayerMask layer) => GetAttackableTargetsInRange(layer, targetterLife.AttackRange);

	private bool doorIsValidTarget(DoorInteraction doorInteractionAnimator) =>
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

	public Vector2 GetWanderPosition(Vector2 wanderPoint)
	{
		var maxTries = 30;
		for (var i = 0; i < maxTries; i++)
		{
			var point = wanderPoint + Random.insideUnitCircle * targetterLife.AggroRange;
			if (!buildingIsInTheWay(point)) return point;
		}

		Debug.Log("wander point not found");
		return wanderPoint;
	}

	#endregion

	public bool HasLineOfSightWith(Vector3 transformPosition) => !buildingIsInTheWay(transformPosition);

	private bool CanAttack(Life target)
	{
		if (target == null || target.IsDead())
			return false;
		if (target.IsObstacle)
		{
			if (!doorIsValidTarget(target.GetComponent<DoorInteraction>()))
				return false;
		}

		if (!isWithinAttackRange(target.transform.position)) return false;

		return HasLineOfSightWith(target.transform.position);
	}
}