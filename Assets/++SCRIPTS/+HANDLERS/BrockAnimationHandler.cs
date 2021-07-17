using UnityEngine;

public class BrockAnimationHandler : MonoBehaviour
{
	public Animator animator;

	private bool isAiming;
	private bool isLanding;
	private bool isAttacking;
	private bool isCharging;
	private bool isJumping;
	private bool isFalling;
	private bool isWalking;
	private bool isDead;

	private MovementHandler movementHandler;
	private BrockAttackHandler attackHandler;
	private DefenceHandler defenceHandler;
	private JumpHandler jumpHandler;
	private AnimationEvents animEvents;
	private DashHandler dashHandler;

	private static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
	private static readonly int IsWalking = Animator.StringToHash("isWalking");
	private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
	private static readonly int IsDead = Animator.StringToHash("isDead");
	private static readonly int IsJumping = Animator.StringToHash("isJumping");
	private static readonly int IsCharging = Animator.StringToHash("isCharging");
	private static readonly int IsFalling = Animator.StringToHash("isFalling");

	private static readonly int Attack1Trigger = Animator.StringToHash("Attack1Trigger");
	private static readonly int Attack2Trigger = Animator.StringToHash("Attack2Trigger");
	private static readonly int Attack3Trigger = Animator.StringToHash("Attack3Trigger");
	private static readonly int ChargeAttackTrigger = Animator.StringToHash("ChargeAttackTrigger");
	private static readonly int JumpAttackTrigger = Animator.StringToHash("JumpAttackTrigger");

	private static readonly int DashTrigger = Animator.StringToHash("DashTrigger");
	private static readonly int ChargeStartTrigger = Animator.StringToHash("ChargeStartTrigger");
	private static readonly int LandingStartTrigger = Animator.StringToHash("LandingStartTrigger");
	private static readonly int KunaiThrowTrigger = Animator.StringToHash("KunaiThrowTrigger");
	private static readonly int IsLanding = Animator.StringToHash("isLanding");
	private static readonly int Attacking = Animator.StringToHash("IsAttacking");



	private void Start()
	{
		attackHandler = GetComponent<BrockAttackHandler>();
		attackHandler.OnAttack1 += Attack1;
		attackHandler.OnAttack2 += Attack2;
		attackHandler.OnAttack3 += Attack3;
		attackHandler.OnChargeStart += ChargeStart;
		attackHandler.OnChargeStop += ChargeStop;
		attackHandler.OnJumpAttack += JumpAttack;
		attackHandler.OnThrow += Throw;
		attackHandler.OnAirThrow += AirThrow;

		dashHandler = GetComponent<DashHandler>();
		dashHandler.OnDashCommand += DashCommand;

		defenceHandler = GetComponent<DefenceHandler>();
		defenceHandler.OnDamaged += OnHit;
		defenceHandler.OnDying += Die;

		movementHandler = GetComponent<MovementHandler>();
		movementHandler.OnMoveStart += MoveStart;
		movementHandler.OnMoveStop += MoveStop;


		jumpHandler = GetComponent<JumpHandler>();
		jumpHandler.OnJump += Jump;
		jumpHandler.OnLand += Land;
		jumpHandler.OnFall += Fall;

		animEvents = GetComponentInChildren<AnimationEvents>();
		animEvents.OnLandingStop += LandStop;
		animEvents.OnAttackStop += AttackStop;
		animEvents.OnAttackStart += AttackStart;
	}

	private void AttackStart(int obj)
	{
		isAttacking = true;
	}

	private void AttackStop(int obj)
	{
		isAttacking = false;
		animator.ResetTrigger(ChargeAttackTrigger);
		animator.ResetTrigger(LandingStartTrigger);
	}

	private void LandStop()
	{
		isLanding = false;
	}

	private void AirThrow()
	{
		animator.SetTrigger(KunaiThrowTrigger);
	}

	private void Throw()
	{
		animator.SetTrigger(KunaiThrowTrigger);
	}

	private void DashCommand()
	{
		animator.SetTrigger(DashTrigger);
	}

	private void JumpAttack()
	{
		animator.SetTrigger(JumpAttackTrigger);
	}

	private void ChargeStop(bool isFullyCharged)
	{
		if (isFullyCharged)
		{
			animator.SetTrigger(ChargeAttackTrigger);
			isCharging = false;
			isAttacking = false;
		}
		else
		{
			isCharging = false;
			isAttacking = false;
		}
	}

	private void ChargeStart()
	{
		animator.SetTrigger(ChargeStartTrigger);

		isCharging = true;
	}

	private void Attack1()
	{
		animator.SetTrigger(Attack1Trigger);
	}

	private void Attack2()
	{
		animator.SetTrigger(Attack2Trigger);
	}

	private void Attack3()
	{
		animator.SetTrigger(Attack3Trigger);
	}

	private void Fall()
	{
		isFalling = true;
	}

	private void Land()
	{
		isFalling = false;
		isJumping = false;
		isLanding = true;
		animator.ResetTrigger(JumpAttackTrigger);
		animator.SetTrigger(LandingStartTrigger);
	}

	private void Jump()
	{
		isFalling = false;
		isJumping = true;
	}

	private void OnHit(Attack attack)
	{
		animator.SetTrigger(HitTrigger);
	}

	private void Die()
	{
		isDead = true;
		isWalking = false;
		UpdateAnimator();
	}

	private void MoveStop()
	{
		isWalking = false;
	}

	private void MoveStart(Vector3 newDir)
	{
		isWalking = true;
	}

	private void Update()
	{
		if (!isDead)
		{
			UpdateAnimator();
		}
	}

	private void UpdateAnimator()
	{
		animator.SetBool(IsWalking, isWalking);
		animator.SetBool(IsAttacking, isAttacking);
		animator.SetBool(IsDead, isDead);
		animator.SetBool(IsJumping, isJumping);
		animator.SetBool(IsCharging, isCharging);
		animator.SetBool(IsFalling, isFalling);
		animator.SetBool(IsLanding, isLanding);
	}
}
