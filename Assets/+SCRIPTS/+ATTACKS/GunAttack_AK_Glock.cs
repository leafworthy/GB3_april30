using System;
using __SCRIPTS;
using UnityEngine;

public class ReloadAbilityController : AbilityController
{
	private ReloadAbility ability => _ability ??= GetComponent<ReloadAbility>();
	private ReloadAbility _ability;

	protected override void StopListeningToPlayer()
	{
		if (anim == null) return;
		anim.animEvents.OnReload -= Anim_OnReload;
	}

	protected override void ListenToPlayer()
	{
		if (anim == null) return;
		anim.animEvents.OnReload += Anim_OnReload;
		player.spawnedPlayerLife.OnDying += Player_OnDead;
	}

	private void Player_OnDead(Player _player, Life _life)
	{
		ability.StopActivity();
	}

	private void Anim_OnReload()
	{
		ability.Reload();
	}
}
public class ReloadAbility : DoableActivity
{
	public event Action OnReload;
	private IAimAbility aim;

	public AnimationClip ReloadAKAnimationClip;
	public AnimationClip ReloadGlockAnimationClip;
	public AnimationClip ReloadGlockAnimationLeftClip;

	private bool isReloading;
	private IAimAndGlock reloader => _reloader ??= GetComponent<IAimAndGlock>();
	private IAimAndGlock _reloader;
	private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
	private AmmoInventory _ammoInventory;
	public override string VerbName => "Reloading";
	protected override bool requiresArms() => true;

	protected override bool requiresLegs() => false;

	public override void StopActivity()
	{
		isReloading = false;
		base.StopActivity();
	}

	public override void StartActivity()
	{
		PlayAnimationClip(ReloadGlockAnimationClip);
		isReloading = true;
	}

	public void Reload()
	{
		if (!isReloading) return;
		if (ammoInventory.GetCorrectAmmoType(reloader.isGlocking).clipIsFull()) return;

		if (!ammoInventory.GetCorrectAmmoType(reloader.isGlocking).hasReserveAmmo()) return;

		anim.SetBool(UnitAnimations.IsShooting, false);
		anim.SetBool(UnitAnimations.IsBobbing, false);

		if (reloader.isGlocking)
			anim.Play(aim.AimDir.x > 0 ? ReloadGlockAnimationClip.name : ReloadGlockAnimationLeftClip.name, 1, 0);
		else
			anim.Play(ReloadAKAnimationClip.name, 1, 0);

		OnReload?.Invoke();
	}

	protected override void AnimationComplete()
	{
		if (!isReloading) return;
		isReloading = false;

		anim.SetBool(UnitAnimations.IsBobbing, true);
		base.AnimationComplete();
	}

}

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack_AK_Glock_Ability : Attacks, IAimAndGlock
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public bool isReloading { get; set; }
		public bool isGlocking { get; set; }
		public bool isShooting;
		private float currentCooldownTime;
		private float attackRate => isGlocking ? attacker.UnlimitedAttackRate : attacker.PrimaryAttackRate;

		public event Action OnAutoReload;
	}

	[Serializable]
	public class GunAttack_AK_Glock : Attacks, IAimAndGlock
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public bool isGlocking { get; private set; }
		public bool isReloading { get; private set; }
		public bool isShooting;
		private float currentCooldownTime;

		private GunAimAbility aim;
		private DoableArms doableArms;
		private AmmoInventory ammoInventory;
		public override string VerbName => "Shooting";

		private float attackRate => isGlocking ? attacker.UnlimitedAttackRate : attacker.PrimaryAttackRate;

		private float currentDamage =>
			isGlocking ? attacker.UnlimitedAttackDamageWithExtra : attacker.PrimaryAttackDamageWithExtra;

		public AnimationClip ReloadAKAnimationClip;
		public AnimationClip ReloadGlockAnimationClip;
		public AnimationClip ReloadGlockAnimationLeftClip;

		private bool isPressing;
		private bool isEmpty;
		private bool isGlockingOnPurpose;
		public event Action OnAutoReload;
		public event Action OnEmpty;

		public override void SetPlayer(Player player)
		{
			base.SetPlayer(player);
			ammoInventory = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();
			ListenToPlayer();
		}



		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void OnDead(Player player, Life life)
		{
			StopShooting();
			StopListeningToPlayer();
		}

		private void ListenToPlayer()
		{
			var player = attacker.player;
			if (player == null) return;
			attacker.OnDying += OnDead;
			player.Controller.OnAttack1_Pressed += PlayerControllerShootPress;
			player.Controller.OnAttack1_Released += PlayerControllerShootRelease;
			player.Controller.OnSwapWeapon_Pressed += Player_SwapWeapon;
		}

		private void StopListeningToPlayer()
		{
			var player = attacker.player;
			if (player == null) return;
			attacker.OnDying -= OnDead;
			player.Controller.OnAttack1_Pressed -= PlayerControllerShootPress;
			player.Controller.OnAttack1_Released -= PlayerControllerShootRelease;
			player.Controller.OnSwapWeapon_Pressed -= Player_SwapWeapon;
		}

		private void FixedUpdate()
		{
			if (pauseManager.IsPaused) return;
			if (doableArms.currentActivity?.VerbName != VerbName) isShooting = false;

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
			if (isReloading) return;

			UseCorrectWeapon();
			if (!ammoInventory.GetCorrectAmmoType(isGlocking).hasAmmoInClip())
			{
				if (!ammoInventory.GetCorrectAmmoType(isGlocking).hasAmmoInReserveOrClip())
				{
					StopShooting();

					if (isEmpty) return;
					OnEmpty?.Invoke();
					isEmpty = true;

					return;
				}

				OnAutoReload?.Invoke();
				if (isEmpty) return;
				OnEmpty?.Invoke();
				isEmpty = true;
				return;
			}


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

			doableArms.Stop(this);

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
			var missPosition = (Vector2) body.FootPoint.transform.position + (Vector2) aim.AimDir.normalized * attacker.PrimaryAttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, aim.AimDir.normalized, attacker.PrimaryAttackRange,
				assetManager.LevelAssets.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = new Attack(attacker, body.FootPoint.transform.position, missPosition, null, 0);

			OnShotMissed?.Invoke(newAttack, body.AttackStartPoint.transform.position);
		}

		private void ShotHitTarget(RaycastHit2D hitObject)
		{
			var target = hitObject.collider.gameObject.GetComponentInChildren<Life>();
			if (target == null) return;
			var newAttack = new Attack(attacker, body.FootPoint.transform.position, hitObject.point, target, currentDamage);
			OnShotHitTarget?.Invoke(newAttack, body.AttackStartPoint.transform.position);
			target.TakeDamage(newAttack);
		}

		private RaycastHit2D CheckRaycastHit(Vector2 targetDirection)
		{
			if (body.isOverLandable)
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, attacker.PrimaryAttackRange,
					assetManager.LevelAssets.EnemyLayerOnLandable);

				return raycastHit;
			}
			else
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, attacker.PrimaryAttackRange, assetManager.LevelAssets.EnemyLayer);
				return raycastHit;
			}
		}

		private void ShootWithCooldown(Vector3 targetDir)
		{
			if (targetDir == Vector3.zero) return;

			if (!(Time.time >= currentCooldownTime)) return;

			if (!ammoInventory.GetCorrectAmmoType(isGlocking).hasAmmoInClip())
			{
				StopShooting();
				OnAutoReload?.Invoke();
				return;
			}

			ammoInventory.GetCorrectAmmoType(isGlocking).UseAmmo(1);
			currentCooldownTime = Time.time + attackRate;
			ShootTarget(targetDir);
		}

		private void Player_SwapWeapon(NewControlButton newControlButton)
		{
			if (!doableArms.isActive) StartSwapping();
		}

		private void StartSwapping()
		{
			isGlocking = !isGlocking;
			isGlockingOnPurpose = isGlocking;
		}





	}
}
