using UnityEngine;

public class BeanAnimationHandler : MonoBehaviour
{
	public Animator topAnimator;
	public Animator bottomAnimator;

	private bool isAiming;
	private bool isDead;
	private bool isAttacking;
	private bool isRunning;

	private DashHandler dashHandler;
	private MovementHandler movementHandler;
	private AnimationEvents animationEvents;
	private BeanAttackHandler attackHandler;
	private Vector3 aimingDir;
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

	private static readonly int IsGlocking = Animator.StringToHash("isGlocking");
	private SpriteRenderer topSpriteRenderer;

	private void Start()
	{
		dashHandler = GetComponent<DashHandler>();
		dashHandler.OnDashCommand += DashCommand;

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

		jumpHandler = GetComponent<JumpHandler>();
		jumpHandler.OnJump += Jump;
		jumpHandler.OnLand += Land;

		topSpriteRenderer = topAnimator.GetComponent<SpriteRenderer>();
		SetTopActive(false);
	}



	private void OnWounded(Attack attack)
	{
		SetTopActive(false);
		bottomAnimator.SetTrigger(FlyingTrigger);
	}

	private void SetTopActive(bool on)
	{
		topSpriteRenderer.enabled = on;
	}

	private void OnFallInStop()
	{
		SetTopActive(true);
	}

	private void ReloadStart()
	{
		bottomAnimator.SetBool(IsGlocking, attackHandler.IsGlocking);
		topAnimator.SetBool(IsGlocking, attackHandler.IsGlocking);
		SetTopActive(true);
		topAnimator.SetTrigger(ReloadTrigger);
	}

	private void LandingStop()
	{
		SetTopActive(true);
	}

	private void Land()
	{
		SetTopActive(false);
		bottomAnimator.SetTrigger(LandTrigger);
	}

	private void Jump()
	{
		SetTopActive(false);
		bottomAnimator.SetTrigger(JumpTrigger);
	}

	private void Dying()
	{
		SetTopActive(false);
		isDead = true;
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
		SetTopActive(true);
	}

	private void DashCommand()
	{
		SetTopActive(false);
		bottomAnimator.SetTrigger(DashTrigger);
	}

	private void MoveStop()
	{
		isRunning = false;
	}

	private void MoveStart(Vector3 newDir)
	{
		isRunning = true;
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
		var clampedDir = aimingDir;
		if ((clampedDir.x < .25f) && (clampedDir.x > 0))
		{
			clampedDir.x = .25f;
		}

		if ((clampedDir.x < 0) && (clampedDir.x > -.25))
		{
			clampedDir.x = -.25f;
		}
		bottomAnimator.SetBool(IsGlocking, attackHandler.IsGlocking);
		topAnimator.SetBool(IsGlocking, attackHandler.IsGlocking);
		bottomAnimator.SetBool(IsRunning, isRunning);
		topAnimator.SetBool(IsFacingLeft, clampedDir.x < 0);
		topAnimator.SetFloat(Vertical, clampedDir.y);
		topAnimator.SetFloat(Horizontal, clampedDir.x);
		bottomAnimator.SetFloat(Vertical, clampedDir.y);
		bottomAnimator.SetFloat(Horizontal, clampedDir.x);
		topAnimator.SetBool(IsShooting, isAttacking);
		topAnimator.SetBool(IsAiming, isAiming);
		bottomAnimator.SetBool(IsShooting, isAttacking);
		bottomAnimator.SetBool(IsAiming, isAiming);
		bottomAnimator.SetBool(IsDead,isDead);
		topAnimator.SetBool(IsDead, isDead);



	}
}
