using System;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class Gun : MonoBehaviour
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public event Action OnNeedReload;
		public event Action OnEmpty;
		private UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		private UnitAnimations _anim;
		protected AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;
		private ASSETS assetManager => _assetManager ??= ServiceLocator.Get<ASSETS>();
		private ASSETS _assetManager;
		private IAimAbility gunAimAbility => _gunAimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _gunAimAbility;

		private Body body => _body ??= GetComponent<Body>();
		private Body _body;
		protected Life life => _life ??= GetComponent<Life>();
		private Life _life;
		public abstract float AttackRate { get; }
		public abstract float Damage { get; }
		public abstract Ammo Ammo { get; }

		public abstract float AttackRange { get; }
		public abstract float Spread { get; }
		public abstract int numberOfBulletsPerShot { get; }
		public virtual bool simpleShoot => false;

		public bool CanShoot()
		{

			if (!Ammo.hasAmmoInClip())
			{
				Debug.Log("[GUN] no ammo in clip in gun");
				return false;
			}
			return true;
		}

		public void Shoot(Vector2 targetPosition)
		{
			Debug.Log("[GUN] shoot in gun");
			if (!Ammo.hasAmmoInClip())
			{
				if (Ammo.hasReserveAmmo())
				{
					Debug.Log("[GUN] needs reload in gun");
					OnNeedReload?.Invoke();
					return;
				}

				Debug.Log("[GUN] empty in gun");
				OnEmpty?.Invoke();
				return;
			}

			if (!CanShoot()) return;
			Debug.Log("[GUN] isAttacking");
			if (simpleShoot) anim.SetTrigger(UnitAnimations.ShootingTrigger);
			for (var i = 0; i < numberOfBulletsPerShot; i++)
			{
				var randomSpread = new Vector2(UnityEngine.Random.Range(-Spread, Spread), UnityEngine.Random.Range(-Spread, Spread));
				ShootBullet(targetPosition + randomSpread);
			}
		}

		private void ShootBullet(Vector3 targetPosition)
		{
			if (!Ammo.hasAmmoInClip()) return;
			Ammo.UseAmmo(1);
			var hitObject = gunAimAbility.CheckRaycastHit(targetPosition, assetManager.LevelAssets.EnemyLayer);
			if (hitObject)
				ShotHitTarget(hitObject);
			else
				ShotMissed();
		}

		private void ShotMissed()
		{
			var missPosition = (Vector2) body.FootPoint.transform.position + gunAimAbility.AimDir.normalized * AttackRange;
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, gunAimAbility.AimDir.normalized, AttackRange,
				assetManager.LevelAssets.BuildingLayer);
			if (raycastHit) missPosition = raycastHit.point;
			var newAttack = new Attack(life, body.FootPoint.transform.position, missPosition, null, 0);

			OnShotMissed?.Invoke(newAttack, body.AttackStartPoint.transform.position);
		}

		private void ShotHitTarget(RaycastHit2D hitObject)
		{
			var target = hitObject.collider.gameObject.GetComponentInChildren<Life>();
			if (target == null) return;
			var newAttack = new Attack(life, body.FootPoint.transform.position, hitObject.point, target, Damage);
			OnShotHitTarget?.Invoke(newAttack, body.AttackStartPoint.transform.position);
			target.TakeDamage(newAttack);
		}
	}

	public class PrimaryGun : Gun
	{
		private bool isShooting;
		private bool simpleShoot1;
		public override float AttackRate => life.PrimaryAttackRate;
		public override float Damage => life.PrimaryAttackDamageWithExtra;
		public override Ammo Ammo => ammoInventory.primaryAmmo;
		public override float AttackRange => life.PrimaryAttackRange;
		public override float Spread => 0;

		public override int numberOfBulletsPerShot => 1;
	}

	public class UnlimitedGun : Gun
	{
		public override float AttackRate => life.UnlimitedAttackRate;
		public override float Damage => life.UnlimitedAttackDamageWithExtra;
		public override Ammo Ammo => ammoInventory.unlimitedAmmo;
		public override float AttackRange => life.UnlimitedAttackRange;
		public override float Spread => 0;
		public override int numberOfBulletsPerShot => 1;
	}
}
