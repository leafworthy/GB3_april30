using System;
using UnityEngine;

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
	private bool isStopped = false;

	public Vector2 moveVelocity;

	private Vector3 moveDir;
	private Vector3 targetPosition;

	private float velocityDecayFactor = .9f;

	private bool isOn;
	private const float overallVelocityMultiplier = 4;

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
				return 1f;
			case PushType.normal:
				return 2f;
			case PushType.high:
				return 2.5f;
			case PushType.highest:
				return 3f;
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
		animEvents.OnDash += Dash;
		animEvents.OnDashStop += DashStop;
		animEvents.OnHitStart += DisableMovement;
		animEvents.OnHitStop += EnableMovement;
		animEvents.OnThrowStart += DisableMovement;
		animEvents.OnThrowStop += EnableMovement;
		animEvents.OnMoveStart += EnableMovement;
		animEvents.OnMoveStop += DisableMovement;
		animEvents.OnTeleport += Teleport;

		health = GetComponent<DefenceHandler>();
		health.OnDamaged += Health_OnDamaged;
		health.OnDying += Health_Dead;
		isOn = true;
	}

	private void Health_Dead()
	{
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

	private Vector3 GetMoveVelocity()
	{
		var dir = (targetPosition - transform.position).normalized;
		return dir * stats.GetStatValue(StatType.moveSpeed);
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

	public void FaceDir(bool faceRight)
	{
		OnMoveDirectionChange?.Invoke(faceRight);
	}

	private PushType GetPushTypeFromDamage(float damageAmount)
	{
		if (damageAmount < 10) return PushType.low;

		if (damageAmount < 25) return PushType.normal;

		return !(damageAmount < 40) ? PushType.highest : PushType.high;
	}

	private void Push(Vector3 DamageDirection, float damageAmount)
	{
		Push(DamageDirection, GetPushTypeFromDamage(damageAmount));
	}

	public void Push(Vector3 DamageDirection, PushType pushType)
	{
		var tempVel = Vector3.zero;
		tempVel += new Vector3(DamageDirection.x * GetPushTypeMultiplier(pushType),
			DamageDirection.y * GetPushTypeMultiplier(pushType), 0);

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

	private void Dash()
	{
		Push(moveDir, stats.GetStatValue(StatType.dashSpeed));

	}

	private void Teleport()
	{
		rb.MovePosition(transform.position + moveDir * stats.GetStatValue(StatType.teleportSpeed));
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
		DisableMovement();
		OnDash?.Invoke();
	}

	private void DisableMovement(int attackType)
	{
		canMove = false;
	}

	private void EnableMovement(int attackType)
	{
		if (!health.IsDeadOrDying()) canMove = true;
	}

	private void MoveTo(Vector3 target)
	{
		moveDir = target - transform.position;
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

			MoveTo(transform.position + direction * (stats.GetStatValue(StatType.moveSpeed) * 2));
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
