using System.Reflection.Emit;
using UnityEngine;

namespace _SCRIPTS
{
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


		private void Start()
		{
			defenceHandler = GetComponent<DefenceHandler>();
			defenceHandler.OnDying += Dying;
			animationEvents = GetComponentInChildren<AnimationEvents>();
			animationEvents.OnDashStop += DashStop;

			attackHandler = GetComponent<BeanAttackHandler>();
			attackHandler.OnAim += Aim;
			attackHandler.OnAimStop += AimStop;
			attackHandler.OnAttackStart += AttackStart;
			attackHandler.OnAttackStop += AttackStop;
			attackHandler.OnNadeThrowStart += NadeThrow;
			attackHandler.OnKnifeStart += KnifeStart;


			movementHandler = GetComponent<MovementHandler>();
			movementHandler.OnMoveStart += MoveStart;
			movementHandler.OnMoveStop += MoveStop;
			movementHandler.OnDash += Dash;
		}

		private void Dying()
		{
			topAnimator.gameObject.SetActive(false);
			bottomAnimator.SetBool("isDead",true);
		}

		private void NadeThrow()
		{
			Debug.Log("ANIMATIONTHROW");
			topAnimator.SetTrigger("ThrowTrigger");
		}

		private void KnifeStart()
		{
			topAnimator.SetTrigger("KnifeTrigger");
		}

		private void DashStop()
		{
			topAnimator.gameObject.SetActive(true);
		}

		private void Dash()
		{
			topAnimator.gameObject.SetActive(false);
			bottomAnimator.SetTrigger("DashTrigger");
		}


		private void MoveStop()
		{
			moveDir = Vector3.zero;
		}

		private void MoveStart(Vector3 newDir)
		{
			moveDir = newDir;
		}

		private void AttackStop()
		{
			isAttacking = false;
		}

		private void AttackStart(OnAttackEventArgs obj)
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
			if (moveDir.magnitude > .02f)
			{
				bottomAnimator.SetBool("isRunning", true);
			}
			else
			{
				bottomAnimator.SetBool("isRunning", false);
			}

			topAnimator.SetBool("isFacingLeft", aimingDir.x<0);
			topAnimator.SetFloat("Vertical", aimingDir.y);
			topAnimator.SetFloat("Horizontal", aimingDir.x);
			bottomAnimator.SetFloat("Vertical", aimingDir.y);
			bottomAnimator.SetFloat("Horizontal", aimingDir.x);

			topAnimator.SetBool("isShooting", isAttacking);
			topAnimator.SetBool("isAiming", isAiming);
			bottomAnimator.SetBool("isShooting", isAttacking);
			bottomAnimator.SetBool("isAiming", isAiming);
		}
	}
}
