using System;
using UnityEngine;

[Serializable]
public class MeleeAttack : Attacks
{
	private float currentCooldownTime;
	private Life currentTargetLife;

	private UnitStats stats;
	private EnemyAI ai;
	private Animations anim;
	private Body body;
	public GameObject attackPoint;
	public float extraPush = .2f;
	private Targetter targetter;

	

	private void Start()
	{
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		stats = GetComponent<UnitStats>();
		targetter = GetComponent<Targetter>();


		if (stats.IsPlayer)
		{
			stats.player.Controller.Attack1.OnPress += Player_Attack;
		}
		else
		{
			ai = GetComponent<EnemyAI>();
			ai.OnAttack += AI_Attack;
		}

		anim.animEvents.OnAttackHit += OnAttackHit;
		anim.animEvents.OnAttackStop += OnAttackStop;
	}

	private void OnAttackStop(int obj)
	{
		body.arms.Stop("MeleeAttack");
	}

	private void OnDisable()
	{
		if (stats.IsPlayer)
		{
			stats.player.Controller.Attack1.OnPress -= Player_Attack;
		}
		else
		{
			ai.OnAttack -= AI_Attack;
		}
	
	}

	private void Player_Attack(NewControlButton newControlButton)
	{
		if (GlobalManager.IsPaused) return;
		if (attacker.IsDead()) return;
		if(!body.arms.Do("MeleeAttack")) return;
		StartAttack();
	}


	private void AI_Attack(Life newTarget)
	{
		if (newTarget == null) return;
		if (GlobalManager.IsPaused) return;
		if (attacker.IsDead()) return;
		if(!targetter.HasLineOfSightWith(newTarget.transform.position) && !newTarget.IsObstacle)
		{
			Debug.Log("THIS");
			return;
		}

		if (!(Time.time >= currentCooldownTime)) return;
		currentCooldownTime = Time.time + stats.AttackRate;
		Debug.Log("trigger");
		anim.SetTrigger(Animations.Attack1Trigger);
	}


	private void StartAttack()
	{
		if (!(Time.time >= currentCooldownTime)) return;

		currentCooldownTime = Time.time + stats.AttackRate;
		
		  
		anim.SetTrigger(Animations.Attack1Trigger);
	}


	private void OnAttackHit(int attackType)
	{
		if (GlobalManager.IsPaused) return;
		if (attacker.IsDead()) return;
		var layer = attacker.IsPlayer?ASSETS.LevelAssets.EnemyLayer: ASSETS.LevelAssets.PlayerLayer;
		 var hits = Physics2D.OverlapCircleAll(attackPoint.transform.position, stats.AttackRange, layer);
		 foreach (var hit in hits)
		 {
			 var life = hit.gameObject.GetComponent<Life>();
			 if (life == null) continue;
			 if (attacker.IsPlayer && life.IsObstacle) continue;
			 if (!life.isEnemyOf(attacker)) continue;
			 if(!targetter.HasLineOfSightWith(hit.transform.position))return;
			 HitTarget(stats.AttackDamage, life, extraPush);
			 return;
		 }
	}

	
}