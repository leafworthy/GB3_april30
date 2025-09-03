using System;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class Gun : MonoBehaviour
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public event Action OnNeedsReload;
		public event Action OnEmpty;
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
		protected abstract float AttackRate { get; }
		protected abstract float Damage { get; }
		protected abstract Ammo Ammo { get; }

		protected abstract float AttackRange { get; }
		protected abstract float Spread { get; }
		protected abstract int numberOfBulletsPerShot { get; }
		protected virtual bool simpleShoot => false;
		private bool IsCoolingDown => !(Time.time >= currentCooldownTime);
		public bool CanReload() => Ammo.CanReload();
		public void Reload() => Ammo.Reload();
		public bool CanShoot() => !NeedToReload() && !IsCoolingDown;

		public void Shoot(Vector2 targetPosition)
		{
			currentCooldownTime = Time.time + AttackRate;
			Debug.Log("[GUN] shoot in gun");

			if (!CanShoot()) return;
			Debug.Log("[GUN] isAttacking, number of bullets: " + numberOfBulletsPerShot, this);
			if (simpleShoot) anim.SetTrigger(UnitAnimations.ShootingTrigger);
			for (var i = 0; i < numberOfBulletsPerShot; i++)
			{
				var randomSpread = new Vector2(UnityEngine.Random.Range(-Spread, Spread), UnityEngine.Random.Range(-Spread, Spread));
				ShootBullet(targetPosition + randomSpread);
			}
		}

		private bool NeedToReload()
		{
			if (!Ammo.hasAmmoInClip())
			{
				if (Ammo.hasReserveAmmo())
				{
					Debug.Log("[GUN] needs reload in gun");
					OnNeedsReload?.Invoke();
					return true;
				}

				Debug.Log("[GUN] empty in gun");
				OnEmpty?.Invoke();
				return true;
			}

			return false;
		}

		private void ShootBullet(Vector3 targetPosition)
		{
			Debug.Log("[GUN] shoot bullet in gun", this);
			if (!Ammo.hasAmmoInClip())
			{
				OnNeedsReload?.Invoke();
				return;
			}

			Ammo.UseAmmo(1);
			var hitObject = Physics2D.Linecast(body.FootPoint.transform.position, targetPosition, life.EnemyLayer);

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
			var newAttack = new Attack(life, body.FootPoint.transform.position, missPosition, null, 0);
			MyDebugUtilities.DrawAttack(newAttack, Color.red);
			Debug.DrawLine(body.FootPoint.transform.position, missPosition, Color.red, 3);
			OnShotMissed?.Invoke(newAttack, body.AttackStartPoint.transform.position);
		}

		private void ShotHitTarget(RaycastHit2D hitObject)
		{
			Debug.Log("shot hit target", this);
			var target = hitObject.collider.gameObject.GetComponentInChildren<Life>();
			if (target == null) return;
			var newAttack = new Attack(life, body.FootPoint.transform.position, hitObject.point, target, Damage);
			OnShotHitTarget?.Invoke(newAttack, body.AttackStartPoint.transform.position);
			MyDebugUtilities.DrawAttack(newAttack, Color.green);
			Debug.DrawLine(body.FootPoint.transform.position, body.FootPoint.transform.position, Color.green, 3);
			target.TakeDamage(newAttack);
		}
	}

	public class PrimaryGun : Gun
	{
		private bool isShooting;
		protected override float AttackRate => life.PrimaryAttackRate;
		protected override float Damage => life.PrimaryAttackDamageWithExtra;
		protected override Ammo Ammo => ammoInventory.primaryAmmo;
		protected override float AttackRange => life.PrimaryAttackRange;
		protected override float Spread => 0;

		protected override int numberOfBulletsPerShot => 1;
	}

	public class UnlimitedGun : Gun
	{
		protected override float AttackRate => life.UnlimitedAttackRate;
		protected override float Damage => life.UnlimitedAttackDamageWithExtra;
		protected override Ammo Ammo => ammoInventory.unlimitedAmmo;
		protected override float AttackRange => life.UnlimitedAttackRange;
		protected override float Spread => 0;
		protected override int numberOfBulletsPerShot => 1;
	}
}
