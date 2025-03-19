using System;

[Serializable]
public class ExplosionAttack : Attacks
{
	private float currentCooldownTime;
	private Life currentTargetLife;

	private EnemyAI ai;
	private Animations anim;
	private Life life;
	private float explosionRadius = 5;

	private void Start()
	{
		life = GetComponent<Life>();
		anim = GetComponent<Animations>();
		anim.animEvents.OnAttackHit += AttackHit;
		if (life.IsPlayer)
		{
			life.player.Controller.Attack1RightTrigger.OnPress += Player_Attack;
			
		}
		else
		{
			ai = GetComponent<EnemyAI>();
			ai.OnAttack += AI_Attack;
		}
	}

	private void OnDisable()
	{
		if (life.IsPlayer)
		{
			life.player.Controller.Attack1RightTrigger.OnPress -= Player_Attack;
		}
		else
		{
			ai.OnAttack -= AI_Attack;
		}
	
	}

	private void AttackHit(int attackType)
	{
		if (PauseManager.IsPaused) return;
		if (attacker.IsDead()) return;
		if (currentTargetLife == null) return;

		life = GetComponent<Life>();
		Explosion_FX.Explode(transform.position, explosionRadius, life.AttackDamage, life.player);
		life.DieNow();
	}

	private void AI_Attack(Life newTarget)
	{
		if (newTarget == null) return;
		if (PauseManager.IsPaused) return;
		if (attacker.IsDead()) return;
		var move = GetComponent<MoveAbility>();
		currentTargetLife = newTarget;
		anim.SetTrigger(Animations.Attack1Trigger);
		move.Push(currentTargetLife.transform.position - transform.position, 4);
	}

	private void Player_Attack(NewControlButton newControlButton)
	{
		if (PauseManager.IsPaused) return;
		if (attacker.IsDead()) return;

		var hitObject = RaycastToObject(currentTargetLife);
		if (hitObject.collider == null) return;


		currentTargetLife = hitObject.collider.gameObject.GetComponent<Life>();
		if (currentTargetLife == null) return;
		anim.SetTrigger(Animations.Attack1Trigger);
	}


	
}