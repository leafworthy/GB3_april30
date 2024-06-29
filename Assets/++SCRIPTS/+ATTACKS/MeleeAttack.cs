using System;
using UnityEngine;

[Serializable]
public class MeleeAttack : Attacks
{
	private float currentCooldownTime;
	private Vector2 currentAttackTarget;
	private Life currentTargetLife;

	private UnitStats stats;
	private EnemyAI ai;
	private Animations anim;
	private Body body;
	private Player owner;
	private RecycleGameObject recycle;

	

	private void Start()
	{
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		stats = GetComponent<UnitStats>();


		if (stats.IsPlayer)
		{
			stats.player.Controller.Attack1.OnPress += Player_Attack;
			owner = stats.player;
		}
		else
		{
			ai = GetComponent<EnemyAI>();
			ai.OnAttack += AI_Attack;
			owner = Players.EnemyPlayer;
		}

		anim.animEvents.OnAttackHit += AttackHit;
	}



	private void Player_Attack(NewControlButton newControlButton)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (attacker.IsDead()) return;

		var hitObject = GetAttackHitObject(currentAttackTarget);
		if (hitObject.collider == null) return;

		currentAttackTarget = hitObject.point;

		currentTargetLife = hitObject.collider.gameObject.GetComponent<Life>();
		if (currentTargetLife == null) return;
		AttackTarget(body.AttackStartPoint.transform.position);
	}


	private void AI_Attack(Life newTarget)
	{
		if (newTarget == null) return;
		if (Game_GlobalVariables.IsPaused) return;
		if (attacker.IsDead()) return;

		if (!(Time.time >= currentCooldownTime)) return;
		currentCooldownTime = Time.time + stats.AttackRate;
		currentTargetLife = newTarget;
		currentAttackTarget = newTarget.transform.position;
		anim.SetTrigger(Animations.Attack1Trigger);
	}


	private void AttackTarget(Vector2 target)
	{
		if (!(Time.time >= currentCooldownTime)) return;

		currentCooldownTime = Time.time + stats.AttackRate;
		currentAttackTarget = target;
		anim.SetTrigger(Animations.Attack1Trigger);
	}


	private void AttackHit(int attackType)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (attacker.IsDead()) return;
		if (currentTargetLife == null) return;
		
		var newAttack = new Attack(attacker, currentTargetLife, currentTargetLife.AttackHeight);
		
		if (Vector2.Distance(currentTargetLife.transform.position, currentAttackTarget) <
		    stats.AttackRange * 2)
			currentTargetLife.TakeDamage(newAttack);
		
	}

	
}
