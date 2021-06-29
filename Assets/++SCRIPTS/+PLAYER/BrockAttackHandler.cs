using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrockAttackHandler : MonoBehaviour, IAttackHandler, IAimHandler
{
	public event Action OnThrow;
	public event Action OnAirThrow;
	public event Action OnAttack1;
	public event Action OnAttack2;
	public event Action OnAttack3;
	public event Action OnChargeStart;
	public event Action<bool> OnChargeStop;
	public event Action OnJumpAttack;

	private AnimationEvents animEvents;
	private JumpHandler jumpHandler;

	private readonly float attack1Cooldown = .5f;
	private readonly float attack2Cooldown = .5f;
	private readonly float attack3Cooldown = .5f;
	private readonly float chargingCooldown = .5f;
	private readonly float jumpAttackCooldown = .5f;

	private float coolDown;

	private bool isAttacking;
	private bool isCharging;
	private bool isCoolingDown;
	private bool isLanding;

	private IPlayerController playerRemote;
	private float hitStunDuration = 1;
	private AmmoHandler ammoHandler;
	private int batNormalCooldown = 10;
	private int attackHitSpecialAmount = 10;
	private int attackKillSpecialAmount = 30;
	private UnitStats stats;
	private bool isGoingToAttackAfterAttack;


	private void Awake()
	{
		stats = GetComponent<UnitStats>();
		jumpHandler = GetComponent<JumpHandler>();
		ammoHandler = GetComponent<AmmoHandler>();

		playerRemote = GetComponent<IPlayerController>();
		playerRemote.OnAttackPress += PlayerRemoteAttackPress;
		playerRemote.OnChargePress += PlayerRemoteChargePress;
		playerRemote.OnChargeRelease += PlayerRemoteChargeRelease;
		playerRemote.OnRightTriggerPress += PlayerRemoteRightTriggerPress;
		playerRemote.OnAim += PlayerRemoteOnAim;
		playerRemote.OnAimStickInactive += PlayerAimStop;

		animEvents = GetComponentInChildren<AnimationEvents>();
		animEvents.OnAttackStart += Anim_AttackStart;
		animEvents.OnAttackHit += Anim_AttackHit;
		animEvents.OnAttackStop += Anim_AttackStop;
		animEvents.OnLandingStart += Anim_LandingStart;
		animEvents.OnLandingStop += Anim_LandingStop;

		animEvents.OnThrowStart += ThrowStart;
		animEvents.OnThrowStop += ThrowStop;
		animEvents.OnThrow += Throw;
		animEvents.OnAirThrow += Airthrow;
	}

	private void PlayerAimStop()
	{
		OnAimStop?.Invoke();
	}

	private void PlayerRemoteOnAim(Vector3 newDirection)
	{
		var aimDir = newDirection.normalized;
		OnAim?.Invoke(aimDir);
	}

	private void Throw()
	{
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.kunai, 1);
	}

	private void Airthrow()
	{
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.kunai, 3);
	}

	private void Anim_AttackStart(int obj)
	{
		isAttacking = true;
	}

	private void StartStandingKunaiAttack()
	{
		Debug.Log("standing kunai");
		OnThrow?.Invoke();
		coolDown = attack1Cooldown;
	}

	private void StartAirKunaiAttack()
	{
		Debug.Log("air kunai");
		OnAirThrow?.Invoke();
		coolDown = attack1Cooldown;
	}

	private void PlayerRemoteRightTriggerPress(Vector3 aimDirection)
	{
		if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.kunai)) return;
		if (!CanAttack()) return;

		isAttacking = true;
		if (jumpHandler.isJumping)
			StartAirKunaiAttack();
		else
			StartStandingKunaiAttack();
	}



	private void FixedUpdate()
	{
		if (!ammoHandler.HasFullAmmo(AmmoHandler.AmmoType.meleeCooldown))
			ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.meleeCooldown, batNormalCooldown);
	}


	private void AirThrow()
	{
		OnAirThrow?.Invoke();
	}


	private void AirThrowStop()
	{
		isAttacking = false;
	}

	private void AirThrowStart()
	{
		isAttacking = true;
	}


	private void ThrowStop()
	{
		isAttacking = false;
	}

	private void ThrowStart()
	{
		isAttacking = true;
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
		if (isGoingToAttackAfterAttack)
		{
			PlayerRemoteAttackPress(Vector3.zero);
		}
	}

	private void Anim_AttackHit(int attackType)
	{
		var circleCast = Physics2D.OverlapCircleAll(transform.position, GetHitRange(attackType));

		foreach (var hit2D in circleCast)
		{
			var enemy = hit2D.transform.gameObject.GetComponent<DefenceHandler>();
			if (enemy != null)
			{
				if (!enemy.IsPlayer())
				{
					var newAttack = new Attack(transform.position, hit2D.transform.position,
						GetAttackDamage(attackType));
					var didItKill = enemy.TakeDamage(newAttack);

					ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.specialCooldown, attackHitSpecialAmount);
					if (didItKill)
					{
						ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.specialCooldown, attackKillSpecialAmount);
						OnKillEnemy?.Invoke();
					}
				}
			}
		}
	}

	private float GetHitRange(int attackType)
	{
		if (attackType == 5)
		{
			return stats.GetStatValue(StatType.attackRange) *2;
		}
		else
		{
			return stats.GetStatValue(StatType.attackRange);
		}
	}

	private float GetAttackDamage(int attackType)
	{
		switch (attackType)
		{
			case 1:
				return 25;
			case 2:
				return 25;
			case 3:
				return 40;
			case 4:
				return 50;
			case 5:
				return 200;
			default:
				Debug.Log("damage type 0");
				return 0;
		}
	}

	private void HandleCooldown()
	{
		if (coolDown > 0)
		{
			isCoolingDown = true;
			coolDown -= Time.deltaTime;
		}
		else
		{
			isCoolingDown = false;
			coolDown = 0;
		}
	}

	private void PlayerRemoteAttackPress(Vector3 obj)
	{
		if (CanAttack())
		{
			if (!ammoHandler.HasFullAmmo(AmmoHandler.AmmoType.meleeCooldown)) return;
			Debug.Log("ATTACK");
			isGoingToAttackAfterAttack = false;
			if (jumpHandler.isJumping)
				StartJumpAttack();
			else
				StartRandomAttack();
		}
		else
		{
			Debug.Log("attack queued");
			isGoingToAttackAfterAttack = true;
		}
	}

	private void StartRandomAttack()
	{
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.meleeCooldown, 1000);
		var randomAttack = Random.Range(1, 3);
		Debug.Log("random attack");
		switch (randomAttack)
		{
			case 1:
				Attack1();
				break;
			case 2:
				Attack2();
				break;
		}
	}

	private bool CanAttack()
	{
		return !isAttacking && !isCharging && !isLanding;
	}

	private void StartJumpAttack()
	{
		OnJumpAttack?.Invoke();
		coolDown = jumpAttackCooldown;
	}

	private void Attack1()
	{
		Debug.Log("attack1");
		OnAttack1?.Invoke();
		coolDown = attack1Cooldown;
	}

	private void Attack2()
	{
		OnAttack2?.Invoke();
		Debug.Log("attack2");
		coolDown = attack2Cooldown;
	}

	private void Attack3()
	{
		OnAttack3?.Invoke();
		Debug.Log("attack3");
		coolDown = attack3Cooldown;
	}

	private void PlayerRemoteChargePress()
	{
		if (!ammoHandler.HasFullAmmo(AmmoHandler.AmmoType.specialCooldown))
		{
			return;
		}

		if (!CanAttack() || !jumpHandler.isDoneLanding) return;
		if (!jumpHandler.isDoneLanding) return;

		isCharging = true;
		coolDown = chargingCooldown;
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.specialCooldown, 100);
		OnChargeStart?.Invoke();
	}

	private void PlayerRemoteChargeRelease()
	{
		if (isCharging)
		{
			isCharging = false;
			OnChargeStop?.Invoke(!isCoolingDown);
			if (isCoolingDown) animEvents.AttackStop();
		}
	}

	public bool CanAttack(Vector3 getPosition)
	{
		throw new NotImplementedException();
	}

	public void Disable()
	{
		enabled = false;
	}

	public event Action OnKillEnemy;
	public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	public event Action<Vector3> OnAim;
	public event Action OnAimStop;
}
