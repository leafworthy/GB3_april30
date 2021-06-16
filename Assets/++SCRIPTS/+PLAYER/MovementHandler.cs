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

		private bool isPressingDash;
		private bool isTryingToMove;
		private bool isPushed;
		private bool isStopped;

		public Vector2 moveVelocity;
		public Vector2 pushVelocity;

		private Vector3 moveDir;
		private Vector3 targetPosition;
		private Vector3 currentPushVector;

		private float velocityDecayFactor = .9f;

		private bool isOn;
		private float minimumPushVectorMagnitude = .5f;
		private float overallVelocityMultiplier = 4;

		public event Action<bool> OnMoveDirectionChange;

		public enum PushType
		{
		low,
		normal,
		high,
		highest
		}

		public float GetPushTypeMultiplier(PushType pushType)
		{
			switch (pushType)
			{
				case PushType.low:
					return .5f;
					break;
				case PushType.normal:
					return 1f;
					break;
				case PushType.high:
					return 1.5f;
					break;
				case PushType.highest:
					return 3f;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(pushType), pushType, null);
			}
		}
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
			animEvents.OnDash += Teleport;
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


		private Vector2 CalculateVelocityWithDeltaTime()
		{
			if (isStopped) return Vector2.zero;
			if (canMove)
			{
				if (IsDashing())
				{
					//moveVelocity = GetDashVelocity();
				}
				else if (IsTryingToMove())
					moveVelocity = GetMoveVelocity();
				else
					moveVelocity = Vector3.zero;
				return moveVelocity * Time.fixedDeltaTime;
			}

			return Vector3.zero;
		}


		private bool IsDashing()
		{
			return isPressingDash;
		}

		private bool IsTryingToMove()
		{
			return isTryingToMove;
		}

		private bool IsPushed()
		{
			return currentPushVector.magnitude < minimumPushVectorMagnitude;
		}

		private Vector2 GetPushVelocity()
		{
			return IsPushed() ? currentPushVector : Vector3.zero;
		}

		private void UpdatePushVector()
		{
			currentPushVector *= velocityDecayFactor;
		}

		private Vector3 GetMoveVelocity()
		{
			var dir = (targetPosition - transform.position).normalized;
			return dir * stats.moveSpeed;
		}

		private Vector2 GetDashVelocity()
		{
			var dir = moveDir;
			return dir * stats.dashSpeed;
		}

		private void FixedUpdate()
		{
			if (!isOn) return;
			AddVelocity(CalculateVelocityWithDeltaTime() * overallVelocityMultiplier);
			DecayVelocity();
		}

		private void DecayVelocity()
		{
			var tempVel = rb.velocity * velocityDecayFactor;
			rb.velocity = tempVel;
		}

		private void Health_OnDamaged(Attack attack)
		{
			Push(attack.DamageDirection.normalized, attack.DamageAmount);
			if (attack.DamageDirection.x > 0)
				OnMoveDirectionChange?.Invoke(false);
			else
				OnMoveDirectionChange?.Invoke(true);
		}

		private PushType GetPushTypeFromDamage(float damageAmount)
		{
			if (damageAmount < 10) return PushType.low;

			if (damageAmount < 25) return PushType.normal;

			return !(damageAmount < 40) ? PushType.highest : PushType.high;
		}

		public void Push(Vector3 DamageDirection, float damageAmount)
		{
			Push(DamageDirection, GetPushTypeFromDamage(damageAmount));
		}

		public void Push(Vector3 DamageDirection, PushType pushType)
		{
			var tempVel = Vector3.zero;
			tempVel += new Vector3(DamageDirection.x * GetPushTypeMultiplier(pushType),
				DamageDirection.y * GetPushTypeMultiplier(pushType), 0);
			isPushed = true;
			AddVelocity(tempVel * overallVelocityMultiplier);
		}

		private void AddVelocity(Vector3 tempVel)
		{
			tempVel += (Vector3) rb.velocity;
			rb.velocity = tempVel;
		}

		private void DashStart()
		{
			health.isInvincible = true;
		}

		private void Teleport()
		{
			rb.MovePosition(transform.position + moveDir * stats.moveSpeed * stats.dashMultiplier);
		}

		private void DashStop()
		{
			isPressingDash = false;
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
			if (isPressingDash) return;
			Debug.Log("first start");
			isPressingDash = true;
			Push(moveDir, stats.dashSpeed);
			DisableMovement();
			OnDash?.Invoke();
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
			moveDir = target -transform.position;
			isTryingToMove = true;
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
			isTryingToMove = false;
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
