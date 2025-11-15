using System;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using GangstaBean.Core;
using UnityEngine;





namespace __SCRIPTS
{
	[Serializable]
	public class ClawAttack : MonoBehaviour, INeedPlayer
	{
		private float currentCooldownTime;
		private IGetAttacked currentTargetLife;

		private IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
		private IGetAttacked _life;

		private ICanAttack ai;
		private UnitAnimations anim;
		private Body body;
		private Targetter targetter;
		public AnimationClip clawAttackAnimationClip;

		public void SetPlayer(Player newPlayer)
		{
			Debug.Log("claw offence set player");
			body = GetComponent<Body>();
			anim = GetComponent<UnitAnimations>();
			targetter = GetComponent<Targetter>();
			ai = GetComponent<ICanAttack>();

			if(ai == null) Debug.LogWarning( "ClawAttack: ICanAttack component not found on " + gameObject.name);
			ai.OnAttack += AI_Attack;
			anim.animEvents.OnAttackHit += OnAttackHit;

		}



		private void OnDisable()
		{
			if (ai == null) return;
			ai.OnAttack -= AI_Attack;
			if (anim == null) return;
			anim.animEvents.OnAttackHit -= OnAttackHit;

		}

		private void AI_Attack(IGetAttacked newTarget)
		{
			Debug.Log("ai on offence");
			if (Services.pauseManager.IsPaused) return;
			if (TargetIsInvalid(newTarget))
			{
				Debug.Log("ClawAttack: Target is invalid, cannot offence.");
				return;
			}

			Debug.Log("target valid, starting offence");
			currentTargetLife = newTarget;
			StartAttack();
		}

		private bool TargetIsInvalid(IGetAttacked newTarget)
		{
			if (life == null ||life.IsDead() || newTarget.IsDead() ) return true;
			return !targetter.HasLineOfSightWith(newTarget.transform.position);
		}

		private void StartAttack()
		{
			if (!(Time.time >= currentCooldownTime))
			{
				Debug.Log("cooling down, cannot offence yet");
				return;
			}

			currentCooldownTime = Time.time + ai.stats.PrimaryAttackRate;

			// Face the target only when starting a new offence
			FaceTarget();

			Debug.Log("triggering offence");
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

			if (Vector2.Distance(transform.position, currentTargetLife.transform.position) <= ai.stats.PrimaryAttackRange*1.25f)
			{
				AttackUtilities.HitTarget(ai, currentTargetLife, ai.stats.PrimaryAttackDamageWithExtra);
			}


		}
	}
}
