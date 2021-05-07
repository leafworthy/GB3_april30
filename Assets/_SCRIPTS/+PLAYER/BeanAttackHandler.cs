using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace _SCRIPTS
{
	[Serializable]
	public class BeanAttackHandler : MonoBehaviour
	{
		[SerializeField] private GameObject aimCenter;
		[SerializeField] private GameObject gunEndPoint;
		[SerializeField] private GameObject footPoint;

		public event Action OnNadeThrowStart;

		private Vector3 aimDir;
		private AnimationEvents animEvents;

		private float currentCooldownTime;

		private bool isAttacking;
		private bool isCharging;
		private bool isDashing;
		private bool isLanding;
		private bool isReloading;
		private bool isNading;
		private JumpHandler jumpHandler;
		private PlayerController player;
		private UnitStats stats;
		private Vector3 targetHitPosition;

		public int totalAmmo;
		public int clipSize;
		public int bulletsInClip;
		public int nades = 5;
		public int reloadTime = 2;
		private Vector3 nadeAimDir;
		public event Action<Vector3> OnNadeAim;


		private void Awake()
		{
			stats = GetComponent<UnitStats>();

			player = GetComponent<PlayerController>();
			player.OnLeftTriggerPress += PlayerNadePress;
			player.OnLeftTriggerRelease += PlayerNadeRelease;
			player.OnRightTriggerPress += PlayerAttackPress;
			player.OnRightTriggerRelease += PlayerAttackRelease;
			player.OnAimStickActive += Player_OnAim;

			jumpHandler = GetComponent<JumpHandler>();

			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnAttackHit += Anim_AttackHit;
			animEvents.OnAttackStop += Anim_AttackStop;
			animEvents.OnLandingStart += Anim_LandingStart;
			animEvents.OnLandingStop += Anim_LandingStop;
			animEvents.OnDashStart += Anim_DashStart;
			animEvents.OnDashStop += Anim_DashStop;
			nades = 15;
		}

		private void Reload()
		{
			if (totalAmmo <= 0)
			{
				Debug.Log("Out of ammo");
				return;
			}
			if (!isReloading)
			{
				OnAttackStop?.Invoke();
				isReloading = true;
				Debug.Log("reload start");
				Invoke(nameof(ReloadComplete), reloadTime);
			}
		}

		private void FixedUpdate()
		{
			if (isNading)
			{
				AimNade(nadeAimDir);
			}
		}

		private void ReloadComplete()
		{
			Debug.Log("reload complete");
			isReloading = false;
			if (totalAmmo > clipSize - bulletsInClip)
			{
				totalAmmo -= clipSize - bulletsInClip;
				bulletsInClip = clipSize;
			}
			else
			{
				bulletsInClip = totalAmmo;
				totalAmmo = 0;
			}
		}

		public event Action OnAttackStop;
		public event Action<Vector3> OnAim;
		public event Action OnAimStop;
		public event Action<OnAttackEventArgs> OnAttackStart;

		private void Anim_DashStart()
		{
			isDashing = true;
		}

		private void Anim_DashStop()
		{
			isDashing = false;
		}

		private void Player_OnAim(Vector3 aimDirection)
		{
			AimInDirection(aimDirection);
		}

		private void PlayerNadeRelease(Vector3 aimDirection)
		{
			if (!isNading) return;
			Debug.Log("NADEPRESS");
			isNading = false;
			if (CantAttack()) return;
			if (NoNades())
			{
				Debug.Log("NONADES");
				return;
			}


			NadeWithCooldown(aimDirection);
		}

		private void PlayerNadePress(Vector3 dir)
		{
			if (NoNades())
			{
				Debug.Log("NONADES");
				return;
			}
			isNading = true;

		 nadeAimDir = dir;

		}
		private void PlayerAttackRelease()
		{
			isAttacking = false;
			OnAttackStop?.Invoke();
		}

		private void PlayerAttackPress(Vector3 aimDirection)
		{
			if (CantAttack()) return;
			if (NoAmmo())
			{
				Reload();
				return;
			}
			AttackWithCooldown(aimDir);
			isAttacking = true;
		}

		public bool CantAttack()
		{
			return isLanding || jumpHandler.isJumping || isDashing || isReloading || isNading;
		}

		private bool NoAmmo()
		{
			bool istrue = bulletsInClip <= 0;
			Debug.Log(istrue +"NOAMMO");
			return istrue;
		}

		private bool NoNades()
		{
			bool istrue = nades <= 0;
			Debug.Log(istrue + "NOAMMO");
			return istrue;
		}

		private void Anim_LandingStop()
		{
			isLanding = false;
		}

		private void Anim_LandingStart()
		{
			isLanding = true;
		}

		private void Anim_AttackStop(int obj)
		{
			isAttacking = false;
		}

		private void Anim_AttackHit(int attackType)
		{
			//
		}

		public Vector3 GetAimDir()
		{
			return aimDir;
		}

		public bool IsPlayer()
		{
			return stats.isPlayer;
		}

		public float GetAttackRange()
		{
			return stats.attackRange;
		}

		public Vector3 GetAimCenter()
		{
			return aimCenter.transform.position;
		}

		private void ShootTarget(Vector3 shootDirection)
		{
			AimInDirection(shootDirection);
			var hitObject = CheckRaycastHit(shootDirection);
			if (hitObject != null)
			{
				var target = hitObject.GetComponent<DefenceHandler>();
				if (target != null)
				{
					ShotHitTarget(shootDirection, target);
				}
				else
				{
					ShotHitObject();
				}
			}
			else
			{
				ShotMissed();
			}
		}

		private void ShotMissed()
		{
			var e = new OnAttackEventArgs(gunEndPoint.transform.position, GetMissPosition());
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
			target.TakeDamage(shootDirection, stats.attackDamage, e.AttackEndPosition);
		}

		private Vector3 GetMissPosition()
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

		public void AimInDirection(Vector3 newDirection)
		{
			aimDir = newDirection.normalized;
			OnAim?.Invoke(aimDir);
		}

		public void AttackWithCooldown(Vector3 target)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			bulletsInClip--;
			AmmoDisplay.UpdateDisplay(this);
			currentCooldownTime = Time.time + stats.attackRate;
			ShootTarget(target);
		}

		public void AimNade(Vector3 dir)
		{
			OnNadeAim?.Invoke(dir);
		}

		public void NadeWithCooldown(Vector3 target)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			nades--;
			AmmoDisplay.UpdateDisplay(this);
			OnNadeThrowStart?.Invoke();
			currentCooldownTime = Time.time + stats.attackRate;
		}

		public bool CanAttack(Vector3 target)
		{
			var targetDistance = Vector3.Distance(GetAimCenter(), target);
			if (targetDistance < stats.attackRange) return true;

			return false;
		}

		public void StopAiming()
		{
			OnAimStop?.Invoke();
		}
	}
}
