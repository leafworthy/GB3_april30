using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class Gun : Weapon
	{
		static readonly string[] PrimaryAnimationClips =
		{
			"E",
			"EES",
			"ES",
			"SE",
			"SSE",
			"SSE",
			"SE",
			"ES",
			"EES",
			"E",
			"EEN",
			"EN",
			"NE",
			"NNE",
			"NNE",
			"NE",
			"EN",
			"EEN"
		};
		public event Action<Attack> OnShotHitTarget;
		public event Action<Attack> OnShotMissed;
		public event Action OnNeedsReload;
		public event Action OnEmpty;
		public event Action<Vector2> OnShoot;

		public AnimationClip simpleShootAnimationClip;
		protected AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		AmmoInventory _ammoInventory;
		IAimAbility gunAimAbility => _gunAimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _gunAimAbility;

		Body body => _body ??= GetComponent<Body>();
		Body _body;
		protected ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>();
		ICanAttack _attacker;

		float currentCooldownTime;
		Vector2 aimDir;
		string AnimationClipSuffix => this is PrimaryGun ? "" : "_Glock";
		public virtual float reloadTime => .5f;
		public abstract float AttackRate { get; }
		protected abstract float Damage { get; }
		protected abstract Ammo Ammo { get; }

		protected abstract float AttackRange { get; }
		protected abstract float Spread { get; }
		protected abstract int numberOfBulletsPerShot { get; }
		protected virtual bool simpleShoot => false;
		bool IsCoolingDown => Time.time <= currentCooldownTime;
		public Vector2 AimDir => gunAimAbility.AimDir;
		public bool CanReload() => Ammo.CanReload();
		public bool CanSwap() => Ammo.hasAmmoInReserveOrClip();
		public bool MustReload() => !Ammo.hasAmmoInClip();
		public void Reload() => Ammo.Reload();


		public bool HasAnyAmmo() => Ammo.hasAmmoInReserveOrClip();

		void Start()
		{
			currentCooldownTime = Time.time;
		}


		public bool Shoot()
		{
			if (!Ammo.hasAmmoInClip())
			{
				if (Ammo.CanReload()) OnNeedsReload?.Invoke();
				else
				{
					OnEmpty?.Invoke();
					Debug.Log("onempty");
				}

				return false;
			}

			if (IsCoolingDown) return false;
			currentCooldownTime = Time.time + AttackRate;
			OnShoot?.Invoke(gunAimAbility.AimDir);
			Ammo.UseAmmo(1);

			for (var i = 0; i < numberOfBulletsPerShot; i++)
			{
				var randomSpread = new Vector2(UnityEngine.Random.Range(-Spread, Spread), UnityEngine.Random.Range(-Spread, Spread));
				ShootBullet(gunAimAbility.AimDir + randomSpread);
			}


			return true;
		}

		float GetDegreesFromAimDir()
		{
			var radians = Mathf.Atan2(-gunAimAbility.AimDir.y, gunAimAbility.AimDir.x); // NEGATE y-axis
			var degrees = radians * Mathf.Rad2Deg;
			if (degrees < 0)
				degrees += 360f;
			return degrees;
		}

		public string GetClipNameFromDegrees()
		{
			var degrees = GetDegreesFromAimDir();
			var whichPortion = GetDirectionPortion(degrees);
			if (whichPortion > PrimaryAnimationClips.Length) whichPortion = 0;
			return PrimaryAnimationClips[whichPortion] + AnimationClipSuffix;
		}

		static int GetDirectionPortion(float degrees)
		{
			var normalizedDegrees = degrees % 360;
			if (normalizedDegrees < 0) normalizedDegrees += 360;
			var offsetDegrees = (normalizedDegrees + 10) % 360;
			var portionWidth = 360f / PrimaryAnimationClips.Length;
			var portionNumber = Mathf.FloorToInt(offsetDegrees / portionWidth);
			portionNumber = Mathf.Clamp(portionNumber, 0, PrimaryAnimationClips.Length - 1);
			return portionNumber;
		}

		void ShootBullet(Vector3 shootDirection)
		{
			var hitObject = Physics2D.LinecastAll(body.FootPoint.transform.position, body.FootPoint.transform.position + shootDirection * AttackRange,
				attacker.EnemyLayer);
			Debug.DrawLine(body.FootPoint.transform.position, body.FootPoint.transform.position + shootDirection * AttackRange, Color.green, 3);

			if (hitObject != null)
			{
				foreach (var hit in hitObject)
				{
					var target = hit.collider.gameObject.GetComponentInParent<IGetAttacked>();
					if (target == null) target = hit.collider.gameObject.GetComponentInChildren<IGetAttacked>();
					if (target == null) continue;
					if (!target.CanTakeDamage()) continue;
					ShotHitTarget(target, hit.point);
					return;
				}
			}

			ShotMissed();
		}

		void ShotMissed()
		{
			var missPosition = (Vector2) body.FootPoint.transform.position + gunAimAbility.AimDir.normalized * AttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, gunAimAbility.AimDir.normalized, AttackRange, attacker.player.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = Attack.Create(attacker, null).WithOriginPoint(body.AttackStartPoint.transform.position).WithDestinationPoint(missPosition)
			                      .WithDamage(0);

			Debug.DrawLine(body.FootPoint.transform.position, missPosition, Color.red, 3);
			OnShotMissed?.Invoke(newAttack);
		}

		void ShotHitTarget(IGetAttacked targetLife, Vector2 hitObjectPoint)
		{
			var newAttack = Attack.Create(attacker, targetLife).WithOriginPoint(body.AttackStartPoint.transform.position).WithDestinationPoint(hitObjectPoint)
			                      .WithDamage(Damage);
			OnShotHitTarget?.Invoke(newAttack);

			targetLife.TakeDamage(newAttack);
		}

		public string GetShootClipName()
		{
			if (simpleShoot) return simpleShootAnimationClip.name;
			return GetClipNameFromDegrees();
		}

		public bool CanUse() => !IsCoolingDown && Ammo.hasAmmoInReserveOrClip();


	}

	public class PrimaryGun : Gun
	{
		bool isShooting;
		public override float AttackRate => simpleShoot ? simpleShootAnimationClip.length : attacker.stats.Stats.Rate(1);
		protected override float Damage => attacker.stats.Stats.Damage(1);
		protected override Ammo Ammo => ammoInventory.primaryAmmo;
		protected override float AttackRange => attacker.stats.Stats.Range(1);
		protected override float Spread => 0;

		protected override int numberOfBulletsPerShot => 1;
	}

	public class UnlimitedGun : Gun
	{
		public override float AttackRate => GetAttackRate();

		float GetAttackRate() => attacker.stats.Stats.Rate(4);

		protected override float Damage => attacker.stats.Stats.Damage(4);
		protected override Ammo Ammo => ammoInventory.unlimitedAmmo;
		protected override float AttackRange => attacker.stats.Stats.Range(4);
		protected override float Spread => 0;
		protected override int numberOfBulletsPerShot => 1;
	}
}
