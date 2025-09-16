using System;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using UnityEngine;


public interface IAttack
{
	event Action<Life> OnAttack;
}


namespace __SCRIPTS
{
	[Serializable]
	public class ClawAttack : Attacks
	{
		private float currentCooldownTime;
		private Life currentTargetLife;

		private IAttack ai;
		private UnitAnimations anim;
		private Body body;
		public float extraPush = .2f;
		private Targetter targetter;

		public override string AbilityName => "MeleeAttack";

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			body = GetComponent<Body>();
			anim = GetComponent<UnitAnimations>();
			targetter = GetComponent<Targetter>();
			ai = GetComponent<IAttack>();

			ai.OnAttack += AI_Attack;
			anim.animEvents.OnAttackHit += OnAttackHit;
			anim.animEvents.OnAttackStop += OnAttackStop;

		}


		private void OnAttackStop(int obj)
		{
			body.arms.Stop(this);
		}

		private void OnDisable()
		{
			if (ai == null) return;
			ai.OnAttack -= AI_Attack;
			if (anim == null) return;
			anim.animEvents.OnAttackHit -= OnAttackHit;
			anim.animEvents.OnAttackStop -= OnAttackStop;

		}

		private void AI_Attack(Life newTarget)
		{
			if (Services.pauseManager.IsPaused) return;
			if (TargetIsInvalid(newTarget)) return;
			currentTargetLife = newTarget;
			StartAttack();
		}

		private bool TargetIsInvalid(Life newTarget)
		{
			if (base.life.IsDead() || newTarget.IsDead()) return true;
			if (!targetter.HasLineOfSightWith(newTarget.transform.position) && !newTarget.IsObstacle) return true;
			return false;

		}

		private void StartAttack()
		{
			if (!(Time.time >= currentCooldownTime)) return;

			currentCooldownTime = Time.time + base.life.PrimaryAttackRate;

			// Face the target only when starting a new attack
			FaceTarget();

			anim.SetTrigger(UnitAnimations.Attack1Trigger);
		}

		private void FaceTarget()
		{
			if (currentTargetLife == null) return;

			// Calculate direction to target
			Vector2 directionToTarget = currentTargetLife.transform.position - transform.position;

			// Only update facing if the distance is significant to avoid flipping
			if (Mathf.Abs(directionToTarget.x) > 0.5f)
			{
				bool shouldFaceRight = directionToTarget.x > 0;
				body.BottomFaceDirection(shouldFaceRight);
			}
		}

		private void OnAttackHit(int attackType)
		{
			if (Services.pauseManager.IsPaused) return;
			if (currentTargetLife == null) return;
			if (base.life.IsDead() || currentTargetLife.IsDead()) return;
			if (!currentTargetLife.IsObstacle)
			{
				if (!targetter.HasLineOfSightWith(currentTargetLife.transform.position))
				{
					return;
				}
			}
			else
			{
				AttackUtilities.HitTarget(base.life.PrimaryAttackDamageWithExtra, life,currentTargetLife, extraPush);
				return;
			}

			if (Vector2.Distance(transform.position, currentTargetLife.transform.position) <= base.life.PrimaryAttackRange*1.25f)
			{
				AttackUtilities.HitTarget(base.life.PrimaryAttackDamageWithExtra, life, currentTargetLife, extraPush);
			}


		}
	}
}
