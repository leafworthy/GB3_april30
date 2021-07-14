using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class BeanAttackHandler : MonoBehaviour, IAttackHandler, IShootHandler, IAimHandler
{
	[SerializeField] private GameObject aimCenter;
	[SerializeField] private GameObject gunEndPoint;
	[SerializeField] private GameObject footPoint;

	public event Action OnNadeThrowStart;
	public event Action OnAttackStop;
	public event Action<Vector3> OnAim;
	public event Action OnAimStop;
	public event Action<Attack> OnShootStart;

	public bool IsGlocking { get; private set; }

	public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	public bool isBusy()
	{
		return CantAttack();
	}

	public event Action OnKnifeStart;
	public event Action<Vector2> OnNadeAim;
	public event Action OnNadeAimStop;
	public event Action OnReloadStart;
	public event Action OnShootMiss;

	private Vector2 aimDir;
	private Vector2 targetHitPosition;
	private float currentCooldownTime;
	private Vector2 nadeAimDir;

	private bool isNading;
	private bool isAttacking;

	public AnimationEvents animEventsTop;
	public AnimationEvents animEvents;
	private JumpHandler jumpHandler;
	private AmmoHandler ammoHandler;
	private UnitStats stats;
	private IPlayerController playerController;
	private int knifeCoolDown = 4;
	private Player player;
	private bool isReloading;
	private float glockAttackRateMultiplier = 5;
	private float glockDamageMultiplier = 1.5f;

	private void EnableAttacking()
	{
		isAttacking = false;
	}

	private void DisableAttacking()
	{
		isAttacking = true;
	}

	private void EnableAttacking(int obj)
	{
		EnableAttacking();
	}

	private void Start()
	{
		stats = GetComponent<UnitStats>();
		player = stats.player;
		ammoHandler = GetComponent<AmmoHandler>();

		playerController = GetComponent<IPlayerController>();
		Debug.Log("player controller" + playerController);
		playerController.OnLeftTriggerPress += PlayerControllerNadePress;
		playerController.OnLeftTriggerRelease += PlayerControllerNadeRelease;

		playerController.OnRightTriggerPress += PlayerControllerShootPress;
		playerController.OnRightTriggerRelease += PlayerControllerShootRelease;
		playerController.OnAim += PlayerControllerOnAim;

		playerController.OnAttackPress += PlayerKnifePress;
		playerController.OnAttackRelease += PlayerKnifeRelease;
		playerController.OnReloadPress += PlayerReload;

		jumpHandler = GetComponent<JumpHandler>();
		jumpHandler.OnJump += DisableAttacking;
		jumpHandler.OnLandingStop += EnableAttacking;

		animEvents.OnAttackHit += Anim_AttackHit;
		animEvents.OnReset += Reset;
		animEventsTop.OnAttackHit += Anim_AttackHit;

		animEvents.OnAttackStop += AttackStop;
		animEventsTop.OnAttackStop += AttackStop;

		animEvents.OnLandingStart += DisableAttacking;
		animEventsTop.OnReload += AnimationEvent_OnReload;
		animEvents.OnDashStart += DisableAttacking;
		animEvents.OnDashStop += EnableAttacking;
		animEventsTop.OnReloadStop += Anim_ReloadStop;
	}

	private void Reset()
	{
		isAttacking = false;
		isNading = false;
		jumpHandler.isDoneLanding = true;
		isReloading = false;
	}

	private void PlayerReload()
	{
		StartReloading();
	}

	private void AnimationEvent_OnReload()
	{
		if (IsGlocking)
		{
			ammoHandler.Reload(AmmoHandler.AmmoType.glock);
		}
		else
		{
			ammoHandler.Reload(AmmoHandler.AmmoType.ak47);
		}
	}

	private void AttackStop(int obj)
	{
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


	private void PlayerControllerOnAim(Vector3 aimDirection)
	{
		AimInDirection(aimDirection);
	}

	private void PlayerControllerNadeRelease(Vector3 aimDirection)
	{
		if (!isNading) return;
		isNading = false;
		if (CantAttack()) return;
		if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.nades)) return;
		OnNadeAimStop?.Invoke();
		NadeWithCooldown(aimDirection);
	}

	private void PlayerControllerNadePress(Vector3 dir)
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

	private void PlayerControllerShootRelease()
	{
		isAttacking = false;
		OnAttackStop?.Invoke();
	}

	private void Anim_ReloadStop()
	{
		isReloading = false;
	}

	private void PlayerControllerShootPress(Vector3 aimDirection)
	{
		if (CantAttack())
		{
			if (!isAttacking) return;
			PlayerControllerShootRelease();
			return;
		}

		if (NeedsToReload())
		{
			StartReloading();

			return;
		}

		ShootWithCooldown(aimDir);
		isAttacking = true;
	}

	private bool NeedsToReload()
	{
		if (!IsGlocking) return !ammoHandler.HasAmmoInClip(AmmoHandler.AmmoType.ak47);
		if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.ak47))
			return !ammoHandler.HasAmmoInClip(AmmoHandler.AmmoType.glock);
		IsGlocking = false;
		return ammoHandler.HasAmmoInClip(AmmoHandler.AmmoType.ak47);

	}

	private void StartReloading()
	{
		if (ammoHandler.clipIsFull(AmmoHandler.AmmoType.ak47))
		{
			Debug.Log("tried to reload but clip is full");
			if (IsGlocking)
			{
				IsGlocking = false;
			}

			PlayerControllerShootRelease();
		}else if (ammoHandler.HasAmmo(AmmoHandler.AmmoType.ak47))
		{
			IsGlocking = false;
			isReloading = true;
			OnReloadStart?.Invoke();
			Debug.Log("reload start");
			PlayerControllerShootRelease();
		}
		else if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.ak47))
		{
			Debug.Log("no AK ammo");
			IsGlocking = true;

			Debug.Log("out of glock ammo");
			if (isReloading) return;
			isReloading = true;
			OnReloadStart?.Invoke();
			PlayerControllerShootRelease();

		}
	}

	public bool CantAttack()
		{
			return !jumpHandler.isDoneLanding || isReloading || isAttacking;
		}


		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 3) return;
			float hitRange = 25;
			var knifeDamageMultiplier = 3f;
			var circleCast = Physics2D.OverlapCircleAll(transform.position, hitRange, ASSETS.LevelAssets.EnemyLayer)
			                          .ToList();
			if (circleCast.Count <= 0) return;
			var closest = circleCast[0];
			foreach (var col in circleCast)
				if (Vector2.Distance(col.gameObject.transform.position, transform.position) <
				    Vector2.Distance(closest.transform.position, transform.position))
					closest = col;
			var hit2D = closest;

			var enemy = hit2D.transform.gameObject.GetComponent<DefenceHandler>();
			if (enemy == null) return;
			if (enemy.IsPlayer()) return;
			var damage = stats.GetStatValue(StatType.attackDamage);
			var newAttack = new Attack(transform.position, enemy.transform.position,
				stats.GetStatValue(StatType.attackDamage) * knifeDamageMultiplier);
			var didItKill = enemy.TakeDamage(newAttack);


			if (!didItKill) return;
			ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.meleeCooldown, 999);
			OnKillEnemy?.Invoke();
		}


		public Vector2 GetAimCenter()
		{
			return aimCenter.transform.position;
		}


		//MAJOR
		private void ShootTarget(Vector3 targetPosition)
		{
			AimInDirection(targetPosition);
			var hitObject = CheckRaycastHit(targetPosition);
			if (hitObject != null)
			{
				var target = hitObject.GetComponent<DefenceHandler>();
				if (target != null)
					ShotHitTarget(GetAimCenter(), target);
				else
					ShotHitObject();
			}
			else
				ShotMissed();
		}

		private void ShotMissed()
		{
			var newAttack = new Attack( gunEndPoint.transform.position, GetShotMissPosition(),
				0);
			OnShootStart?.Invoke(newAttack);
			OnShootMiss?.Invoke();
		}

		private void ShotHitObject()
		{
			var newAttack = new Attack(gunEndPoint.transform.position,
				targetHitPosition,
				stats.GetStatValue(StatType.attackDamage), false, HITSTUN.StunLength.None);

			OnShootStart?.Invoke(newAttack);
			OnShootMiss?.Invoke();
		}

		private void ShotHitTarget(Vector3 origin, DefenceHandler target)
		{
			var heightVector = new Vector3(0, target.GetAimHeight(), 0);
			var damage = IsGlocking
				? stats.GetStatValue(StatType.attackDamage) * glockDamageMultiplier
				: stats.GetStatValue(
					StatType.attackDamage);
			var newAttack = new Attack(origin, targetHitPosition + (Vector2) heightVector,
				damage);
			OnShootStart?.Invoke(newAttack);
			var itKilled = target.TakeDamage(newAttack);
			if (itKilled) OnKillEnemy?.Invoke();
		}

		private Vector3 GetShotMissPosition()
		{
			return GetAimCenter() + aimDir * stats.GetStatValue(StatType.attackRange);
		}

		private GameObject CheckRaycastHit(Vector3 targetDirection)
		{
			var raycastHit = Physics2D.Raycast(footPoint.transform.position,
				targetDirection.normalized,
				stats.GetStatValue(StatType.attackRange),
				ASSETS.LevelAssets.EnemyLayer);

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
			Debug.Log("aiming in direction" + aimDir);
			OnAim?.Invoke(aimDir);
		}

		private void ShootWithCooldown(Vector3 target)
		{
			if (!IsGlocking)
			{
				if (!(Time.time >= currentCooldownTime)) return;
				OnUseAmmo?.Invoke(AmmoHandler.AmmoType.ak47, 1);
				currentCooldownTime = Time.time + stats.GetStatValue(StatType.attackRate);
				ShootTarget(target);
			}
			else
			{
				if (!(Time.time >= currentCooldownTime)) return;
				OnUseAmmo?.Invoke(AmmoHandler.AmmoType.glock, 1);
				currentCooldownTime = Time.time + stats.GetStatValue(StatType.attackRate) * glockAttackRateMultiplier;
				ShootTarget(target);
			}
		}


		private void AimNade(Vector3 dir)
		{
			OnNadeAim?.Invoke(dir);
		}

		private void FixedUpdate()
		{
			ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.meleeCooldown, knifeCoolDown);
			if (isNading) AimNade(nadeAimDir);
			if (isNading && player.isUsingKeyboard) AimNadeAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}

		private void AimNadeAt(Vector2 target)
		{
			OnNadeAimAt?.Invoke(target);
		}

		private void NadeWithCooldown(Vector3 target)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			OnUseAmmo?.Invoke(AmmoHandler.AmmoType.nades, 1);
			OnNadeThrowStart?.Invoke();
			currentCooldownTime = Time.time + stats.GetStatValue(StatType.attackRate);
		}

		public Player GetPlayer()
		{
			return player;
		}

		public bool CanAttack(Vector3 target)
		{
			var targetDistance = Vector3.Distance(GetAimCenter(), target);
			if (targetDistance < stats.GetStatValue(StatType.attackRate)) return true;

			return false;
		}

	public event Action OnKillEnemy;

	public void StopAiming()
	{
		OnAimStop?.Invoke();
	}

	public event Action<Vector2> OnNadeAimAt;
}
