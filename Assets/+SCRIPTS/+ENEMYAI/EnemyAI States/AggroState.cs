using UnityEngine;

namespace __SCRIPTS._ENEMYAI.EnemyAI_States
{
	public class AggroState : IAIState
	{
		private Life target;
		private IAI ai;

		public void OnEnterState(IAI _ai)
		{
			ai = _ai;
			_ai.GetBornIfBornOnAggro();
			target = ai.Targets.GetClosestPlayer();


			if (target == null)
			{
				ai.Thoughts.Think("No target found, going idle");
				ai.TransitionToState(new IdleState());
				return;
			}

			ai.Pathmaker.SetTargetPosition(target.transform.position);
		}




		public void OnExitState()
		{
		}

		public void UpdateState()
		{
			// Always prioritize players - simple and direct approach
			target = ai.Targets.GetClosestPlayer();
			if (target == null)
			{
				ai.TransitionToState(new IdleState());
				return;
			}

			// Check if we're stuck near a door and need to attack it
			if (IsBlockedByDoor())
			{
				var blockingDoor = ai.Targets.GetClosestAttackableObstacle();
				if (blockingDoor != null)
				{
					ai.TransitionToState(new AttackObstacleState());
					return;
				}
			}

			// Try pathfinding first, but have fallback to direct movement
			
			// If we have line of sight, walk directly (this avoids pathfinding issues)
			if (ai.Targets.HasLineOfSightWith(target.transform.position))
			{
				ai.Pathmaker.WalkInDirectionOfTarget(target.transform.position);
			}
			else
			{
				// Try pathfinding (will fallback to direct movement if pathfinding fails)
				ai.Pathmaker.SetTargetPosition(target.transform.position);
			}
			
			// If close enough to attack, switch to attack state
			if (CloseEnoughToAttack()) 
			{
				ai.TransitionToState(new AttackPlayerState());
				return;
			}
		}

		private bool IsBlockedByDoor()
		{
			// Check if we can't reach the player due to a door blocking the path
			if (!ai.Targets.HasLineOfSightWith(target.transform.position))
			{
				// No line of sight - check if there's a door within attack range
				var nearbyDoor = ai.Targets.GetClosestAttackableObstacle();
				if (nearbyDoor != null)
				{
					float doorDistance = Vector2.Distance(ai.transform.position, nearbyDoor.transform.position);
					// If we're close to a door and can't see the player, the door is likely blocking us
					return doorDistance <= ai.Life.PrimaryAttackRange * 1.5f;
				}
			}
			return false;
		}

		private void WalkDirectlyToTarget()
		{
			ai.Pathmaker.WalkInDirectionOfTarget(target.transform.position);
		}

		private bool CloseEnoughToAttack() =>
			Vector2.Distance(ai.transform.position, target.transform.position) <= ai.Life.PrimaryAttackRange;
	}
}
