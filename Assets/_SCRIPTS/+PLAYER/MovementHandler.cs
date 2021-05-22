using System;
using UnityEngine;

namespace _SCRIPTS
{
	public class MovementHandler : MonoBehaviour
	{
		public event Action OnDash;
		public event Action<Vector3> OnMoveStart;
		public event Action OnMoveStop;

		private AnimationEvents animEvents;
		private DefenceHandler health;
		private IMovementController controller;
		private Rigidbody2D rb;
		private UnitStats stats;

		private bool canMove = true;
		private readonly float hitPushMultiplier = 1f;

		private bool isDashing;
		private bool isMoving;
		private bool isPushed;

		private Vector3 moveDir;
		private Vector3 targetPosition;
		private Vector3 currentPushVector;

		private float pushDecayFactor = .8f;
		private float pushTime;

		private bool isOn;

		public event Action<bool> OnMoveDirectionChange;

		private void Awake()
		{
			stats = GetComponent<UnitStats>();
			rb = GetComponent<Rigidbody2D>();

			controller = GetComponent<IMovementController>();
			controller.OnMovePress += ControllerMoveInDirection;
			controller.OnMoveRelease += ControllerStopMoving;
			controller.OnDashButtonPress += ControllerDashPress;

			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnAttackStart += DisableMovement;
			animEvents.OnAttackStop += EnableMovement;
			animEvents.OnLandingStart += DisableMovement;
			animEvents.OnLandingStop += EnableMovement;
			animEvents.OnDashStart += DashStart;
			animEvents.OnDash += Dash;
			animEvents.OnDashStop += DashStop;
			animEvents.OnHitStart += DisableMovement;
			animEvents.OnHitStop += EnableMovement;
			animEvents.OnThrowStart += DisableMovement;
			animEvents.OnThrowStop += EnableMovement;
			animEvents.OnMoveStart += EnableMovement;
			animEvents.OnMoveStop += DisableMovement;

			health = GetComponent<DefenceHandler>();
			health.OnDamaged += Health_OnDamaged;
			health.OnDying += Health_Dead;
			isOn = true;
		}

		private void Health_Dead()
		{
			rb.velocity = Vector3.zero;
			currentPushVector = Vector3.zero;
			isOn = false;
			canMove = false;
			DisableAllColliders();
		}

		private void DisableAllColliders()
		{
			var colliders = GetComponents<Collider2D>();
			foreach (var col in colliders) col.enabled = false;

			var moreColliders = GetComponentsInChildren<Collider2D>();
			foreach (var col in moreColliders) col.enabled = false;
		}

		private void FixedUpdate()
		{
			if (!isOn) return;
			if (canMove)
			{
				if (isMoving)
				{
					moveDir = (targetPosition - transform.position).normalized;
					var newVel =  (Vector3)moveDir * stats.moveSpeed;
					rb.velocity = newVel;
					if (isPushed) GetPushed();
				}
				else if (isPushed)
					GetPushed();
				else
					rb.velocity = Vector2.zero;
			}
			else if (isDashing)
				rb.velocity = moveDir * stats.dashSpeed;
			else if (isPushed)
				GetPushed();
			else
				rb.velocity = Vector2.zero;
		}

		private void GetPushed()
		{
			currentPushVector *= pushDecayFactor;
			var newVel = (Vector3) rb.velocity + (Vector3) currentPushVector;
			rb.velocity = newVel;
			if (pushTime < 0)
			{
				pushTime = 0;
				isPushed = false;
			}
			else
				pushTime -= Time.fixedDeltaTime;
		}


		private void Health_OnDamaged(Vector3 DamageDirection, float DamageAmount, Vector3 DamagePosition)
		{
			Push(DamageDirection.normalized, DamageAmount, DamagePosition);
			if (rb.velocity.x > 0)
				OnMoveDirectionChange?.Invoke(false);
			else
				OnMoveDirectionChange?.Invoke(true);
		}

		public void Push(Vector3 DamageDirection, float DamageAmount, Vector3 DamagePosition)
		{
			var tempVel = Vector3.zero;
			tempVel += new Vector3(DamageDirection.x * DamageAmount * hitPushMultiplier,
				DamageDirection.y * DamageAmount * hitPushMultiplier, 0);
			isPushed = true;
			rb.velocity = tempVel;
			currentPushVector = tempVel;
			pushTime = .5f;
		}

		private void DashStart()
		{
			health.isInvincible = true;
		}

		private void Dash()
		{
			rb.MovePosition(transform.position + moveDir * stats.moveSpeed * stats.dashMultiplier);
		}

		private void DashStop()
		{
			isDashing = false;
			health.isInvincible = false;
			EnableMovement();
		}

		private void EnableMovement()
		{
			EnableMovement(0);
		}

		private void DisableMovement()
		{
			DisableMovement(0);
		}


		private void ControllerDashPress()
		{
			if (!isDashing)
			{
				Debug.Log("first start");
				isDashing = true;
				DisableMovement();
				OnDash?.Invoke();
			}
		}

		private void DisableMovement(int attackType = 0)
		{
			canMove = false;
		}

		private void EnableMovement(int attackType = 0)
		{
			if (!health.IsDeadOrDying()) canMove = true;
		}

		private void MoveTo(Vector3 target)
		{
			isMoving = true;
			targetPosition = target;
			OnMoveStart?.Invoke(target);
		}

		private void ControllerMoveInDirection(Vector3 direction)
		{
			if (canMove)
			{
				if (direction.x >= 0)
					OnMoveDirectionChange?.Invoke(true);
				else
					OnMoveDirectionChange?.Invoke(false);

				MoveTo(transform.position + direction * (stats.moveSpeed * 2));
			}
		}

		private void ControllerStopMoving()
		{
			moveDir = Vector3.zero;
			rb.velocity = moveDir;
			isMoving = false;
			OnMoveStop?.Invoke();
		}


		public Vector3 GetVelocity()
		{
			return rb.velocity;
		}

		public bool CanMove()
		{
			return canMove;
		}
	}
}
