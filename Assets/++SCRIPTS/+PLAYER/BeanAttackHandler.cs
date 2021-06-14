using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace _SCRIPTS
{
	[Serializable]
	public class BeanAttackHandler : MonoBehaviour, IAttackHandler
	{
		[SerializeField] private GameObject aimCenter;
		[SerializeField] private GameObject gunEndPoint;
		[SerializeField] private GameObject footPoint;

		public event Action OnNadeThrowStart;
		public event Action OnAttackStop;
		public event Action<Vector3> OnAim;
		public event Action OnAimStop;
		public event Action<OnAttackEventArgs> OnAttackStart;
		public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
		public event Action OnKnifeStart;
		public event Action<Vector3> OnNadeAim;

		private Vector3 aimDir;
		private Vector3 targetHitPosition;
		private float currentCooldownTime;
		private Vector3 nadeAimDir;

		private bool isNading;
		private bool isAttacking;

		public AnimationEvents animEventsTop;
		public AnimationEvents animEvents;
		private JumpHandler jumpHandler;
		private AmmoHandler ammoHandler;
		private UnitStats stats;
		private IPlayerController playerRemote;
		private int knifeCoolDown = 4;

		private void EnableAttacking()
		{
			Debug.Log("attacking enabled");
			isAttacking = false;
		}

		private void DisableAttacking()
		{
			Debug.Log("attacking disabled");
			isAttacking = true;
		}

		private void EnableAttacking(int obj)
		{
			EnableAttacking();
		}

		private void Awake()
		{
			stats = GetComponent<UnitStats>();
			ammoHandler = GetComponent<AmmoHandler>();

			playerRemote = GetComponent<IPlayerController>();
			playerRemote.OnLeftTriggerPress += PlayerRemoteNadePress;
			playerRemote.OnLeftTriggerRelease += PlayerRemoteNadeRelease;

			playerRemote.OnRightTriggerPress += PlayerRemoteShootPress;
			playerRemote.OnRightTriggerRelease += PlayerRemoteShootRelease;
			playerRemote.OnAim += PlayerRemoteOnAim;

			playerRemote.OnAttackPress += PlayerKnifePress;
			playerRemote.OnAttackRelease += PlayerKnifeRelease;

			jumpHandler = GetComponent<JumpHandler>();
			jumpHandler.OnJump += DisableAttacking;
			jumpHandler.OnLandingStop += EnableAttacking;

			animEvents.OnAttackHit += Anim_AttackHit;
			animEventsTop.OnAttackHit += Anim_AttackHit;

			animEvents.OnAttackStop += AttackStop;
			animEventsTop.OnAttackStop += AttackStop;

			animEvents.OnLandingStart += DisableAttacking;
			animEvents.OnDashStart += DisableAttacking;
			animEvents.OnDashStop += EnableAttacking;
		}

		private void AttackStop(int obj)
		{
			Debug.Log("attack stop");
			EnableAttacking();
		}


		private void PlayerKnifeRelease()
		{
			OnAttackStop?.Invoke();

		}

		private void PlayerKnifePress(Vector3 obj)
		{
			if (CantAttack()) return;
			if (!ammoHandler.HasFullAmmo(AmmoHandler.AmmoType.meleeCooldown)) return;
			OnUseAmmo?.Invoke(AmmoHandler.AmmoType.meleeCooldown, 999);
			OnKnifeStart?.Invoke();
		}



		private void PlayerRemoteOnAim(Vector3 aimDirection)
		{
			AimInDirection(aimDirection);
		}

		private void PlayerRemoteNadeRelease(Vector3 aimDirection)
		{
			if (!isNading) return;
			isNading = false;
			if (CantAttack()) return;
			if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.nades)) return;

			NadeWithCooldown(aimDirection);
		}

		private void PlayerRemoteNadePress(Vector3 dir)
		{
			if (CantAttack()) return;
			if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.nades))
			{
				Debug.Log("NONADES");
				return;
			}

			isNading = true;
			nadeAimDir = dir;
		}

		private void PlayerRemoteShootRelease()
		{
			isAttacking = false;
			OnAttackStop?.Invoke();
		}

		private void PlayerRemoteShootPress(Vector3 aimDirection)
		{
			if (jumpHandler.isJumping || ammoHandler.isReloading) return;
			if (!ammoHandler.HasAmmoInClip(AmmoHandler.AmmoType.ak47))
			{
				ammoHandler.Reload(AmmoHandler.AmmoType.ak47);
				PlayerRemoteShootRelease();
				return;
			}

			ShootWithCooldown(aimDir);
			isAttacking = true;
		}

		public bool CantAttack()
		{
			return  jumpHandler.isJumping || ammoHandler.isReloading || isAttacking;
		}


		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 3) return;
			float hitRange = 25;
			float knifeDamageMultiplier = 3;
			var circleCast = Physics2D.OverlapCircleAll(transform.position, hitRange);

			foreach (var hit2D in circleCast)
			{
				Debug.Log("hit");
				var enemy = hit2D.transform.gameObject.GetComponent<DefenceHandler>();
				if (enemy == null) continue;
				if (enemy.IsPlayer()) continue;
				var attackDirection = hit2D.transform.position - transform.position;

				var didItKill = enemy.TakeDamage(attackDirection,
					stats.attackDamage * knifeDamageMultiplier,
					enemy.transform.position);


				if (!didItKill) continue;
				ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.meleeCooldown, 999);
				OnKillEnemy?.Invoke();
			}
		}


		public Vector3 GetAimCenter()
		{
			return aimCenter.transform.position;
		}


		//MAJOR
		private void ShootTarget(Vector3 shootDirection)
		{
			AimInDirection(shootDirection);
			var hitObject = CheckRaycastHit(shootDirection);
			if (hitObject != null)
			{
				var target = hitObject.GetComponent<DefenceHandler>();
				if (target != null)
					ShotHitTarget(shootDirection, target);
				else
					ShotHitObject();
			}
			else
				ShotMissed();
		}
		private void ShotMissed()
		{
			var e = new OnAttackEventArgs(gunEndPoint.transform.position, GetShotMissPosition());
			OnAttackStart?.Invoke(e);
		}
		private void ShotHitObject()
		{
			var e = new OnAttackEventArgs(gunEndPoint.transform.position,
				targetHitPosition);
			OnAttackStart?.Invoke(e);
		}
		private void ShotHitTarget(Vector3 shootDirection, DefenceHandler target)
		{
			var heightVector = new Vector3(0, target.GetAimHeight(), 0);
			var e = new OnAttackEventArgs(gunEndPoint.transform.position, targetHitPosition + heightVector, target);
			OnAttackStart?.Invoke(e);
			var itKilled = target.TakeDamage(shootDirection, stats.attackDamage, e.AttackEndPosition);
			if (itKilled) OnKillEnemy?.Invoke();
		}
		private Vector3 GetShotMissPosition()
		{
			return GetAimCenter() + aimDir * stats.attackRange;
		}
		private GameObject CheckRaycastHit(Vector3 targetDirection)
		{
			var raycastHit = Physics2D.Raycast(footPoint.transform.position,
				targetDirection.normalized,
				stats.attackRange,
				ASSETS.layers.EnemyLayer);

			if (raycastHit.collider != null)
			{
				targetHitPosition = raycastHit.point;
				return raycastHit.collider.gameObject;
			}

			return null;
		}
		private void AimInDirection(Vector3 newDirection)
		{
			aimDir = newDirection.normalized;
			OnAim?.Invoke(aimDir);
		}
		private void ShootWithCooldown(Vector3 target)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			OnUseAmmo?.Invoke(AmmoHandler.AmmoType.ak47, 1);
			currentCooldownTime = Time.time + stats.attackRate;
			ShootTarget(target);
		}


		private void AimNade(Vector3 dir)
		{
			OnNadeAim?.Invoke(dir);
		}
		private void FixedUpdate()
		{
			ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.meleeCooldown, knifeCoolDown);
			if (isNading) AimNade(nadeAimDir);
		}
		private void NadeWithCooldown(Vector3 target)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			OnUseAmmo?.Invoke(AmmoHandler.AmmoType.nades, 1);
			OnNadeThrowStart?.Invoke();
			currentCooldownTime = Time.time + stats.attackRate;
		}

		public bool CanAttack(Vector3 target)
		{
			var targetDistance = Vector3.Distance(GetAimCenter(), target);
			if (targetDistance < stats.attackRange) return true;

			return false;
		}

		public event Action OnKillEnemy;

		public void StopAiming()
		{
			OnAimStop?.Invoke();
		}
	}
}
