using UnityEngine;

namespace GangstaBean.EnemyAI.States._ENEMYAI.EnemyAI_States
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

		private bool CanAttackObstacleWithinAttackRange()
		{

			var closestObstacle = ai.Targets.GetClosestAttackableObstacle();
			if (closestObstacle == null) return false;
			if(ai.Targets.HasLineOfSightWith(target.transform.position))
			{
				ai.Thoughts.Think("Not attacking because target in sight");
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
			//if (IdleIfNoTargetInAggroRange()) return;
			target = ai.Targets.GetClosestPlayer();
			if (target == null)
			{
				ai.TransitionToState(new IdleState());
				return;
			}
			if (ai.Targets.HasLineOfSightWith(target.transform.position))
			{
				WalkDirectlyToTarget();
				if (CloseEnoughToAttack()) ai.TransitionToState(new AttackPlayerState());
				return;
			}
			if (CanAttackObstacleWithinAttackRange()) return;
			if (CloseEnoughToAttack()) ai.TransitionToState(new AttackPlayerState());
			ai.Pathmaker.SetTargetPosition(target.transform.position);
		}

		private void WalkDirectlyToTarget()
		{
			ai.Pathmaker.WalkInDirectionOfTarget(target.transform.position);
		}

		private bool CloseEnoughToAttack() =>
			Vector2.Distance(ai.transform.position, target.transform.position) <= ai.Life.PrimaryAttackRange;
	}
}
