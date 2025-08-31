using System;
using GangstaBean.Core;
using UnityEngine;

public class ReloadActivity : IActivity
{
	public string VerbName => "Reloading";


}

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack_AK_Glock : Attacks, IAimableGun, IActivity
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public event Action OnReload;
		public bool isGlocking { get; private set; }
		public bool isReloading { get; private set; }
		public bool isShooting;
		private float currentCooldownTime;
		private ReloadActivity reloadActivity = new();

		private GunAimAbility aim;
		private Arms arms;
		private Body body;
		private UnitAnimations anim;
		private AmmoInventory ammoInventory;
		public override string VerbName => "Shooting";

		private float attackRate => isGlocking ? attacker.UnlimitedAttackRate : attacker.PrimaryAttackRate;

		private float currentDamage =>
			isGlocking ? attacker.UnlimitedAttackDamageWithExtra : attacker.PrimaryAttackDamageWithExtra;

		public Ammo GetCorrectAmmoType() => isGlocking ? ammoInventory.unlimitedAmmo : ammoInventory.primaryAmmo;
		public AnimationClip ReloadAKAnimationClip;
		public AnimationClip ReloadGlockAnimationClip;
		public AnimationClip ReloadGlockAnimationLeftClip;

		private bool isPressing;
		private bool isEmpty;
		private bool isGlockingOnPurpose;
		public event Action OnEmpty;

		public override void SetPlayer(Player player)
		{
			ammoInventory = GetComponent<AmmoInventory>();
			anim = GetComponent<UnitAnimations>();
			body = GetComponent<Body>();
			arms = body.arms;
			aim = GetComponent<GunAimAbility>();

			anim.animEvents.OnReload += Anim_OnReload;
			anim.animEvents.OnReloadStop += Anim_OnReloadStop;

			ListenToPlayer();
		}

		private void Anim_OnReloadStop()
		{

			StopReloading();
		}

		private void OnDisable()
		{
			anim.animEvents.OnReload -= Anim_OnReload;
			anim.animEvents.OnReloadStop -= Anim_OnReloadStop;
			StopListeningToPlayer();
		}

		private void OnDead(Player player, Life life)
		{
			StopShooting();
			StopReloading();
			StopListeningToPlayer();
		}

		private void StopReloading()
		{
			if (!isReloading) return;
			isReloading = false;
			arms.Stop( reloadActivity);
			if (!isPressing)
			{
				anim.SetBool(UnitAnimations.IsBobbing, true);
				return;
			}

			TryShooting();
		}

		private void ListenToPlayer()
		{
			var player = attacker.player;
			if (player == null) return;
			attacker.OnDying += OnDead;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
			player.Controller.SwapWeaponSquare.OnPress += Player_SwapWeapon;
		}

		private void StopListeningToPlayer()
		{
			var player = attacker.player;
			if (player == null) return;
			attacker.OnDying -= OnDead;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress -= Player_Reload;
			player.Controller.SwapWeaponSquare.OnPress -= Player_SwapWeapon;
		}

		private void FixedUpdate()
		{
			if (pauseManager.IsPaused) return;
			if (arms.currentActivity?.VerbName != this.VerbName) isShooting = false;

			if (isPressing) TryShooting();
			if (isShooting) ShootWithCooldown(aim.AimDir);
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			isPressing = true;
			TryShooting();
		}

		private void TryShooting()
		{
			if (isReloading)
			{

				return;
			}

			UseCorrectWeapon();
			if (!GetCorrectAmmoType().hasAmmoInClip())
			{
				if (!GetCorrectAmmoType().hasAmmoInReserveOrClip())
				{
					StopShooting();

					if (isEmpty) return;
					OnEmpty?.Invoke();
					isEmpty = true;

					return;
				}


				StartReloading();
				if (isEmpty) return;
				OnEmpty?.Invoke();
				isEmpty = true;
				return;
			}

			if (!arms.Do(this)) return;


			StartShooting();
		}

		private void StartShooting()
		{
			isShooting = true;
			anim.SetBool(UnitAnimations.IsShooting, true);
			anim.SetBool(UnitAnimations.IsBobbing, false);
		}

		private void StopShooting()
		{
			isShooting = false;

			arms.Stop(this);


			if (anim == null) return;
			anim.SetBool(UnitAnimations.IsShooting, false);
			anim.SetBool(UnitAnimations.IsBobbing, true);
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressing = false;
			StopShooting();
		}

		private void UseCorrectWeapon()
		{
			if (isGlockingOnPurpose) return;
			if (ammoInventory.primaryAmmo.hasAmmoInReserveOrClip())
			{
				isGlocking = false;
				return;
			}

			isGlocking = true;
		}

		private void ShootTarget(Vector3 targetPosition)
		{
			var hitObject = CheckRaycastHit(targetPosition);
			if (hitObject)
				ShotHitTarget(hitObject);
			else
				ShotMissed();
		}

		private void ShotMissed()
		{
			var missPosition = (Vector2) body.FootPoint.transform.position +
			                   aim.AimDir.normalized * attacker.PrimaryAttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, aim.AimDir.normalized,
				attacker.PrimaryAttackRange, assets.LevelAssets.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = new Attack(attacker, body.FootPoint.transform.position, missPosition, null, 0);

			OnShotMissed?.Invoke(newAttack, body.AttackStartPoint.transform.position);
		}

		private void ShotHitTarget(RaycastHit2D hitObject)
		{
			var target = hitObject.collider.gameObject.GetComponentInChildren<Life>();
			if (target == null) return;
			var newAttack = new Attack(attacker, body.FootPoint.transform.position, hitObject.point, target,
				currentDamage);
			OnShotHitTarget?.Invoke(newAttack, body.AttackStartPoint.transform.position);
			target.TakeDamage(newAttack);
		}

		private RaycastHit2D CheckRaycastHit(Vector2 targetDirection)
		{
			if (body.isOverLandable)
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized,
					attacker.PrimaryAttackRange, assets.LevelAssets.EnemyLayerOnLandable);

				return raycastHit;
			}
			else
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized,
					attacker.PrimaryAttackRange, assets.LevelAssets.EnemyLayer);
				return raycastHit;
			}
		}

		private void ShootWithCooldown(Vector3 targetDir)
		{
			if (targetDir == Vector3.zero) return;

			if (!(Time.time >= currentCooldownTime)) return;

			if (!GetCorrectAmmoType().hasAmmoInClip())
			{
				StopShooting();
				StartReloading();
				return;
			}

			GetCorrectAmmoType().UseAmmo(1);
			currentCooldownTime = Time.time + attackRate;
			ShootTarget(targetDir);
		}

		private void Player_SwapWeapon(NewControlButton newControlButton)
		{
			if (!arms.isActive) StartSwapping();
		}

		private void StartSwapping()
		{
			isGlocking = !isGlocking;
			isGlockingOnPurpose = isGlocking;
		}

		private void Player_Reload(NewControlButton newControlButton)
		{
			StartReloading();
		}

		private void StartReloading()
		{
			if (isReloading)
			{

				return;
			}

			if (GetCorrectAmmoType().clipIsFull())
			{

				return;
			}

			if (!GetCorrectAmmoType().hasReserveAmmo())
			{

				return;
			}

			if (!arms.Do(reloadActivity)) return;
			anim.SetBool(UnitAnimations.IsShooting, false);
			anim.SetBool(UnitAnimations.IsBobbing, false);
			isReloading = true;
			if (isGlocking)
				anim.Play(aim.AimDir.x > 0 ? ReloadGlockAnimationClip.name : ReloadGlockAnimationLeftClip.name, 1, 0);
			else
				anim.Play(ReloadAKAnimationClip.name, 1, 0);
		}

		private void Anim_OnReload()
		{
			OnReload?.Invoke();
			GetCorrectAmmoType().Reload();
			isEmpty = false;
		}
	}
}
