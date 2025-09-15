using System;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class Gun : MonoBehaviour
	{
		public event Action<Attack> OnShotHitTarget;
		public event Action<Attack> OnShotMissed;
		public event Action OnNeedsReload;
		public event Action OnEmpty;
		public event Action<Vector2> OnShoot;
		private UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		private UnitAnimations _anim;
		protected AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;
		private IAimAbility gunAimAbility => _gunAimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _gunAimAbility;

		private Body body => _body ??= GetComponent<Body>();
		private Body _body;
		protected Life life => _life ??= GetComponent<Life>();
		private Life _life;
		private bool isCoolingDown;
		private float currentCooldownTime;
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
		public void Reload() => Ammo.Reload();

		private void Awake()
		{
			currentCooldownTime = Time.time;
		}

		public bool CanShoot()
		{
			if (!Ammo.hasAmmoInClip())
			{
				OnNeedsReload?.Invoke();
				Debug.Log("gun needs reload", this);
				return false;
			}
			return true;
			Debug.Log("is cooling down: " + IsCoolingDown + ", has ammo in clip: " + Ammo.hasAmmoInClip(), this);
			Debug.Log("cooldown: " + currentCooldownTime + " vs time: " + Time.time +" good?: "+ (currentCooldownTime >= Time.time) , this);
			if(Ammo.hasAmmoInClip() && !IsCoolingDown) Debug.Log("wtf this is right");
			return Ammo.hasAmmoInClip() && !IsCoolingDown;
		}

		public void Shoot(Vector2 shootDirection)
		{

			if (!CanShoot())
			{
				if (!Ammo.hasAmmoInClip())
				{
					OnNeedsReload?.Invoke();
					Debug.Log("gun needs reload", this);
					return;
				}
				Debug.Log("can't shoot", this);
				return;
			}
			currentCooldownTime = Time.time + AttackRate;
			Debug.Log("[GUN] shoot in gun");


			Debug.Log("[GUN] isAttacking, number of bullets: " + numberOfBulletsPerShot, this);
			OnShoot?.Invoke(shootDirection);
			Ammo.UseAmmo(1);

			if (simpleShoot) anim.SetTrigger(UnitAnimations.ShootingTrigger);
			for (var i = 0; i < numberOfBulletsPerShot; i++)
			{
				var randomSpread = new Vector2(UnityEngine.Random.Range(-Spread, Spread), UnityEngine.Random.Range(-Spread, Spread));
				ShootBullet(shootDirection + randomSpread);
			}
		}


		private void ShootBullet(Vector3 shootDirection)
		{
			Debug.Log("[GUN] shoot bullet in gun", this);




			var hitObject = Physics2D.Linecast(body.FootPoint.transform.position, body.FootPoint.transform.position + shootDirection * AttackRange, life.EnemyLayer);
			Debug.DrawLine( body.FootPoint.transform.position, body.FootPoint.transform.position + shootDirection * AttackRange, Color.green, 3);

			if (hitObject)
				ShotHitTarget(hitObject);
			else
				ShotMissed();
		}

		private void ShotMissed()
		{
			Debug.Log("shot missed target", this);
			var missPosition = (Vector2) body.FootPoint.transform.position + gunAimAbility.AimDir.normalized * AttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, gunAimAbility.AimDir.normalized, AttackRange, life.Player.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = new Attack(life, body.AttackStartPoint.transform.position, missPosition, 0);

			Debug.DrawLine(body.FootPoint.transform.position, missPosition, Color.red, 3);
			OnShotMissed?.Invoke(newAttack);
		}

		private void ShotHitTarget(RaycastHit2D hitObject)
		{
			Debug.Log("shot hit target", this);
			var target = hitObject.collider.gameObject.GetComponentInParent<Life>();
			if (target == null)
			{
				Debug.Log("target is null", this);
				target = hitObject.collider.gameObject.GetComponentInChildren<Life>();
				if (target == null) Debug.Log("target is still null", this);
				return;
			}
			var newAttack = new Attack(life, target, body.AttackStartPoint.transform.position, hitObject.point, Damage);
			OnShotHitTarget?.Invoke(newAttack);
			target.TakeDamage(newAttack);
		}
	}

	public class PrimaryGun : Gun
	{
		private bool isShooting;
		public override float AttackRate => life.PrimaryAttackRate;
		protected override float Damage => life.PrimaryAttackDamageWithExtra;
		protected override Ammo Ammo => ammoInventory.primaryAmmo;
		protected override float AttackRange => life.PrimaryAttackRange;
		protected override float Spread => 0;

		protected override int numberOfBulletsPerShot => 1;
	}

	public class UnlimitedGun : Gun
	{
		public override float AttackRate => life.UnlimitedAttackRate;
		protected override float Damage => life.UnlimitedAttackDamageWithExtra;
		protected override Ammo Ammo => ammoInventory.unlimitedAmmo;
		protected override float AttackRange => life.UnlimitedAttackRange;
		protected override float Spread => 0;
		protected override int numberOfBulletsPerShot => 1;
	}
}
