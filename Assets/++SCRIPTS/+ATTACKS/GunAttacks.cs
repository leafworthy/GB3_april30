using System;
using UnityEngine;

[Serializable]
public class GunAttacks : Attacks
{
	public override Color GetBackgroundColor() => Colors.Blue;

	public override string GetIconPath() => "Assets/Bullet_Icon.png";
	public event Action<Attack, Vector2> OnShotHitTarget;
	public event Action<Attack, Vector2> OnShotMissed;
	public event Action OnReload;
	public bool isGlocking { get; private set; }
	public bool isShooting;

	private float currentCooldownTime;

	private GunAimAbility aim;
	private Arms arms;
	private Body body;
	private Animations anim;
	private AmmoInventory ammoInventory;
	private string VerbName = "shooting";
	private float attackRate => isGlocking ? attacker.Attack2Rate : attacker.AttackRate;
	private float currentDamage => isGlocking ? attacker.Attack2Damage : attacker.AttackDamage;
	private AmmoInventory.AmmoType ammoType => isGlocking ? AmmoInventory.AmmoType.glock : AmmoInventory.AmmoType.primaryAmmo;
	private string ReloadAnimationClipName = "Top-Reload";
	private bool isReloading;
	private bool isPressing;
	private bool reloadComplete;
	private float reloadTime = .2f;
	private float reloadCounter;
	private float reloadCooldown = 1f;
	

	private void OnValidate()
	{
		attacker = GetComponent<Life>();
	}

	public void Start()
	{
		ammoInventory = GetComponent<AmmoInventory>();
		anim = GetComponent<Animations>();

		body = GetComponent<Body>();
		arms = body.arms;

		ListenToPlayer();
		aim = GetComponent<GunAimAbility>();
		attacker.OnDead += OnDead;
	}

	private void OnDisable()
	{
		StopListeningToPlayer();
	}

	private void OnDead(Player player)
	{
		StopShooting();
		StopReloading();
		StopListeningToPlayer();
	}

	private void ListenToPlayer()
	{
		if (attacker == null) attacker = GetComponent<Life>();
		var player = attacker.player;
		if (player == null) return;
		player.Controller.Attack1.OnPress += PlayerControllerShootPress;
		player.Controller.Attack1.OnRelease += PlayerControllerShootRelease;
		player.Controller.Reload1.OnPress += Player_Reload;
	}

	private void StopListeningToPlayer()
	{
		if (attacker == null) attacker = GetComponent<Life>();
		attacker.OnDead -= OnDead;
		var player = attacker.player;
		if (player == null) return;

		player.Controller.Attack1.OnPress -= PlayerControllerShootPress;
		player.Controller.Attack1.OnRelease -= PlayerControllerShootRelease;
		player.Controller.Reload1.OnPress -= Player_Reload;
	}

	private void FixedUpdate()
	{
		if (GlobalManager.IsPaused) return;
		if (isReloading)
		{
			if (reloadCounter > 0)
				reloadCounter -= Time.deltaTime;
			else
				StopReloading();

			if (!(reloadCounter < reloadCooldown - reloadTime) || reloadComplete) return;
			Anim_OnReload();
			reloadComplete = true;
			return;
		}

		if (arms.currentActivity != VerbName) return;
		if (isShooting) ShootWithCooldown(aim.AimDir);
		
		anim.SetBool(Animations.IsGlocking, isGlocking);
	}

	private void PlayerControllerShootPress(NewControlButton newControlButton)
	{
		if (GlobalManager.IsPaused) return;
		isPressing = true;
		TryShooting();
	}

	private void TryShooting()
	{
		if (IsOutOfAllAmmo())
		{
			StopShooting();
			return;
		}

		UseCorrectWeapon();

		if (!ammoInventory.HasAmmoInClip(ammoType))
		{
			StartReloading();
			return;
		}

		StartShooting();
	}

	private void StartShooting()
	{
		if (!arms.Do(VerbName)) return;
		isShooting = true;
		anim.SetBool(Animations.IsShooting, true);
		anim.SetBool(Animations.IsBobbing, false);
	}

	private bool IsOutOfAllAmmo()
	{
		if (!ammoInventory.HasAmmoInReserveOrClip(AmmoInventory.AmmoType.primaryAmmo) &&
		    !ammoInventory.HasAmmoInReserveOrClip(AmmoInventory.AmmoType.glock))
			return true;
		return false;
	}

	private void ChangeGuns()
	{
		isGlocking = !isGlocking;
	}

	private bool NeedsToChangeGuns()
	{
		if (isGlocking) return ammoInventory.HasAmmoInReserveOrClip(AmmoInventory.AmmoType.primaryAmmo);

		return !ammoInventory.HasAmmoInReserveOrClip(AmmoInventory.AmmoType.primaryAmmo);
	}

	private void StopShooting()
	{
		isShooting = false;
		if(arms != null) arms.Stop(VerbName);
		if (anim == null) return;
		anim.SetBool(Animations.IsShooting, false);
		anim.SetBool(Animations.IsBobbing, true);
	}

	private void PlayerControllerShootRelease(NewControlButton newControlButton)
	{
		isPressing = false;
		StopShooting();
	}

	private void UseCorrectWeapon()
	{
		if (ammoInventory.HasAmmoInReserveOrClip(AmmoInventory.AmmoType.primaryAmmo))
		{
			isGlocking = false;
			anim.SetBool(Animations.IsGlocking, isGlocking);
			return;
		}

		isGlocking = true;
		anim.SetBool(Animations.IsGlocking, isGlocking);
	}

	private void ShootTarget(Vector3 targetPosition)
	{
		var hitObject = CheckRaycastHit(targetPosition);
		if (isGlocking) anim.SetTrigger(Animations.GlockTrigger);
		if (hitObject)
			ShotHitTarget(hitObject);
		else
			ShotMissed();
	}

	private void ShotMissed()
	{
		var missPosition = (Vector2) body.FootPoint.transform.position + aim.AimDir.normalized * attacker.AttackRange;
		var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, aim.AimDir.normalized, attacker.AttackRange,
			ASSETS.LevelAssets.BuildingLayer);
		if (raycastHit)
		{
			missPosition = raycastHit.point;
		}
		var newAttack = new Attack(attacker, body.FootPoint.transform.position, missPosition, null, 0);

		OnShotMissed?.Invoke(newAttack, body.AttackStartPoint.transform.position);
		
		;
		
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
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, attacker.AttackRange,
				ASSETS.LevelAssets.EnemyLayerOnLandable);
			
			Debug.DrawRay( body.FootPoint.transform.position, targetDirection.normalized * attacker.AttackRange, Color.red, 1f);
			return raycastHit;
		}
		else
		{
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, attacker.AttackRange,
				ASSETS.LevelAssets.EnemyLayer);
			return raycastHit;
		}
	}

	private void ShootWithCooldown(Vector3 target)
	{
		
		if (!(Time.time >= currentCooldownTime)) return;

		if (!ammoInventory.HasAmmoInClip(ammoType))
		{
			StopShooting();
			StartReloading();
			return;
		}

		ammoInventory.UseAmmo(ammoType, 1);
		currentCooldownTime = Time.time + attackRate;
		ShootTarget(target);
	}

	private void Player_Reload(NewControlButton newControlButton)
	{
		StartReloading();
	}

	private void StartReloading()
	{
		if (isReloading) return;
		if (ammoInventory.clipIsFull(ammoType)) return;
		if (!ammoInventory.HasReserveAmmo(ammoType)) return;
		if (!arms.Do("Reload")) return;
		anim.SetBool(Animations.IsShooting, false);
		isReloading = true;
		reloadComplete = false;
		reloadCounter = reloadCooldown;
		anim.Play(ReloadAnimationClipName, 1, 0);
	}

	private void Anim_OnReload()
	{
		OnReload?.Invoke();
		ammoInventory.Reload(ammoType);
	}

	private void StopReloading()
	{
		if (!isReloading) return;
		isReloading = false;
		arms.Stop("Reload");
		if (!isPressing) return;
		TryShooting();
	}
}