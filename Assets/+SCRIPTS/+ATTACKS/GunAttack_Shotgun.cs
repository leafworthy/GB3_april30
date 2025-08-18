using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class GunAttack_Shotgun : Attacks, IAimableGun
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public event Action OnEmpty;
		public bool isGlocking => false;
		public bool isReloading { get; set; }

		public bool isShooting;
		public int numberOfShots = 4;

		private float currentCooldownTime;
		private GunAimAbility_Simple aim;
		private AmmoInventory ammoInventory;
		public override string VerbName => "Shotgunning";
		private float currentDamage => attacker.PrimaryAttackDamageWithExtra;

		private Ammo GetCorrectAmmoType() => ammoInventory.primaryAmmo;

		private bool isPressing;
		private bool isEmpty;
		private bool isGlocking1;
		private bool isReloading1;
		private float spread = .25f;

		public override void SetPlayer(Player _player)
		{
			ammoInventory = GetComponent<AmmoInventory>();
			anim.animEvents.OnAnimationComplete += Anim_OnComplete;

			aim = GetComponent<GunAimAbility_Simple>();
			ListenToPlayer();
		}


		private void Anim_OnComplete()
		{
			StopShooting();
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
		}

		private void StopListeningToPlayer()
		{
			var player = attacker.player;
			if (player == null) return;
			attacker.OnDying -= OnDead;
			player.Controller.OnAttack1_Pressed -= PlayerControllerShootPress;
			player.Controller.OnAttack1_Released -= PlayerControllerShootRelease;
		}

		private void FixedUpdate()
		{
			if (pauseManager.IsPaused) return;

			if (isPressing) TryShooting();
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			isPressing = true;
			TryShooting();
		}

		private void TryShooting()
		{
			if (isShooting) return;
			if (anim == null) return;
			if (!GetCorrectAmmoType().hasAmmoInClip())
			{
				if (!GetCorrectAmmoType().hasAmmoInReserveOrClip()) StopShooting();

				if (isEmpty) return;
				OnEmpty?.Invoke();
				isEmpty = true;
				return;
			}

			ShootTarget(aim.AimDir);
		}

		private void StopShooting()
		{
			if (!isShooting) return;
			isShooting = false;
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressing = false;
			StopShooting();
		}

		private void ShootTarget(Vector3 targetPosition)
		{

			isShooting = true;
			anim.SetTrigger(UnitAnimations.ShootingTrigger);
			for (var i = 0; i < numberOfShots; i++)
			{
				var randomSpread = new Vector3(UnityEngine.Random.Range(-spread, spread), UnityEngine.Random.Range(-spread, spread), 0);
				ShootBullet(targetPosition + randomSpread);
			}
		}

		private void ShootBullet(Vector3 targetPosition)
		{
			if (!GetCorrectAmmoType().hasAmmoInReserveOrClip()) return;
			GetCorrectAmmoType().UseAmmo(1);
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
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, attacker.PrimaryAttackRange,
				body.isOverLandable ? assetManager.LevelAssets.EnemyLayerOnLandable : assetManager.LevelAssets.EnemyLayer);
			return raycastHit;
		}
	}
}
