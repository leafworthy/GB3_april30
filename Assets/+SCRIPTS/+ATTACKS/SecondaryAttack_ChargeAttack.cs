using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class SecondaryAttack_ChargeAttack : Attacks
	{
		private AnimationEvents animEvents;
		private Animations anim;
		private Body body;
		private MoveAbility move;
		private AimAbility aim;

		private bool isCharging;

		private AmmoInventory ammo;
		private Life life;

		private float SpecialAttackDistance = 3;
		private float SpecialAttackWidth = 3;
		private bool isFullyCharged;
		private GameObject currentArrowHead;
		public override string VerbName => "ChargeAttack";
		public event Action OnAttackHit;
		public event Action OnSpecialAttackHit;
		public event Action OnChargePress;
		public event Action OnChargeStop;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			move = GetComponent<MoveAbility>();
			body = GetComponent<Body>();
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<AimAbility>();

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
			if (attacker != null) return;
			attacker.player.Controller.Attack2LeftTrigger.OnPress -= Player_ChargePress;
			attacker.player.Controller.Attack2LeftTrigger.OnRelease -= Player_ChargeRelease;
			if (animEvents != null) return;
			animEvents = anim.animEvents;
			animEvents.OnFullyCharged -= Anim_FullyCharged;
			animEvents.OnAttackHit -= Anim_AttackHit;
		}

		private void Update()
		{
			if (isCharging)
			{
				ShowAiming();
				body.BottomFaceDirection(move.MoveAimDir.x > 0);
			}
			else
				HideAiming();
		}


		private void SpawnFX()
		{
			currentArrowHead = objectMaker.Make(assets.FX.nadeTargetPrefab);
			currentArrowHead.SetActive(false);
		}

		private void ShowAiming()
		{
			currentArrowHead.SetActive(true);
			currentArrowHead.transform.position = move.GetMoveAimPoint();
		}

		private void HideAiming()
		{
			currentArrowHead.SetActive(false);
		}

		private void Player_ChargePress(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			if (!body.arms.Do(this)) return;

			if (!body.legs.Do(this))
			{
				body.arms.Stop(this);
				return;
			}

			isCharging = true;
			isFullyCharged = false;
			OnChargePress?.Invoke();

			anim.SetTrigger(Animations.ChargeStartTrigger);
			anim.SetBool(Animations.IsCharging, true);
		}

		private void Player_ChargeRelease(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			if (!isCharging) return;
			OnChargeStop?.Invoke();

			isCharging = false;

			if (isFullyCharged)
			{

				ammo.secondaryAmmo.UseAmmo(1000);
				anim.SetTrigger(Animations.ChargeAttackTrigger);
				anim.SetBool(Animations.IsCharging, false);
			}
			else
			{
				anim.SetBool(Animations.IsCharging, false);
				ammo.secondaryAmmo.UseAmmo(1000);
				body.arms.Stop(this);
				body.legs.Stop(this);
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
			var targetPoint = move.IsIdle() ? aim.GetAimPoint() : ((Vector2) position + (Vector2)move.GetLastMoveAimDirOffset());

			var connect = false;
			SpecialAttackDistance = Vector2.Distance(position, targetPoint);
			body.arms.Stop(this);
			body.legs.Stop(this);
			MoveIfDidCollideWithBuilding(position, targetPoint, out var raycast);
			OnSpecialAttackHit?.Invoke();

			foreach (var raycastHit2D in raycast)
			{
				var otherLife = raycastHit2D.collider.GetComponent<Life>();
				if (!otherLife.isEnemyOf(attacker) || otherLife.cantDie || otherLife.IsObstacle) return;
				HitTarget(life.SecondaryAttackDamageWithExtra, otherLife, 3);
			}

			var circleCast = Physics2D.OverlapCircleAll(
				(Vector2)transform.position + move.MoveAimDir * SpecialAttackDistance, GetHitRange(attackType));

			foreach (var hit2D in circleCast)
			{
				var otherLife = hit2D.gameObject.GetComponent<Life>();
				if (otherLife == null) continue;
				if (!otherLife.isEnemyOf(attacker) || otherLife.cantDie || otherLife.IsObstacle) return;
				HitTarget(otherLife.SecondaryAttackDamageWithExtra, otherLife, 2);
				connect = true;
				objectMaker.Make( assets.FX.hits.GetRandom(), hit2D.transform.position);
			}

			if (!connect) return;
			OnAttackHit?.Invoke();
		}

		private void MoveIfDidCollideWithBuilding(Vector3 position, Vector3 targetPoint, out RaycastHit2D[] raycast)
		{
			var hitObstacle = Physics2D.Linecast(position, targetPoint, assets.LevelAssets.BuildingLayer);
			if (hitObstacle) targetPoint = hitObstacle.point;

			raycast = Physics2D.CircleCastAll(position, SpecialAttackWidth, move.MoveAimDir, SpecialAttackDistance,
				 assets.LevelAssets.EnemyLayer);
			transform.position = targetPoint;
		}

		private float GetHitRange(int attackType)
		{
			if (attackType == 5)
				return life.PrimaryAttackRange * 2;
			return life.PrimaryAttackRange;
		}
	}
}
