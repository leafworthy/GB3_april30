
using System;
using UnityEngine;

[Serializable]
public class ExplosionAttack : Attacks
{
	private float currentCooldownTime;
	private Vector2 currentAttackTarget;
	private Life currentTargetLife;

	private EnemyAI ai;
	private Animations anim;
	private Life life;
	private RecycleGameObject recycle;
	private float explosionRadius = 5;

	private void Start()
	{
		life = GetComponent<Life>();
		anim = GetComponent<Animations>();
		anim.animEvents.OnAttackHit += AttackHit;
		if (life.IsPlayer)
		{
			life.player.Controller.Attack1.OnPress += Player_Attack;
			
		}
		else
		{
			ai = GetComponent<EnemyAI>();
			ai.OnAttack += AI_Attack;
		}
	}



	private void AttackHit(int attackType)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (attacker.IsDead()) return;
		if (currentTargetLife == null) return;

		life = GetComponent<Life>();
		Explosions.I.Explode(transform.position, explosionRadius, life.AttackDamage, life.player);
		Debug.Log("explode");
		life.DieNow();
	}

	private void AI_Attack(Life newTarget)
	{
		if (newTarget == null) return;
		if (Game_GlobalVariables.IsPaused) return;
		if (attacker.IsDead()) return;
		var move = GetComponent<MoveAbility>();
		currentTargetLife = newTarget;
		currentAttackTarget = newTarget.transform.position;
		anim.SetTrigger(Animations.Attack1Trigger);
		move.Push(currentTargetLife.transform.position - transform.position, 10);
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
		AttackTarget(transform.position);
	}

	private void AttackTarget(Vector2 target)
	{
		currentAttackTarget = target;
		anim.SetTrigger(Animations.Attack1Trigger);
	}
	
}