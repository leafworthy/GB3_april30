using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack : Attacks
	{

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
		private float attackRate => isGlocking ? attacker.UnlimitedAttackRate : attacker.PrimaryAttackRate;
		private float currentDamage => isGlocking ? attacker.UnlimitedAttackDamageWithExtra : attacker.PrimaryAttackDamageWithExtra;

		private AmmoInventory.AmmoType ammoType =>
			isGlocking ? AmmoInventory.AmmoType.unlimited : AmmoInventory.AmmoType.primaryAmmo;

		public AnimationClip ReloadAKAnimationClip;
		public AnimationClip ReloadGlockAnimationClip;
		public bool isReloading;
		private bool isPressing;
		private bool isEmpty;
		public event Action OnEmpty;


		private void OnValidate()
		{
			attacker = GetComponent<Life>();
		}

		public void Start()
		{
			ammoInventory = GetComponent<AmmoInventory>();
			anim = GetComponent<Animations>();
			anim.animEvents.OnReload += Anim_OnReload;
			anim.animEvents.OnReloadStop += Anim_OnReloadStop;

			body = GetComponent<Body>();
			arms = body.arms;

			ListenToPlayer();
			aim = GetComponent<GunAimAbility>();
			attacker.OnDying += OnDead;
		}

		private void Anim_OnReloadStop()
		{
			StopReloading();
		}

		private void OnDisable()
		{
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
			arms.Stop("Reload");
			if (!isPressing)
			{
				anim.SetBool(Animations.IsBobbing, true);
				return;
			}

			TryShooting();
		}

		private void ListenToPlayer()
		{
			if (attacker == null) attacker = GetComponent<Life>();
			var player = attacker.player;
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
			//player.Controller.SwapWeaponSquare.OnPress += Player_SwapWeapon;
		}

		private void StopListeningToPlayer()
		{
			if (attacker == null) attacker = GetComponent<Life>();
			attacker.OnDying -= OnDead;
			var player = attacker.player;
			if (player == null) return;

			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress -= Player_Reload;
		}

		private void FixedUpdate()
		{
			if (PauseManager.IsPaused) return;
			if (arms.currentActivity != VerbName)
			{
				isShooting = false;
				//isPressing = false;
				//return;
			}
			if(isPressing) TryShooting();
			if (isShooting) ShootWithCooldown(aim.AimDir);
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			if (PauseManager.IsPaused) return;
			isPressing = true;
			TryShooting();
		}

		private void TryShooting()
		{
	
			if(isReloading) return;
			UseCorrectWeapon();
			if (!ammoInventory.HasAmmoInClip(ammoType))
			{
				if (!ammoInventory.HasAmmoInReserveOrClip(ammoType))
				{
					StopShooting();
					if(isEmpty) return;
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

			if (!arms.Do(VerbName)) return;
			StartShooting();
		}

		private void StartShooting()
		{
		
			isShooting = true;
			anim.SetBool(Animations.IsShooting, true);
			anim.SetBool(Animations.IsBobbing, false);
		}




		private void StopShooting()
		{
			isShooting = false;

			if (arms != null) arms.Stop(VerbName);
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
			var missPosition = (Vector2)body.FootPoint.transform.position + aim.AimDir.normalized * attacker.PrimaryAttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, aim.AimDir.normalized,
				attacker.PrimaryAttackRange,
				ASSETS.LevelAssets.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
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
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized,
					attacker.PrimaryAttackRange,
					ASSETS.LevelAssets.EnemyLayerOnLandable);

				Debug.DrawRay(body.FootPoint.transform.position, targetDirection.normalized * attacker.PrimaryAttackRange,
					Color.red, 1f);
				return raycastHit;
			}
			else
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized,
					attacker.PrimaryAttackRange,
					ASSETS.LevelAssets.EnemyLayer);
				return raycastHit;
			}
		}

		private void ShootWithCooldown(Vector3 targetDir)
		{
			if (targetDir == Vector3.zero) return;

			if (!(Time.time >= currentCooldownTime)) return;

			if (!ammoInventory.HasAmmoInClip(ammoType))
			{
				StopShooting();
				StartReloading();
				return;
			}

			ammoInventory.UseAmmo(ammoType, 1);
			currentCooldownTime = Time.time + attackRate;
			ShootTarget(targetDir);
		}

		private void Player_SwapWeapon(NewControlButton newControlButton)
		{
			StartSwapping();
		}

		private void StartSwapping()
		{
			isGlocking = !isGlocking;
		}

		private void Player_Reload(NewControlButton newControlButton)
		{
			StartReloading();
		}

		private void StartReloading()
		{
			if (isReloading)
			{
				Debug.Log("still reloading");
				return;
			}
			if (ammoInventory.clipIsFull(ammoType))
			{
				Debug.Log("Clip is full");
				return;
			}
			if (!ammoInventory.HasReserveAmmo(ammoType))
			{
				Debug.Log("has reserve ammo");
				return;
			}
			if (!arms.Do("Reload"))
			{
				return;
			}
			anim.SetBool(Animations.IsShooting, false);
			anim.SetBool(Animations.IsBobbing, false);
			isReloading = true;
			anim.Play(!isGlocking ? ReloadAKAnimationClip.name : ReloadGlockAnimationClip.name, 1, 0);
		}

		private void Anim_OnReload()
		{
			OnReload?.Invoke();
			ammoInventory.Reload(ammoType);
			isEmpty = false;
		}
	}
}