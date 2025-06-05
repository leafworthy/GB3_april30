using System;
using __SCRIPTS._ENEMYAI;

namespace __SCRIPTS
{
	[Serializable]
	public class ExplosionAttack : Attacks
	{
		private Life currentTargetLife;

		private EnemyAI ai;
		private Animations anim;
		private float explosionRadius = 5;
		public override string VerbName => "ExplosionAttack";

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			anim = GetComponent<Animations>();
			anim.animEvents.OnAttackHit += AttackHit;
			if (attacker.IsPlayer)
			{
				attacker.player.Controller.Attack1RightTrigger.OnPress += Player_Attack;

			}
			else
			{
				ai = GetComponent<EnemyAI>();
				ai.OnAttack += AI_Attack;
			}
		}

		private void OnDisable()
		{
			if (attacker.IsPlayer)
			{
				attacker.player.Controller.Attack1RightTrigger.OnPress -= Player_Attack;
			}
			else
			{
				ai.OnAttack -= AI_Attack;
			}

			if (anim == null) return;
			if (anim.animEvents == null) return;
			anim.animEvents.OnAttackHit -= AttackHit;

		}

		private void AttackHit(int attackType)
		{
			if (PauseManager.I.IsPaused) return;
			if (attacker.IsDead()) return;
			if (currentTargetLife == null) return;

			Explosion_FX.Explode(transform.position, explosionRadius, attacker.PrimaryAttackDamageWithExtra, attacker.player);
			attacker.DieNow();
		}

		private void AI_Attack(Life newTarget)
		{
			if (newTarget == null) return;
			if (PauseManager.I.IsPaused) return;
			if (attacker.IsDead()) return;
			var move = GetComponent<MoveAbility>();
			currentTargetLife = newTarget;
			anim.SetTrigger(Animations.Attack1Trigger);
			move.Push(currentTargetLife.transform.position - transform.position, 4);
		}

		private void Player_Attack(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			if (attacker.IsDead()) return;

			var hitObject = RaycastToObject(currentTargetLife);
			if (hitObject.collider == null) return;


			currentTargetLife = hitObject.collider.gameObject.GetComponent<Life>();
			if (currentTargetLife == null) return;
			anim.SetTrigger(Animations.Attack1Trigger);
		}



	}
}
