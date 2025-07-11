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
				ai.Thoughts.Think("Attacking Player!" + target.name);
				ai.Attack(target);
			}
			else
			{
				ai.Thoughts.Think("no targets, going aggro!");
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
				ai.Thoughts.Think("No more player, wander time");
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
					ai.Thoughts.Think("Attacking Door!");
					ai.Attack(target);
				}
				else
				{
					// Door is too far away now, go back to chasing player
					ai.Thoughts.Think("Door too far, going back to aggro");
					ai.TransitionToState(new AggroState());
				}
			}
			else
			{
				// Door is destroyed or no longer attackable - return to player targeting
				ai.Thoughts.Think("Door cleared, Going back to aggro");
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
