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
		if (newTarget == null) return;
		if (GlobalManager.IsPaused) return;
		if (attacker.IsDead()) return;
		if (!targetter.HasLineOfSightWith(newTarget.transform.position) && !newTarget.IsObstacle) return;

		if (!(Time.time >= currentCooldownTime)) return;
		currentCooldownTime = Time.time + attacker.AttackRate;
		Debug.Log("AI Trying To Attack", this);
		anim.SetTrigger(Animations.Attack1Trigger);
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
		if (attacker.IsDead()) return;
		Debug.Log("hit target");
		var layer = ASSETS.LevelAssets.EnemiesCanHitTheseLayers;
		var hits = Physics2D.OverlapCircleAll(transform.position, attacker.AttackRange*1.1f, layer);
		foreach (var hit in hits)
		{
			Debug.Log("one hit", this);
			var defender = hit.gameObject.GetComponent<Life>();
			if (defender == null) continue;

			

			HitTarget(attacker.AttackDamage, defender, extraPush);
			return;
		}
	}
}