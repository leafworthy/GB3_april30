using UnityEngine;

public class ConeAnimationHandler : MonoBehaviour
{
	private Animator animator;
	private MovementHandler movementHandler;
	private ConeAttackHandler coneAttackHandler;
	private DefenceHandler defenceHandler;

	private bool isAttacking;
	private bool isMoving;
	private bool isDead;

	private static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
	private EnemyAI ai;

	private static readonly int AggroTrigger = Animator.StringToHash("AggroTrigger");


	private void Start()
	{
		ai = GetComponent<EnemyAI>();
		ai.OnAggro += AI_OnAggro;
		animator = GetComponentInChildren<Animator>();

		coneAttackHandler = GetComponent<ConeAttackHandler>();
		coneAttackHandler.OnAttackStart += AttackStart;
		coneAttackHandler.OnAttackStop += AttackStop;

		defenceHandler = GetComponent<DefenceHandler>();
		defenceHandler.OnDying += Defence_OnDying;
		defenceHandler.OnDamaged += Defence_OnDamaged;

		movementHandler = GetComponent<MovementHandler>();
		movementHandler.OnMoveStart += MoveStart;
		movementHandler.OnMoveStop += MoveStop;
	}

	private void AI_OnAggro()
	{
		animator.SetTrigger(AggroTrigger);
	}

	private void Defence_OnDamaged(Attack attack)
	{
		animator.SetTrigger(HitTrigger);
	}

	private void Defence_OnDying()
	{
		isDead = true;
		isMoving = false;
		UpdateAnimator();
	}

	private void MoveStop()
	{
		isMoving = false;
	}

	private void MoveStart(Vector3 newDir)
	{
		isMoving = true;
	}

	private void AttackStop()
	{
		isAttacking = false;
	}

	private void AttackStart()
	{
		animator.SetTrigger("AttackTrigger");
	}

	void Update()
	{
		if (!isDead)
		{
			UpdateAnimator();
		}
	}


	void UpdateAnimator()
	{
		animator.SetBool("isWalking", isMoving);
		animator.SetBool("isAttacking", isAttacking);
		animator.SetBool("isDead", isDead);
	}
}
