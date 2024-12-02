using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BatAttacks : Attacks
{
	private AnimationEvents animEvents;
	private JumpAbility jumps;
	private Animations anim;
	private Body body;
	private const string batAttackVerbName = "bat attacking";
	private bool isCharging;

	private Life life;
	private AmmoInventory ammo;

	private int batNormalCooldown = 10;
	private int attackKillSpecialAmount = 30;

	private bool isFullyCharged;

	private Player owner;
	public event Action OnAttackStop;
	public event Action OnHit;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		life = GetComponent<Life>();
		owner = life.player;
		jumps = GetComponent<JumpAbility>();
		ammo = GetComponent<AmmoInventory>();

		if (owner != null) owner.Controller.Attack1.OnPress += Player_AttackPress;

		animEvents = anim.animEvents;
		animEvents.OnAttackHit += Anim_AttackHit;
		animEvents.OnAttackStop += Anim_AttackStop;
	}

	private void FixedUpdate()
	{
		if (!ammo.HasFullReserve(AmmoInventory.AmmoType.meleeCooldown))
			ammo.AddAmmoToReserve(AmmoInventory.AmmoType.meleeCooldown, batNormalCooldown);
	}

	private void Anim_AttackStop(int attackType)
	{
		body.arms.Stop(batAttackVerbName);
		body.legs.Stop(batAttackVerbName);

		if (attackType == 0) OnAttackStop?.Invoke();
		anim.ResetTrigger(Animations.ChargeAttackTrigger);
		anim.ResetTrigger(Animations.ChargeStartTrigger);
	}

	private void Anim_AttackHit(int attackType)
	{
		if (attackType != 5)
			RegularAttackHit(attackType);
	}

	private void RegularAttackHit(int attackType)
	{
		var circleCast = Physics2D.OverlapCircleAll(transform.position, GetHitRange(attackType), ASSETS.LevelAssets.EnemyLayer);
		var closest2 = circleCast.OrderBy(item => Vector2.Distance(item.gameObject.transform.position, transform.position)).Take(2);
		foreach (var col in closest2)
		{
			if (col == null) return;
			var life = col.gameObject.GetComponent<Life>();
			if (!life.isEnemyOf(attacker) || life.cantDie) return;
			if (attacker.IsPlayer && life.IsObstacle) return;
			HitTarget(GetAttackDamage(attackType), life, .2f);
			OnHit?.Invoke();
			ammo.AddAmmoToReserve(AmmoInventory.AmmoType.meleeCooldown, attackKillSpecialAmount);
		}
	}

	private float GetHitRange(int attackType)
	{
		if (attackType == 5)
			return life.AttackRange * 2;
		return life.AttackRange;
	}

	private float GetAttackDamage(int attackType)
	{
		return attackType switch
		       {
			       1 => 50,
			       2 => 50,
			       3 => 100,
			       4 => 100,
			       _ => 0
		       };
	}

	private void Player_AttackPress(NewControlButton newControlButton)
	{
		if (jumps.IsJumping)
			StartJumpAttack();
		if (!body.arms.Do(batAttackVerbName))
		{
			if (body.arms.currentActivity != JumpAbility.VerbName)
				return;
		}

		if (!body.legs.Do(batAttackVerbName)) return;
		if (!ammo.HasFullReserve(AmmoInventory.AmmoType.meleeCooldown)) return;

		StartRandomAttack();
	}

	private void StartRandomAttack()
	{
		ammo.UseAmmo(AmmoInventory.AmmoType.meleeCooldown, 1000);
		var randomAttack = Random.Range(1, 3);
		switch (randomAttack)
		{
			case 1:
				anim.SetTrigger(Animations.Attack1Trigger);
				break;
			case 2:
				anim.SetTrigger(Animations.Attack2Trigger);
				break;
			default:
				anim.SetTrigger(Animations.Attack3Trigger);
				break;
		}
	}

	private void StartJumpAttack()
	{
		anim.SetTrigger(Animations.JumpAttackTrigger);
	}
}