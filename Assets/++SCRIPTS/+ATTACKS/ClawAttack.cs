using System;
using UnityEngine;

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

	private void Start()
	{
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
		body.arms.Stop("MeleeAttack");
	}

	private void OnDisable()
	{
		ai.OnAttack -= AI_Attack;
	}

	private void AI_Attack(Life newTarget)
	{
		if (GlobalManager.IsPaused) return;
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

		currentCooldownTime = Time.time + attacker.AttackRate;

		anim.SetTrigger(Animations.Attack1Trigger);
	}

	private void OnAttackHit(int attackType)
	{
		if (GlobalManager.IsPaused) return;
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
			HitTarget(attacker.AttackDamage, currentTargetLife, extraPush);
			return;
		}

		if (Vector2.Distance(transform.position, currentTargetLife.transform.position) <= attacker.AttackRange*1.25f)
		{
			HitTarget(attacker.AttackDamage, currentTargetLife, extraPush);
		}
			
		
	}
}