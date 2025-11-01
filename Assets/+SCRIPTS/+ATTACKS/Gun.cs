using System;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class Gun : MonoBehaviour
	{
		private static readonly string[] PrimaryAnimationClips =
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

		public AnimationClip pullOutAnimationClip;
		protected AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;
		private IAimAbility gunAimAbility => _gunAimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _gunAimAbility;

		private Body body => _body ??= GetComponent<Body>();
		private Body _body;
		protected ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>();
		private ICanAttack _attacker;
		protected IHaveAttackStats stats => _stats ??= GetComponent<IHaveAttackStats>();
		private IHaveAttackStats _stats;
		private bool isCoolingDown;
		private float currentCooldownTime;
		public string AnimationClipSuffix => this is PrimaryGun ? "" : "_Glock";
		public virtual float reloadTime => .5f;
		public abstract float AttackRate { get; }
		protected abstract float Damage { get; }
		protected abstract Ammo Ammo { get; }

		protected abstract float AttackRange { get; }
		protected abstract float Spread { get; }
		protected abstract int numberOfBulletsPerShot { get; }
		public virtual bool simpleShoot => false;
		private bool IsCoolingDown => Time.time <= currentCooldownTime;
		public bool CanReload() => Ammo.CanReload();
		public bool MustReload() => !Ammo.hasAmmoInClip();
		public void Reload() => Ammo.Reload();

		private void Awake()
		{
			currentCooldownTime = Time.time;
		}

		public bool CanShoot() => Ammo.hasAmmoInClip();

		public void Shoot(Vector2 shootDirection)
		{
			if (!CanShoot())
			{
				if (!Ammo.hasAmmoInClip()) OnNeedsReload?.Invoke();
				return;
			}

			currentCooldownTime = Time.time + AttackRate;
			OnShoot?.Invoke(shootDirection);
			Ammo.UseAmmo(1);

			for (var i = 0; i < numberOfBulletsPerShot; i++)
			{
				var randomSpread = new Vector2(UnityEngine.Random.Range(-Spread, Spread), UnityEngine.Random.Range(-Spread, Spread));
				ShootBullet(shootDirection + randomSpread);
			}
		}

		private float GetDegreesFromAimDir()
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

		private static int GetDirectionPortion(float degrees)
		{
			var normalizedDegrees = degrees % 360;
			if (normalizedDegrees < 0) normalizedDegrees += 360;
			var offsetDegrees = (normalizedDegrees + 10) % 360;
			var portionWidth = 360f / PrimaryAnimationClips.Length;
			var portionNumber = Mathf.FloorToInt(offsetDegrees / portionWidth);
			portionNumber = Mathf.Clamp(portionNumber, 0, PrimaryAnimationClips.Length - 1);
			return portionNumber;
		}

		private void ShootBullet(Vector3 shootDirection)
		{
			var hitObject = Physics2D.Linecast(body.FootPoint.transform.position, body.FootPoint.transform.position + shootDirection * AttackRange,
				attacker.EnemyLayer);
			Debug.DrawLine(body.FootPoint.transform.position, body.FootPoint.transform.position + shootDirection * AttackRange, Color.green, 3);

			if (hitObject)
			{
				var target = hitObject.collider.gameObject.GetComponentInParent<IGetAttacked>();
				if (target == null) target = hitObject.collider.gameObject.GetComponentInChildren<IGetAttacked>();
				if (target == null)
				{
					ShotMissed();
					return;
				}
				ShotHitTarget(target, hitObject.point);
			}
			else
				ShotMissed();
		}

		private void ShotMissed()
		{
			var missPosition = (Vector2) body.FootPoint.transform.position + gunAimAbility.AimDir.normalized * AttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, gunAimAbility.AimDir.normalized, AttackRange, attacker.player.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = Attack.Create(attacker, null).WithOriginPoint(body.AttackStartPoint.transform.position).WithDestinationPoint(missPosition)
			                      .WithDamage(0);

			Debug.DrawLine(body.FootPoint.transform.position, missPosition, Color.red, 3);
			OnShotMissed?.Invoke(newAttack);
		}

		private void ShotHitTarget(IGetAttacked targetLife, Vector2 hitObjectPoint)
		{
			var newAttack = Attack.Create(attacker, targetLife).WithOriginPoint(body.AttackStartPoint.transform.position).WithDestinationPoint(hitObjectPoint)
			                      .WithDamage(Damage);
			OnShotHitTarget?.Invoke(newAttack);

			targetLife.TakeDamage(newAttack);
		}
	}



	public class PrimaryGun : Gun
	{
		private bool isShooting;
		public override float AttackRate => stats.PrimaryAttackRate;
		protected override float Damage => stats.PrimaryAttackDamageWithExtra;
		protected override Ammo Ammo => ammoInventory.primaryAmmo;
		protected override float AttackRange => stats.PrimaryAttackRange;
		protected override float Spread => 0;

		protected override int numberOfBulletsPerShot => 1;
	}

	public class UnlimitedGun : Gun
	{
		public override float AttackRate => GetAttackRate();

		private float GetAttackRate()
		{
			Debug.Log("unlimited attack rate: " + stats.UnlimitedAttackRate);
			return stats.UnlimitedAttackRate;
		}

		protected override float Damage => stats.UnlimitedAttackDamageWithExtra;
		protected override Ammo Ammo => ammoInventory.unlimitedAmmo;
		protected override float AttackRange => stats.UnlimitedAttackRange;
		protected override float Spread => 0;
		protected override int numberOfBulletsPerShot => 1;
	}
}
