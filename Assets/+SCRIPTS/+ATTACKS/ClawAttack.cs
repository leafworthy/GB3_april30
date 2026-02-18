using System;
using __SCRIPTS._ENEMYAI;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class ClawAttack : MonoBehaviour, INeedPlayer
	{
		float currentCooldownTime;
		IGetAttacked currentTargetLife;

		IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
		IGetAttacked _life;

		ICanAttack ai;
		UnitAnimations anim;
		Body body;
		Targetter targetter;
		public AnimationClip clawAttackAnimationClip;

		public void SetPlayer(Player newPlayer)
		{
			body = GetComponent<Body>();
			anim = GetComponent<UnitAnimations>();
			targetter = GetComponent<Targetter>();
			ai = GetComponent<ICanAttack>();

			if (ai == null) Debug.LogWarning("ClawAttack: ICanAttack component not found on " + gameObject.name);
			ai.OnAttack += AI_Attack;
			anim.animEvents.OnAttackHit += OnAttackHit;
		}

		void OnDisable()
		{
			if (ai == null) return;
			ai.OnAttack -= AI_Attack;
			if (anim == null) return;
			anim.animEvents.OnAttackHit -= OnAttackHit;
		}

		void AI_Attack(IGetAttacked newTarget)
		{
			if (Services.pauseManager.IsPaused) return;
			if (TargetIsInvalid(newTarget))
			{
				Debug.Log("ClawAttack: Target is invalid, cannot offence.");
				return;
			}
			currentTargetLife = newTarget;
			StartAttack();
		}

		bool TargetIsInvalid(IGetAttacked newTarget)
		{
			if (life == null || life.IsDead() || newTarget.IsDead()) return true;
			return !targetter.HasLineOfSightWith(newTarget.transform.position);
		}

		void StartAttack()
		{
			if (!(Time.time >= currentCooldownTime))
			{
				return;
			}

			currentCooldownTime = Time.time + ai.stats.Stats.Rate(1);

			// Face the target only when starting a new offence
			FaceTarget();

			anim.SetTrigger(UnitAnimations.Attack1Trigger);
		}

		void FaceTarget()
		{
			if (currentTargetLife == null) return;

			// Calculate direction to target
			Vector2 directionToTarget = currentTargetLife.transform.position - transform.position;

			// Only update facing if the distance is significant to avoid flipping
			if (Mathf.Abs(directionToTarget.x) > 0.5f)
			{
				var shouldFaceRight = directionToTarget.x > 0;
				body.BottomFaceDirection(shouldFaceRight);
			}
		}

		void OnAttackHit(int attackType)
		{
			if (Services.pauseManager.IsPaused) return;
			if (currentTargetLife == null) return;

			if (Vector2.Distance(transform.position, currentTargetLife.transform.position) <= ai.stats.Stats.Range(1))
				MyAttackUtilities.HitTarget(ai, currentTargetLife, ai.stats.Stats.Damage(1));
		}
	}
}
