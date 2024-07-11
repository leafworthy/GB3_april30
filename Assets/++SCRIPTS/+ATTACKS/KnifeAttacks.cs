using System;
using System.Linq;
using UnityEngine;

public class KnifeAttacks : Attacks
{
	public float knifeDamageMultiplier = 3f;
	private int knifeCoolDown = 4;

	private AmmoInventory ammo;
	private UnitStats stats;
	private Player player;
	private Body body;
	private Animations anim;
	private string KnifeAttackVerbName = "knifing";
	private string AnimationName = "Top-Knife";
	private bool isAttacking;
	private bool hasPressed;
	public GameObject attackPoint;
	public event Action OnMiss;
	public event Action<Vector2> OnHit;
	public float hitSize = 15;

	private void Start()
	{
		anim = GetComponent<Animations>();
		body = GetComponent<Body>();
		stats = GetComponent<UnitStats>();
		player = stats.player;
		ammo = GetComponent<AmmoInventory>();
		player.Controller.Attack3.OnPress += PlayerKnifePress;
		player.Controller.Attack3.OnRelease += PlayerKnifeRelease;
		anim.animEvents.OnAttackHit += Anim_AttackHit;
	}

	private void OnDisable()
	{
		player.Controller.Attack3.OnPress -= PlayerKnifePress;
		player.Controller.Attack3.OnRelease -= PlayerKnifeRelease;
	 anim.animEvents.OnAttackHit -= Anim_AttackHit;
	}

	private void FixedUpdate()
	{
		if (ammo.HasFullReserve(AmmoInventory.AmmoType.meleeCooldown) && isAttacking)
		{
			isAttacking = false;

			anim.SetBool(Animations.IsBobbing, false);
			body.arms.Stop(KnifeAttackVerbName);
			return;
		}
		ammo.AddAmmoToReserve(AmmoInventory.AmmoType.meleeCooldown, knifeCoolDown);



	}
	private void PlayerKnifeRelease(NewControlButton newControlButton)
	{
		hasPressed = false;
	}

	private void PlayerKnifePress(NewControlButton newControlButton)
	{
		if (GlobalManager.IsPaused) return;
		if (!ammo.HasFullReserve(AmmoInventory.AmmoType.meleeCooldown)) return;
		if (!body.arms.Do(KnifeAttackVerbName)) return;
		if (hasPressed) return;
		isAttacking = true;
		hasPressed = true;
		ammo.UseAmmo(AmmoInventory.AmmoType.meleeCooldown, 999);
		anim.Play(AnimationName, 1, 0);
		anim.SetTrigger(Animations.KnifeTrigger);
		anim.SetBool(Animations.IsBobbing, true);
	}

	private GameObject FindClosestHit()
	{
	
		var circleCast = Physics2D.OverlapCircleAll(attackPoint.transform.position, hitSize, ASSETS.LevelAssets.EnemyLayer)
		                          .ToList();
		if (circleCast.Count <= 0) return null;

		var closest = circleCast[0];
		foreach (var col in circleCast)
		{
			var colStats = col.GetComponentInChildren<Life>();
			if (colStats.IsObstacle) continue;
			if (colStats.cantDie) continue;
			if (Vector2.Distance(col.gameObject.transform.position, transform.position) <
			    Vector2.Distance(closest.transform.position, transform.position))
				closest = col;
		}

		return closest.gameObject;
	}

	private void Anim_AttackHit(int attackType)
	{
		if (attackType != 3) return;

		var enemyHit = FindClosestHit();
		if (enemyHit == null)
		{
			OnMiss?.Invoke();
			return;
		}
		var enemy = enemyHit.transform.gameObject.GetComponent<Life>();
		if (enemy == null)
		{
			enemy = enemyHit.transform.gameObject.GetComponentInParent<Life>();
		}

		if (enemy == null || enemy.IsPlayer || enemy.cantDie || enemy.IsObstacle)
		{
			OnMiss?.Invoke();
			return;
		}
		OnHit?.Invoke(enemyHit.transform.position);
		
		HitTarget(stats.AttackDamage * knifeDamageMultiplier, enemy, .1f);

	}

}