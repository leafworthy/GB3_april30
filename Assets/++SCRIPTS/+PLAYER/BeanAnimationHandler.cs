using UnityEngine;

public class BeanAnimationHandler : MonoBehaviour
{
	public Animator topAnimator;
	public Animator bottomAnimator;

	private bool isAiming;
	private bool isAttacking;

	private MovementHandler movementHandler;
	private AnimationEvents animationEvents;
	private BeanAttackHandler attackHandler;
	Vector3 aimingDir;
	private Vector3 moveDir;
	private DefenceHandler defenceHandler;
	private JumpHandler jumpHandler;
	private static readonly int IsRunning = Animator.StringToHash("isRunning");
	private static readonly int IsFacingLeft = Animator.StringToHash("isFacingLeft");
	private static readonly int Vertical = Animator.StringToHash("Vertical");
	private static readonly int Horizontal = Animator.StringToHash("Horizontal");
	private static readonly int IsShooting = Animator.StringToHash("isShooting");
	private static readonly int IsAiming = Animator.StringToHash("isAiming");
	private static readonly int DashTrigger = Animator.StringToHash("DashTrigger");
	private static readonly int KnifeTrigger = Animator.StringToHash("KnifeTrigger");
	private static readonly int IsDead = Animator.StringToHash("isDead");
	private static readonly int ThrowTrigger = Animator.StringToHash("ThrowTrigger");
	private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
	private static readonly int LandTrigger = Animator.StringToHash("LandTrigger");
	private static readonly int ReloadTrigger = Animator.StringToHash("ReloadTrigger");
	private static readonly int FlyingTrigger = Animator.StringToHash("FlyingTrigger");
	private bool isRunning;


	private void Start()
	{
		defenceHandler = GetComponent<DefenceHandler>();
		defenceHandler.OnDying += Dying;
		defenceHandler.OnWounded += OnWounded;
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnDashStop += DashStop;
		animationEvents.OnLandingStop += LandingStop;
		animationEvents.OnRoar += OnFallInStop;


		attackHandler = GetComponent<BeanAttackHandler>();
		attackHandler.OnAim += Aim;
		attackHandler.OnAimStop += AimStop;
		attackHandler.OnShootStart += ShootStart;
		attackHandler.OnAttackStop += AttackStop;
		attackHandler.OnNadeThrowStart += NadeThrow;
		attackHandler.OnKnifeStart += KnifeStart;
		attackHandler.OnReloadStart += ReloadStart;


		movementHandler = GetComponent<MovementHandler>();
		movementHandler.OnMoveStart += MoveStart;
		movementHandler.OnMoveStop += MoveStop;
		movementHandler.OnDash += Dash;

		jumpHandler = GetComponent<JumpHandler>();
		jumpHandler.OnJump += Jump;
		jumpHandler.OnLand += Land;

		topAnimator.gameObject.SetActive(false);

	}

	private void OnWounded(Attack attack)
	{
		Debug.Log("damaged, trigger set");
		topAnimator.gameObject.SetActive(false);
		bottomAnimator.SetTrigger(FlyingTrigger);
	}

	private void OnFallInStop()
	{
		topAnimator.gameObject.SetActive(true);
	}

	private void ReloadStart()
	{
		topAnimator.SetTrigger(ReloadTrigger);
	}

	private void LandingStop()
	{
		topAnimator.gameObject.SetActive(true);
	}

	private void Land()
	{
		Debug.Log("land");
		bottomAnimator.SetTrigger(LandTrigger);
		topAnimator.gameObject.SetActive(false);
	}

	private void Jump()
	{
		Debug.Log("jump");
		topAnimator.gameObject.SetActive(false);
		bottomAnimator.SetTrigger(JumpTrigger);
	}

	private void Dying()
	{
		topAnimator.gameObject.SetActive(false);
		bottomAnimator.SetBool(IsDead,true);
	}

	private void NadeThrow()
	{
		topAnimator.SetTrigger(ThrowTrigger);
	}

	private void KnifeStart()
	{
		topAnimator.SetTrigger(KnifeTrigger);
	}

	private void DashStop()
	{
		topAnimator.gameObject.SetActive(true);
	}

	private void Dash()
	{
		topAnimator.gameObject.SetActive(false);
		bottomAnimator.SetTrigger(DashTrigger);
	}


	private void MoveStop()
	{
		Debug.Log("move stop");
		isRunning = false;
		moveDir = Vector3.zero;
	}

	private void MoveStart(Vector3 newDir)
	{
		Debug.Log("move start");
		isRunning = true;
		moveDir = newDir;
	}

	private void AttackStop()
	{
		isAttacking = false;
	}

	private void ShootStart(Attack attack)
	{
		isAttacking = true;
	}

	private void Aim(Vector3 newDirection)
	{
		aimingDir = newDirection;
		isAiming = true;
	}

	private void AimStop()
	{
		isAiming = false;
	}

	private void Update()
	{
		UpdateAnimator();
	}

	private void UpdateAnimator()
	{
		moveDir = movementHandler.GetVelocity();

		bottomAnimator.SetBool(IsRunning, isRunning);


		topAnimator.SetBool(IsFacingLeft, aimingDir.x<0);
		topAnimator.SetFloat(Vertical, aimingDir.y);
		topAnimator.SetFloat(Horizontal, aimingDir.x);
		bottomAnimator.SetFloat(Vertical, aimingDir.y);
		bottomAnimator.SetFloat(Horizontal, aimingDir.x);

		topAnimator.SetBool(IsShooting, isAttacking);
		topAnimator.SetBool(IsAiming, isAiming);
		bottomAnimator.SetBool(IsShooting, isAttacking);
		bottomAnimator.SetBool(IsAiming, isAiming);
	}
}
