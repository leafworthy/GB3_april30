using UnityEngine;

namespace __SCRIPTS._ENEMYAI.EnemyAI_States
{
	public class AttackPlayerState : IAIState
	{
		private Life target;
		private IAI ai;


		public void OnEnterState(IAI _ai)
		{
			ai = _ai;
			target = _ai.Targets.GetClosestAttackablePlayer();
			if (target == null)
			{
				_ai.TransitionToState(new AggroState());
				return;
			}

			_ai.Attack(target);
		}

		public void OnExitState()
		{
		}

		public void UpdateState()
		{

			target = ai.Targets.GetClosestAttackablePlayer();
			if (target != null)
			{
				ai.Attack(target);
			}
			else
			{
				ai.TransitionToState(new AggroState());
			}
		}
	}

	public class AttackObstacleState : IAIState
	{
		private Life target;
		private IAI ai;
		public void OnEnterState(IAI _ai)
		{
			ai = _ai;
			AttackObstacle();
		}

		private void AttackObstacle()
		{
			var player = ai.Targets.GetClosestPlayer();
			if(player == null)
			{
				ai.TransitionToState(new WanderState());
				return;
			}

			target = ai.Targets.GetClosestAttackableObstacle();
			if (target != null && !target.IsDead())
			{
				// Check if we're still within attack range
				float distance = Vector2.Distance(ai.transform.position, target.transform.position);
				if (distance <= ai.Life.PrimaryAttackRange)
				{
					ai.Attack(target);
				}
				else
				{
					// Door is too far away now, go back to chasing player
					ai.TransitionToState(new AggroState());
				}
			}
			else
			{
				// Door is destroyed or no longer attackable - return to player targeting
				ai.TransitionToState(new AggroState());
			}
		}

		public void OnExitState()
		{
		}

		public void UpdateState()
		{
			AttackObstacle();
		}
	}
}
