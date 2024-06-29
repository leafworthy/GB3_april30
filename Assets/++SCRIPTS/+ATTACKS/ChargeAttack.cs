using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : Attacks
{
	private AnimationEvents animEvents;
	private Animations anim;
	private Body body;
	private GunAimer aim;

	private bool isCharging;

	private AmmoInventory ammo;
	private UnitStats stats;

	private int attackKillSpecialAmount = 30;
	private float SpecialAttackDistance = 3;
	private float SpecialAttackWidth = 3;
	private bool isFullyCharged;
	private GameObject currentArrowHead;
	private string verbName = "ChargeAttack";
	private float attackDamage = 100;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		aim = GetComponent<GunAimer>();
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		stats = GetComponent<UnitStats>();
		ammo = GetComponent<AmmoInventory>();
		


		if (attacker != null)
		{
			attacker.player.Controller.Reload1.OnPress += Player_ChargePress;
			attacker.player.Controller.Reload1.OnRelease += Player_ChargeRelease;
		}

		animEvents = anim.animEvents;
		animEvents.OnFullyCharged += Anim_FullyCharged;
		animEvents.OnAttackHit += Anim_AttackHit;
		SpawnFX();
	}

	private void Update()
	{
		if (isCharging)
		{
			ShowAiming();
			body.BottomFaceDirection(aim.AimDir.x>0);
		}
		else
			HideAiming();
	}

	private void SpawnFX()
	{
		currentArrowHead = Maker.Make(ASSETS.FX.nadeTargetPrefab);
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
		if (Game_GlobalVariables.IsPaused) return;
		// if (!ammo.HasFullAmmo(AmmoInventory.AmmoType.primaryAmmo)) return;
		if (!body.arms.Do(verbName))
		{
			return;
		}

		if (!body.legs.Do(verbName))
		{
			return;
		}
		isCharging = true;
		isFullyCharged = false;
		ASSETS.sounds.brock_charge_sounds.PlayRandom();
		anim.SetTrigger(Animations.ChargeStartTrigger);
		anim.SetBool(Animations.IsCharging, true);
	}

	private void Player_ChargeRelease(NewControlButton newControlButton)
	{
		if (Game_GlobalVariables.IsPaused) return;
		if (!isCharging) return;

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
		if (!DidCollide(position, targetPoint, out var raycast)) return;
		ASSETS.sounds.brock_homerunhit_sounds.PlayRandom();

		foreach (var raycastHit2D in raycast)
		{
			HitTarget(attackDamage, raycastHit2D.collider, false);
			ammo.AddAmmoToReserve(AmmoInventory.AmmoType.primaryAmmo, attackKillSpecialAmount);
		}


		var circleCast = Physics2D.OverlapCircleAll(
			(Vector2) transform.position + (Vector2) aim.AimDir * SpecialAttackDistance,
			GetHitRange(attackType));

		foreach (var hit2D in circleCast)
		{
			HitTarget(attackType, hit2D, true);
			connect = true;
			ammo.AddAmmoToReserve(AmmoInventory.AmmoType.primaryAmmo, attackKillSpecialAmount);
		}

		if (!connect) return;
		ASSETS.sounds.brock_bathit_sounds.PlayRandom();
		Maker.Make(ASSETS.FX.hits.GetRandom());
	}

	private bool DidCollide(Vector3 position, Vector3 targetPoint, out RaycastHit2D[] raycast)
	{
		var hitObstacle = Physics2D.Linecast((Vector2) position, targetPoint, ASSETS.LevelAssets.BuildingLayer);
		if (hitObstacle) targetPoint = hitObstacle.point;

		raycast = Physics2D.CircleCastAll((Vector2) position, SpecialAttackWidth, aim.AimDir, SpecialAttackDistance,
			ASSETS.LevelAssets.EnemyLayer);
		Debug.DrawRay((Vector2) position, (Vector2) aim.AimDir * SpecialAttackDistance, Color.red);
		transform.position = targetPoint;
		return raycast.Length > 0;
	}

	private float GetHitRange(int attackType)
	{
		if (attackType == 5)
			return stats.AttackRange * 2;
		else
			return stats.AttackRange;
	}

	
}