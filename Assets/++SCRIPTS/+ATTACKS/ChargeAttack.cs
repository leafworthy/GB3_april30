using System;
using UnityEngine;

public class ChargeAttack : Attacks
{
	private AnimationEvents animEvents;
	private Animations anim;
	private Body body;
	private MoveAbility aim;

	private bool isCharging;

	private AmmoInventory ammo;
	private Life life;

	private int attackKillSpecialAmount = 30;
	private float SpecialAttackDistance = 3;
	private float SpecialAttackWidth = 3;
	private bool isFullyCharged;
	private GameObject currentArrowHead;
	private string verbName = "ChargeAttack";
	private float attackDamage = 100;
	public event Action OnAttackHit;
	public event Action OnSpecialAttackHit;
	public event Action OnChargePress;
	public event Action OnChargeStop;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		aim = GetComponent<MoveAbility>();
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		life = GetComponent<Life>();
		ammo = GetComponent<AmmoInventory>();

		if (attacker != null)
		{
			attacker.player.Controller.ReloadTriangle.OnPress += Player_ChargePress;
			attacker.player.Controller.ReloadTriangle.OnRelease += Player_ChargeRelease;
		}

		animEvents = anim.animEvents;
		animEvents.OnFullyCharged += Anim_FullyCharged;
		animEvents.OnAttackHit += Anim_AttackHit;
		SpawnFX();
	}

	private void OnDisable()
	{
		if (attacker != null)
		{
			attacker.player.Controller.ReloadTriangle.OnPress -= Player_ChargePress;
			attacker.player.Controller.ReloadTriangle.OnRelease -= Player_ChargeRelease;
		}
	}

	private void Update()
	{
		if (isCharging)
		{
			ShowAiming();
			body.BottomFaceDirection(aim.MoveAimDir.x > 0);
		}
		else
			HideAiming();
	}

	private void SpawnFX()
	{
		currentArrowHead = ObjectMaker.Make(FX.Assets.nadeTargetPrefab);
		currentArrowHead.SetActive(false);
	}

	private void ShowAiming()
	{
		currentArrowHead.SetActive(true);
		currentArrowHead.transform.position = aim.GetAimPoint();
	}

	private void HideAiming()
	{
		currentArrowHead.SetActive(false);
	}

	private void Player_ChargePress(NewControlButton newControlButton)
	{
		if (GlobalManager.IsPaused) return;
		// if (!ammo.HasFullAmmo(AmmoInventory.AmmoType.primaryAmmo)) return;
		if (!body.arms.Do(verbName)) return;

		if (!body.legs.Do(verbName)) return;
		isCharging = true;
		isFullyCharged = false;
		OnChargePress?.Invoke();
		anim.SetTrigger(Animations.ChargeStartTrigger);
		anim.SetBool(Animations.IsCharging, true);
	}

	private void Player_ChargeRelease(NewControlButton newControlButton)
	{
		if (GlobalManager.IsPaused) return;
		if (!isCharging) return;
		OnChargeStop?.Invoke();
	
		isCharging = false;
		anim.SetBool(Animations.IsCharging, false);
		if (isFullyCharged)
		{
			ammo.UseAmmo(AmmoInventory.AmmoType.primaryAmmo, 1000);
			anim.SetTrigger(Animations.ChargeAttackTrigger);
		}
		else
		{
			body.arms.Stop(verbName);
			body.legs.Stop(verbName);
		}
	}

	private void Anim_AttackHit(int attackType)
	{
		if (attackType != 5) return;
		SpecialAttackHit(attackType);
	}

	private void Anim_FullyCharged()
	{
		isFullyCharged = true;
	}

	private void SpecialAttackHit(int attackType)
	{
		var position = body.AimCenter.transform.position;
		var targetPoint = aim.GetAimPoint();
		var connect = false;
		SpecialAttackDistance = Vector2.Distance(position, targetPoint);
		body.arms.Stop(verbName);
		body.legs.Stop(verbName);
		if (!DidCollideWithBuilding(position, targetPoint, out var raycast)) return;
		OnSpecialAttackHit?.Invoke();

		foreach (var raycastHit2D in raycast)
		{
			var life = raycastHit2D.collider.GetComponent<Life>();
			if (!life.isEnemyOf(attacker) || life.cantDie || life.IsObstacle) return;
			HitTarget(attackDamage, life, .8f);
			ammo.AddAmmoToReserve(AmmoInventory.AmmoType.primaryAmmo, attackKillSpecialAmount);
		}

		var circleCast = Physics2D.OverlapCircleAll((Vector2) transform.position + aim.MoveAimDir * SpecialAttackDistance, GetHitRange(attackType));

		foreach (var hit2D in circleCast)
		{
			var life = hit2D.gameObject.GetComponent<Life>();
			if(life == null) continue;
			if (!life.isEnemyOf(attacker) || life.cantDie || life.IsObstacle) return;
			HitTarget(life.Attack2Damage, life, .4f);
			connect = true;
			ammo.AddAmmoToReserve(AmmoInventory.AmmoType.primaryAmmo, attackKillSpecialAmount);
			ObjectMaker.Make(FX.Assets.hits.GetRandom(), hit2D.transform.position);
		}

		if (!connect) return;
		OnAttackHit?.Invoke();
	}

	private bool DidCollideWithBuilding(Vector3 position, Vector3 targetPoint, out RaycastHit2D[] raycast)
	{
		var hitObstacle = Physics2D.Linecast(position, targetPoint, ASSETS.LevelAssets.BuildingLayer);
		if (hitObstacle) targetPoint = hitObstacle.point;

		raycast = Physics2D.CircleCastAll(position, SpecialAttackWidth, aim.MoveAimDir, SpecialAttackDistance, ASSETS.LevelAssets.EnemyLayer);
		Debug.DrawRay((Vector2) position, aim.MoveAimDir * SpecialAttackDistance, Color.red);
		transform.position = targetPoint;
		return raycast.Length > 0;
	}

	private float GetHitRange(int attackType)
	{
		if (attackType == 5)
			return life.AttackRange * 2;
		return life.AttackRange;
	}
}