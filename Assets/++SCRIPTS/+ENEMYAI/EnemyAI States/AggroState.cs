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

			Debug.Log($"[{ai.transform.name}] AggroState.OnEnterState - Found target: {(target != null ? target.name : "NULL")}");

			if (target == null)
			{
				ai.Thoughts.Think("No target found, going idle");
				Debug.Log($"[{ai.transform.name}] No target found, transitioning to IdleState");
				ai.TransitionToState(new IdleState());
				return;
			}

			Debug.Log($"[{ai.transform.name}] Setting target position to: {target.transform.position}");
			ai.Pathmaker.SetTargetPosition(target.transform.position);
		}

		private bool CanAttackObstacleWithinAttackRange()
		{

			var closestObstacle = ai.Targets.GetClosestAttackableObstacle();
			if (closestObstacle == null) return false;
			if(!ai.Targets.HasLineOfSightWith(closestObstacle.transform.position))
			{
				ai.Thoughts.Think("Not attacking because obstacle not in sight");
				return false;
			}

			ai.Thoughts.Think("attacking obstacle " + closestObstacle.name);
			ai.TransitionToState(new AttackObstacleState());
			return true;
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
				Debug.Log($"[{ai.transform.name}] AggroState.UpdateState - No target, going to IdleState");
				ai.TransitionToState(new IdleState());
				return;
			}

			// Try pathfinding first, but have fallback to direct movement
			Debug.Log($"[{ai.transform.name}] AggroState.UpdateState - Targeting: {target.name} at {target.transform.position}");
			
			// If we have line of sight, walk directly (this avoids pathfinding issues)
			if (ai.Targets.HasLineOfSightWith(target.transform.position))
			{
				Debug.Log($"[{ai.transform.name}] Has line of sight, walking directly to target");
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
				Debug.Log($"[{ai.transform.name}] Close enough to attack, switching to AttackPlayerState");
				ai.TransitionToState(new AttackPlayerState());
				return;
			}
		}

		private void WalkDirectlyToTarget()
		{
			ai.Pathmaker.WalkInDirectionOfTarget(target.transform.position);
		}

		private bool CloseEnoughToAttack() =>
			Vector2.Distance(ai.transform.position, target.transform.position) <= ai.Life.PrimaryAttackRange;
	}
}
