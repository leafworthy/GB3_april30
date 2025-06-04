using System;
using GangstaBean.EnemyAI;
using UnityEngine;

namespace GangstaBean.Attacks
{
	[Serializable]
	public class ClawAttack : Attacks
	{
		private float currentCooldownTime;
		private Life currentTargetLife;

		private EnemyAI ai;
		private Animations anim;
		private Body body;
		public float extraPush = .2f;
		private Targetter targetter;

		public override string VerbName => "MeleeAttack";

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			body = GetComponent<Body>();
			anim = GetComponent<Animations>();
			targetter = GetComponent<Targetter>();
			ai = GetComponent<EnemyAI>();

			ai.OnAttack += AI_Attack;
			anim.animEvents.OnAttackHit += OnAttackHit;
			anim.animEvents.OnAttackStop += OnAttackStop;

		}


		private void OnAttackStop(int obj)
		{
			body.arms.StopSafely(this);
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
			if (PauseManager.I.IsPaused) return;
			if (TargetIsInvalid(newTarget)) return;
			currentTargetLife = newTarget;
			StartAttack();
		}

		private bool TargetIsInvalid(Life newTarget)
		{
			if (attacker.IsDead() || newTarget.IsDead()) return true;
			if (!targetter.HasLineOfSightWith(newTarget.transform.position) && !newTarget.IsObstacle) return true;
			return false;

		}

		private void StartAttack()
		{
			if (!(Time.time >= currentCooldownTime)) return;

			currentCooldownTime = Time.time + attacker.PrimaryAttackRate;

			anim.SetTrigger(Animations.Attack1Trigger);
		}

		private void OnAttackHit(int attackType)
		{
			if (PauseManager.I.IsPaused) return;
			if (currentTargetLife == null) return;
			if (attacker.IsDead() || currentTargetLife.IsDead()) return;
			if (!currentTargetLife.IsObstacle)
			{
				if (!targetter.HasLineOfSightWith(currentTargetLife.transform.position))
				{
					return;
				}
			}
			else
			{
				HitTarget(attacker.PrimaryAttackDamageWithExtra, currentTargetLife, extraPush);
				return;
			}

			if (Vector2.Distance(transform.position, currentTargetLife.transform.position) <= attacker.PrimaryAttackRange*1.25f)
			{
				HitTarget(attacker.PrimaryAttackDamageWithExtra, currentTargetLife, extraPush);
			}


		}
	}
}
