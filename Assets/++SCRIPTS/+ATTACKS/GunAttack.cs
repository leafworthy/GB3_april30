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

		private float currentDamage =>
			isGlocking ? attacker.UnlimitedAttackDamageWithExtra : attacker.PrimaryAttackDamageWithExtra;

		
		
		public Ammo GetCorrectAmmoType() => isGlocking ? ammoInventory.unlimitedAmmo : ammoInventory.primaryAmmo;
		public AnimationClip ReloadAKAnimationClip;
		public AnimationClip ReloadGlockAnimationClip;
		public AnimationClip ReloadGlockAnimationLeftClip;
		public bool isReloading;
		private bool isPressing;
		private bool isEmpty;
		private bool isGlockingOnPurpose;
		private string ReloadVerb => isGlocking ? "ReloadGlock" : "ReloadAK";
		public event Action OnEmpty;

		public override void SetPlayer(Player player)
		{
			attacker = GetComponent<Life>();
			ammoInventory = GetComponent<AmmoInventory>();
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			arms = body.arms;
			aim = GetComponent<GunAimAbility>();

			anim.animEvents.OnReload += Anim_OnReload;
			anim.animEvents.OnReloadStop += Anim_OnReloadStop;


			ListenToPlayer();
		}


		private void Anim_OnReloadStop()
		{
			Debug.Log("reload stop anim");
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
			arms.StopSafely(ReloadVerb);
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
			attacker.OnDying += OnDead;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress += Player_Reload;
			player.Controller.SwapWeaponSquare.OnPress += Player_SwapWeapon;
		}

		private void StopListeningToPlayer()
		{
			if (attacker == null) attacker = GetComponent<Life>();
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
			if (PauseManager.I.IsPaused) return;
			if (arms.currentActivity != VerbName)
			{
				isShooting = false;
				//isPressing = false;
				//return;
			}

			if (isPressing) TryShooting();
			if (isShooting) ShootWithCooldown(aim.AimDir);
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			isPressing = true;
			TryShooting();
		}

		private void TryShooting()
		{
			if (isReloading)
			{
				Debug.Log("cant shoot, reloading");
				return;
			}

			UseCorrectWeapon();
			if (!GetCorrectAmmoType().hasAmmoInClip())
			{
				if (!GetCorrectAmmoType().hasAmmoInReserveOrClip())
				{
					StopShooting();
					Debug.Log("ammo just emptied");
					if (isEmpty) return;
					OnEmpty?.Invoke();
					isEmpty = true;

					return;
				}

				Debug.Log("no ammo, start reloading");
				StartReloading();
				if (isEmpty) return;
				OnEmpty?.Invoke();
				isEmpty = true;
				return;
			}

			if (!arms.Do(VerbName))
			{
				Debug.Log("can't gun attack because busy with  " + arms.currentActivity);
				return;
			}

			Debug.Log("actually start shooting");
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

			arms.StopSafely(VerbName);
			Debug.Log("stop shooting");

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
			if (isGlockingOnPurpose) return;
			if (ammoInventory.primaryAmmo.hasAmmoInReserveOrClip( ))
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
			var missPosition = (Vector2)body.FootPoint.transform.position +
			                   aim.AimDir.normalized * attacker.PrimaryAttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, aim.AimDir.normalized,
				attacker.PrimaryAttackRange,
				ASSETS.LevelAssets.BuildingLayer);
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
					attacker.PrimaryAttackRange,
					ASSETS.LevelAssets.EnemyLayerOnLandable);

				Debug.DrawRay(body.FootPoint.transform.position,
					targetDirection.normalized * attacker.PrimaryAttackRange, Color.red, 1f);
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
				Debug.Log("still reloading");
				return;
			}

			if (GetCorrectAmmoType().clipIsFull())
			{
				Debug.Log("Clip is full");
				return;
			}

			if (!GetCorrectAmmoType().hasReserveAmmo())
			{
				Debug.Log("has reserve ammo");
				return;
			}

			if (!arms.Do(ReloadVerb)) return;
			anim.SetBool(Animations.IsShooting, false);
			anim.SetBool(Animations.IsBobbing, false);
			isReloading = true;
			if (isGlocking)
			{
				anim.Play(aim.AimDir.x > 0 ? ReloadGlockAnimationClip.name : ReloadGlockAnimationLeftClip.name, 1, 0);
			}
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