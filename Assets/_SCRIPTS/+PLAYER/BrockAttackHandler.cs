using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _SCRIPTS
{
	public class BrockAttackHandler : MonoBehaviour
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

		private PlayerController player;
		private UnitStats stats;

		private void Awake()
		{
			stats = GetComponent<UnitStats>();
			jumpHandler = GetComponent<JumpHandler>();

			player = GetComponent<PlayerController>();
			player.OnAttackPress += PlayerAttackPress;
			player.OnAttackRelease += PlayerAttackRelease;
			player.OnChargePress += PlayerChargePress;
			player.OnChargeRelease += PlayerChargeRelease;
			player.OnRightTriggerPress += PlayerRightTriggerPress;
			player.OnRightTriggerRelease += PlayerRightTriggerRelease;

			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnAttackHit += Anim_AttackHit;
			animEvents.OnAttackStop += Anim_AttackStop;
			animEvents.OnLandingStart += Anim_LandingStart;
			animEvents.OnLandingStop += Anim_LandingStop;

			animEvents.OnThrowStart += ThrowStart;
			animEvents.OnThrowStop += ThrowStop;
		}

		private void PlayerRightTriggerRelease()
		{
			isAttacking = false;
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

		private void PlayerRightTriggerPress(Vector3 aimDirection)
		{
			if (!CantAttack())
			{
				Debug.Log("AIM ATTACK");
				isAttacking = true;
				if (jumpHandler.isJumping)
					StartAirKunaiAttack();
				else
					StartStandingKunaiAttack();
			}
			else
				Debug.Log("isbusy");
		}

		private void Update()
		{
			HandleCooldown();
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
		}

		private void Anim_AttackHit(int attackType)
		{
			float hitRange = 15;
			var circleCast = Physics2D.OverlapCircleAll(transform.position, hitRange);
			foreach (var hit2D in circleCast)
			{
				var enemy = hit2D.transform.gameObject.GetComponent<DefenceHandler>();
				if (enemy != null)
				{
					if (!enemy.IsPlayer())
					{
						var attackDirection = hit2D.transform.position - transform.position;
						enemy.TakeDamage(attackDirection, GetAttackDamage(attackType),
							enemy.transform.position);
					}
				}
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
					return 100;
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

		private void PlayerAttackRelease()
		{
			isAttacking = false;
		}

		private void PlayerAttackPress(Vector3 obj)
		{
			if (!CantAttack())
			{
				Debug.Log("ATTACK");
				isAttacking = true;
				if (jumpHandler.isJumping)
					StartJumpAttack();
				else
					StartRandomAttack();
			}
			else
				Debug.Log("isbusy");
		}

		private void StartRandomAttack()
		{
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

		public bool CantAttack()
		{
			return isAttacking || isCharging  || isLanding;
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

		private void PlayerChargePress()
		{
			if (!CantAttack() && jumpHandler.isStanding)
			{
				if (jumpHandler.isStanding)
				{
					isCharging = true;
					coolDown = chargingCooldown;
					OnChargeStart?.Invoke();
				}
			}
		}

		private void PlayerChargeRelease()
		{
			if (isCharging)
			{
				isCharging = false;
				OnChargeStop?.Invoke(!isCoolingDown);
				if (isCoolingDown) animEvents.AttackStop();
			}
		}
	}
}
