using UnityEngine;

namespace _SCRIPTS
{
	public class ToastAnimationHandler : MonoBehaviour
	{
		private Animator animator;
		private MovementHandler movementHandler;
		private ToastAttackHandler toastAttackHandler;
		private DefenceHandler defenceHandler;

		private bool isAttacking;
		private bool isMoving;
		private bool isDead;

		private static readonly int HitTrigger = Animator.StringToHash("HitTrigger");

		private void Start()
		{
			animator = GetComponentInChildren<Animator>();

			toastAttackHandler = GetComponent<ToastAttackHandler>();
			toastAttackHandler.OnAttackStart += AttackStart;
			toastAttackHandler.OnAttackStop += AttackStop;

			defenceHandler = GetComponent<DefenceHandler>();
			defenceHandler.OnDying += Defence_OnDying;
			defenceHandler.OnDamaged += Defence_OnDamaged;

			movementHandler = GetComponent<MovementHandler>();
			movementHandler.OnMoveStart += MoveStart;
			movementHandler.OnMoveStop += MoveStop;
		}


		private void Defence_OnDamaged(Vector3 vector3, float f, Vector3 arg3)
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
}
