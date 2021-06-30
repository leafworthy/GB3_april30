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
	public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	public event Action OnKnifeStart;
	public event Action<Vector3> OnNadeAim;
	public event Action OnReloadStart;

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
	private Player player;

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
		player = stats.player;
		ammoHandler = GetComponent<AmmoHandler>();

		playerRemote = GetComponent<IPlayerController>();
		playerRemote.OnLeftTriggerPress += PlayerRemoteNadePress;
		playerRemote.OnLeftTriggerRelease += PlayerRemoteNadeRelease;

		playerRemote.OnRightTriggerPress += PlayerRemoteShootPress;
		playerRemote.OnRightTriggerRelease += PlayerRemoteShootRelease;
		playerRemote.OnAim += PlayerRemoteOnAim;

		playerRemote.OnAttackPress += PlayerKnifePress;
		playerRemote.OnAttackRelease += PlayerKnifeRelease;
		playerRemote.OnReloadPress += PlayerReload;

		jumpHandler = GetComponent<JumpHandler>();
		jumpHandler.OnJump += DisableAttacking;
		jumpHandler.OnLandingStop += EnableAttacking;

		animEvents.OnAttackHit += Anim_AttackHit;
		animEventsTop.OnAttackHit += Anim_AttackHit;

		animEvents.OnAttackStop += AttackStop;
		animEventsTop.OnAttackStop += AttackStop;

		animEvents.OnLandingStart += DisableAttacking;
		animEventsTop.OnReload += AnimationEvent_OnReload;
		animEvents.OnDashStart += DisableAttacking;
		animEvents.OnDashStop += EnableAttacking;
		animEventsTop.OnReloadStop += Anim_ReloadStop;
	}

	private void PlayerReload()
	{
		StartReloading();
	}

	private void AnimationEvent_OnReload()
	{
		ammoHandler.Reload(AmmoHandler.AmmoType.ak47);
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

	private void Anim_ReloadStop()
	{
		ammoHandler.ReloadStop();
	}

	private void PlayerRemoteShootPress(Vector3 aimDirection)
	{
		if (CantAttack())
		{
			if (!isAttacking) return;
			PlayerRemoteShootRelease();
			return;
		}

		if (!ammoHandler.HasAmmoInClip(AmmoHandler.AmmoType.ak47))
		{
			StartReloading();

			return;
		}

		ShootWithCooldown(aimDir);
		isAttacking = true;
	}

	private void StartReloading()
	{
		if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.ak47) || ammoHandler.clipIsFull(AmmoHandler.AmmoType.ak47))
			PlayerRemoteShootRelease();
		else
		{
			OnReloadStart?.Invoke();
			ammoHandler.ReloadStart();
			PlayerRemoteShootRelease();
		}
	}

	public bool CantAttack()
	{
		return !jumpHandler.isDoneLanding || ammoHandler.isReloading || isAttacking;
	}


	private void Anim_AttackHit(int attackType)
	{
		if (attackType != 3) return;
		float hitRange = 25;
		var knifeDamageMultiplier = 3f;
		var circleCast = Physics2D.OverlapCircleAll(transform.position, hitRange, ASSETS.LevelAssets.EnemyLayer).ToList();
		if (circleCast.Count <= 0) return;
		var closest = circleCast[0];
		foreach (var col in circleCast)
		{
			if (Vector2.Distance(col.gameObject.transform.position, transform.position) <
			    Vector2.Distance(closest.transform.position, transform.position))
			{
				closest = col;
			}
		}
		var hit2D = closest;

		Debug.Log("hit");
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


	public Vector3 GetAimCenter()
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
		var shotPosition = GetShotMissPosition();
		var newAttack = new Attack((Vector3) gunEndPoint.transform.position - shotPosition, GetShotMissPosition(),
			0);
		newAttack.DamageOrigin = gunEndPoint.transform.position;
		OnShootStart?.Invoke(newAttack);
	}

	private void ShotHitObject()
	{
		var newAttack = new Attack(targetHitPosition - gunEndPoint.transform.position, targetHitPosition,
			stats.GetStatValue(StatType.attackDamage), false, HITSTUN.StunLength.None);
		newAttack.DamageOrigin = gunEndPoint.transform.position;
		OnShootStart?.Invoke(newAttack);
	}

	private void ShotHitTarget(Vector3 origin, DefenceHandler target)
	{
		var heightVector = new Vector3(0, target.GetAimHeight(), 0);
		var newAttack = new Attack(origin, targetHitPosition + heightVector, stats.GetStatValue(StatType.attackDamage));
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
		OnAim?.Invoke(aimDir);
	}

	private void ShootWithCooldown(Vector3 target)
	{
		if (!(Time.time >= currentCooldownTime)) return;
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.ak47, 1);
		currentCooldownTime = Time.time + stats.GetStatValue(StatType.attackRate);
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
}
