using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class SecondaryAttack_ChargeAttack : Attacks
	{
		public float chargeRate = 10;
		private AnimationEvents animEvents;
		private Animations anim;
		private Body body;
		private MoveAbility aim;

		private bool isCharging;

		private AmmoInventory ammo;
		private Life life;

		private float SpecialAttackDistance = 3;
		private float SpecialAttackWidth = 3;
		private bool isFullyCharged;
		private GameObject currentArrowHead;
		private string verbName = "ChargeAttack";
		private float specialAttackDamage = 200;
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
				attacker.player.Controller.Attack2LeftTrigger.OnPress += Player_ChargePress;
				attacker.player.Controller.Attack2LeftTrigger.OnRelease += Player_ChargeRelease;
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
				attacker.player.Controller.Attack2LeftTrigger.OnPress -= Player_ChargePress;
				attacker.player.Controller.Attack2LeftTrigger.OnRelease -= Player_ChargeRelease;
			}
		}

		private void Update()
		{
			if (isCharging)
			{
			
				ShowAiming();
				body.BottomFaceDirection(aim.MoveAimDir.x > 0);
				if(!isFullyCharged) AddChargingAmmo();
			}
			else
				HideAiming();
		}

		private void AddChargingAmmo()
		{
			if (ammo.tertiaryAmmo.hasFullReserve())
			{
				isFullyCharged = true;
				Debug.Log("fully charged here");
				return;
			}
			ammo.tertiaryAmmo.AddAmmoToReserve((int)Mathf.Round(chargeRate* Time.deltaTime));
			Debug.Log("charge added, now" +  ammo.tertiaryAmmo.reserveAmmo + " / " + ammo.tertiaryAmmo.maxReserveAmmo);
		}

		private void SpawnFX()
		{
			currentArrowHead = ObjectMaker.I.Make(ASSETS.FX.nadeTargetPrefab);
			currentArrowHead.SetActive(false);
		}

		private void ShowAiming()
		{
			currentArrowHead.SetActive(true);
			currentArrowHead.transform.position = aim.GetMoveAimPoint();
		}

		private void HideAiming()
		{
			currentArrowHead.SetActive(false);
		}

		private void Player_ChargePress(NewControlButton newControlButton)
		{
			if (PauseManager.IsPaused) return;
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
			if (PauseManager.IsPaused) return;
			if (!isCharging) return;
			OnChargeStop?.Invoke();
	
			isCharging = false;
			anim.SetBool(Animations.IsCharging, false);
			if (isFullyCharged)
			{
				ammo.tertiaryAmmo.UseAmmo(1000);
				anim.SetTrigger(Animations.ChargeAttackTrigger);
			}
			else
			{
				ammo.tertiaryAmmo.UseAmmo(1000);
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
			var targetPoint = aim.GetMoveAimPoint();
			var connect = false;
			SpecialAttackDistance = Vector2.Distance(position, targetPoint);
			body.arms.Stop(verbName);
			body.legs.Stop(verbName);
			if (!DidCollideWithBuilding(position, targetPoint, out var raycast)) return;
			OnSpecialAttackHit?.Invoke();

			foreach (var raycastHit2D in raycast)
			{
				var otherLife = raycastHit2D.collider.GetComponent<Life>();
				if (!otherLife.isEnemyOf(attacker) || otherLife.cantDie || otherLife.IsObstacle) return;
				HitTarget(life.SecondaryAttackDamageWithExtra, otherLife, 3);
			}

			var circleCast = Physics2D.OverlapCircleAll((Vector2) transform.position + aim.MoveAimDir * SpecialAttackDistance, GetHitRange(attackType));

			foreach (var hit2D in circleCast)
			{
				var otherLife = hit2D.gameObject.GetComponent<Life>();
				if(otherLife == null) continue;
				if (!otherLife.isEnemyOf(attacker) || otherLife.cantDie || otherLife.IsObstacle) return;
				HitTarget(otherLife.SecondaryAttackDamageWithExtra, otherLife, 2);
				connect = true;
				ObjectMaker.I.Make(ASSETS.FX.hits.GetRandom(), hit2D.transform.position);
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
				return life.PrimaryAttackRange * 2;
			return life.PrimaryAttackRange;
		}
	}
}