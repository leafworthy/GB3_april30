using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	public interface IAimableGun
	{
		event Action<Attack, Vector2> OnShotHitTarget;
		event Action<Attack, Vector2> OnShotMissed;
		bool isGlocking { get; }
		Vector2 AimDir { get; }
	}

	[Serializable]
	public class DoableGunAttack_AK_Glock : Ability, IAimableGun
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public bool isGlocking { get; private set; }
		public bool isPressingShoot;

		private float currentCooldownTime;

		private IAimAbility gunAimAbility => _gunAimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _gunAimAbility;
		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;
		public override string VerbName => "Shooting";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		private float currentAttackRate => isGlocking ? life.UnlimitedAttackRate : life.PrimaryAttackRate;

		private float currentDamage =>
			isGlocking ? life.UnlimitedAttackDamageWithExtra : life.PrimaryAttackDamageWithExtra;

		public Vector2 AimDir => gunAimAbility.AimDir;

		public Ammo GetCorrectAmmoType() => isGlocking ? ammoInventory.unlimitedAmmo : ammoInventory.primaryAmmo;

		private bool isEmpty;
		private bool isGlockingOnPurpose;
		public event Action OnNeedReload;
		public event Action OnEmpty;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			ListenToPlayer();
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void OnDead(Player _player, Life _life)
		{
			Stop();
		}

		public override bool canDo()
		{
			if (!base.canDo()) return false;
			return true;
		}

		protected override void DoAbility()
		{
			ShootWithCooldown(gunAimAbility.AimDir);
		}

		private void OnDestroy()
		{
			StopListeningToPlayer();
		}

		private void ListenToPlayer()
		{
			life.OnDying += OnDead;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		private void StopListeningToPlayer()
		{
			life.OnDying -= OnDead;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		private void FixedUpdate()
		{
			if (pauseManager.IsPaused) return;
			if (isPressingShoot) Do();
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			if (canDo()) isPressingShoot = true;
			Do();
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
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
			currentCooldownTime = Time.time + currentAttackRate;
			var hitObject = CheckRaycastHit(targetPosition);
			if (hitObject)
				ShotHitTarget(hitObject);
			else
				ShotMissed();
		}

		private void ShotMissed()
		{
			var missPosition = (Vector2) body.FootPoint.transform.position + gunAimAbility.AimDir.normalized * life.PrimaryAttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, gunAimAbility.AimDir.normalized, life.PrimaryAttackRange,
				assets.LevelAssets.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = new Attack(life, body.FootPoint.transform.position, missPosition, null, 0);

			OnShotMissed?.Invoke(newAttack, body.AttackStartPoint.transform.position);
		}

		private void ShotHitTarget(RaycastHit2D hitObject)
		{
			var target = hitObject.collider.gameObject.GetComponentInChildren<Life>();
			if (target == null) return;
			var newAttack = new Attack(life, body.FootPoint.transform.position, hitObject.point, target, currentDamage);
			OnShotHitTarget?.Invoke(newAttack, body.AttackStartPoint.transform.position);
			target.TakeDamage(newAttack);
		}


		private RaycastHit2D CheckRaycastHit(Vector2 targetDirection)
		{
			if (body.isOverLandable)
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, life.PrimaryAttackRange,
					assets.LevelAssets.EnemyLayerOnLandable);

				return raycastHit;
			}
			else
			{
				var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, life.PrimaryAttackRange,
					assets.LevelAssets.EnemyLayer);
				return raycastHit;
			}
		}

		private void ShootWithCooldown(Vector3 targetDir)
		{
			if (!(Time.time >= currentCooldownTime)) return;

			if (!GetCorrectAmmoType().hasAmmoInReserveOrClip())
			{
				Stop();
				OnEmpty?.Invoke();
				return;
			}

			if (!GetCorrectAmmoType().hasAmmoInClip())
			{
				Stop();
				OnNeedReload?.Invoke();
				return;
			}

			GetCorrectAmmoType().UseAmmo(1);

			ShootTarget(targetDir);
			PlayAnimationClip(null,1,.2f);
		}



	}
}
