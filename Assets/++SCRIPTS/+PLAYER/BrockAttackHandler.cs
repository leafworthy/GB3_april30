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
	public event Action<int> OnHitConnected;

	private AnimationEvents animEvents;
	private JumpHandler jumpHandler;

	private readonly float attack1Cooldown = .5f;
	private readonly float attack2Cooldown = .5f;
	private readonly float attack3Cooldown = .5f;
	private readonly float chargingCooldown = .5f;
	private readonly float jumpAttackCooldown = .5f;


	private bool isAttacking;
	private bool isCharging;
	private bool isLanding;

	private IPlayerController playerRemote;
	private float hitStunDuration = 1;
	private AmmoHandler ammoHandler;
	private int batNormalCooldown = 10;
	private int attackHitSpecialAmount = 10;
	private int attackKillSpecialAmount = 30;
	private UnitStats stats;
	private bool isGoingToAttackAfterAttack;
	private bool isAfterHitStillAttacking;
	private bool hasReleased;
	private Vector3 aimDir;




	private void Start()
	{
		stats = GetComponent<UnitStats>();
		jumpHandler = GetComponent<JumpHandler>();
		ammoHandler = GetComponent<AmmoHandler>();
		movement = GetComponent<MovementHandler>();

		playerRemote = GetComponent<IPlayerController>();
		playerRemote.OnAttackPress += PlayerRemoteAttackPress;
		playerRemote.OnAttackRelease += PlayerRemoteAttackRelease;
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
		animEvents.OnAfterHit += Anim_AfterHit;
		animEvents.OnDash += ChargeAttackDash;
		animEvents.OnThrowStart += ThrowStart;
		animEvents.OnThrowStop += ThrowStop;
		animEvents.OnThrow += Throw;
		animEvents.OnAirThrow += Airthrow;
	}

	private float SpecialAttackDistance = 3;
	private MovementHandler movement;
	private float SpecialAttackWidth =3 ;
	[SerializeField]private GameObject aimCenter;

	private void ChargeAttackDash()
	{

	}

	private void Anim_AfterHit()
	{
		isAfterHitStillAttacking = true;
	}

	private void PlayerRemoteAttackRelease()
	{
		hasReleased = true;
	}

	private void PlayerAimStop()
	{
		OnAimStop?.Invoke();
	}

	private void PlayerRemoteOnAim(Vector3 newDirection)
	{
		aimDir = newDirection.normalized;
		if (!movement.isTryingToMove)
		{
			movement.FaceDir(aimDir.x >= 0);
		}

		Debug.Log("aiming");
		OnAim?.Invoke(aimDir);
	}

	private void Throw()
	{
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.nades, 1);
	}

	private void Airthrow()
	{
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.nades, 3);
	}

	private void Anim_AttackStart(int obj)
	{
		isAttacking = true;
	}

	private void StartStandingKunaiAttack()
	{
		OnThrow?.Invoke();
	}

	private void StartAirKunaiAttack()
	{
		OnAirThrow?.Invoke();
	}

	private void PlayerRemoteRightTriggerPress(Vector3 aimDirection)
	{
		if (!ammoHandler.HasAmmo(AmmoHandler.AmmoType.nades)) return;
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
		isAfterHitStillAttacking = false;
		if (isGoingToAttackAfterAttack) PlayerRemoteAttackPress(Vector3.zero);
	}

	private void Anim_AttackHit(int attackType)
	{
		if (attackType != 5)
		{
			var circleCast = Physics2D.OverlapCircleAll(transform.position, GetHitRange(attackType));

			foreach (var hit2D in circleCast)
			{
				HitTarget(attackType, hit2D, true);
			}
		}
		else
		{
			SpecialAttackDistance = Vector2.Distance(aimCenter.transform.position,
				Camera.main.ScreenToWorldPoint(Input.mousePosition));
			var raycast = Physics2D.CircleCastAll((Vector2) transform.position, SpecialAttackWidth, aimDir, SpecialAttackDistance,
				ASSETS.LevelAssets.EnemyLayer);
			Debug.DrawRay((Vector2)transform.position, (Vector2) aimDir *SpecialAttackDistance, Color.red);

			if (raycast.Length <= 0)
			{
				Debug.Log("miss");
				return;
			}
			OnHitConnected?.Invoke(attackType);

			foreach (var raycastHit2D in raycast)
			{
				HitTarget(attackType, raycastHit2D.collider, false);
			}


			var circleCast = Physics2D.OverlapCircleAll((Vector2) transform.position+ (Vector2)aimDir*SpecialAttackDistance, GetHitRange(attackType));

			foreach (var hit2D in circleCast)
			{
				HitTarget(attackType, hit2D, true);
			}
		}
	}

	private void HitTarget(int attackType, Collider2D hit2D, bool connect)
	{
		var enemy = hit2D.transform.gameObject.GetComponent<DefenceHandler>();
		if (enemy == null) return;
		if (enemy.IsPlayer()) return;
		if (connect)
		{
			OnHitConnected?.Invoke(attackType);
		}
		var newAttack = new Attack(transform.position, hit2D.transform.position,
			GetAttackDamage(attackType), false, HITSTUN.StunLength.Long, true, this);
		var didItKill = enemy.TakeDamage(newAttack);


		ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.ak47, attackHitSpecialAmount);
		if (!didItKill) return;
		ammoHandler.AddAmmoToReserve(AmmoHandler.AmmoType.ak47, attackKillSpecialAmount);
		OnKillEnemy?.Invoke();
	}

	private float GetHitRange(int attackType)
	{
		if (attackType == 5)
			return stats.GetStatValue(StatType.attackRange) * 2;
		else
			return stats.GetStatValue(StatType.attackRange);
	}

	private float GetAttackDamage(int attackType)
	{
		switch (attackType)
		{
			case 1:
				return 50;
			case 2:
				return 50;
			case 3:
				return 100;
			case 4:
				return 100;
			case 5:
				return 200;
			default:
				Debug.Log("damage type 0");
				return 0;
		}
	}


	private void PlayerRemoteAttackPress(Vector3 obj)
	{
		if (CanAttack())
		{
			if (!ammoHandler.HasFullAmmo(AmmoHandler.AmmoType.meleeCooldown)) return;

			isGoingToAttackAfterAttack = false;
			isAfterHitStillAttacking = false;
			if (jumpHandler.isJumping)
				StartJumpAttack();
			else
				StartRandomAttack();
		}
		else
		{
			if (!isAfterHitStillAttacking) return;
			Debug.Log("attack queued");
			isGoingToAttackAfterAttack = true;
		}
	}

	private void StartRandomAttack()
	{
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.meleeCooldown, 1000);
		var randomAttack = Random.Range(1, 3);
		switch (randomAttack)
		{
			case 1:
				Attack1();
				break;
			case 2:
				Attack2();
				break;
			default:
				Attack1();
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
	}

	private void Attack1()
	{
		Debug.Log("attack1");
		OnAttack1?.Invoke();
	}

	private void Attack2()
	{
		OnAttack2?.Invoke();
		Debug.Log("attack2");
	}



	private void PlayerRemoteChargePress()
	{
		Debug.Log("charge press");
		if (!ammoHandler.HasFullAmmo(AmmoHandler.AmmoType.ak47)) return;

		if (!CanAttack() || !jumpHandler.isDoneLanding) return;
		if (!jumpHandler.isDoneLanding) return;

		isCharging = true;
		OnUseAmmo?.Invoke(AmmoHandler.AmmoType.ak47, 100);
		OnChargeStart?.Invoke();
	}

	private void PlayerRemoteChargeRelease()
	{
		if (!isCharging) return;
		isCharging = false;
		OnChargeStop?.Invoke(true);
	}

	public Player GetPlayer()
	{
		return stats.player;
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
	public bool isBusy()
	{
		return !CanAttack();
	}

	public event Action<Vector3> OnAim;
	public event Action OnAimStop;
	public Vector2 GetAimCenter()
	{
		return aimCenter.transform.position;
	}
}
