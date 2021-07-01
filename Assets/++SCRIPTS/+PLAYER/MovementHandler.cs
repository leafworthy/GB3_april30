using System;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
	public event Action OnDash;
	public event Action<Vector3> OnMoveStart;
	public event Action OnMoveStop;
	public event Action<bool> OnMoveDirectionChange;

	private AnimationEvents animEvents;
	private DefenceHandler health;
	private IMovementController controller;
	private Rigidbody2D rb;
	private UnitStats stats;

	private bool canMove = true;

	private bool isOn;
	private bool isPressingDash;
	private bool isTryingToMove;

	private Vector2 moveVelocity;
	private Vector2 moveDir;
	private Vector2 targetPosition;
	private Vector2 velocity;

	private const float velocityDecayFactor = .9f;
	private const float overallVelocityMultiplier = 4;

	public enum PushType
	{
		low,
		normal,
		high,
		highest
	}

	private static float GetPushTypeMultiplier(PushType pushType)
	{
		switch (pushType)
		{
			case PushType.low:
				return .2f;
			case PushType.normal:
				return .3f;
			case PushType.high:
				return 1f;
			case PushType.highest:
				return 2f;
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
		animEvents.OnDashStart += DashStart;
		animEvents.OnDash += Dash;
		animEvents.OnDashStop += DashStop;
		animEvents.OnTeleport += Teleport;

		animEvents.OnAttackStart += DisableMovement;
		animEvents.OnAttackStop += EnableMovement;
		animEvents.OnLandingStart += DisableMovement;
		animEvents.OnLandingStop += EnableMovement;
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


	private Vector2 CalculateMoveVelocityWithDeltaTime()
	{
		if (!canMove) return Vector3.zero;
		moveVelocity = IsTryingToMove() ? GetMoveVelocity() : Vector2.zero;
		return moveVelocity * Time.fixedDeltaTime;
	}

	private bool IsTryingToMove()
	{
		return isTryingToMove;
	}

	private Vector2 GetMoveVelocity()
	{
		var dir = (targetPosition - (Vector2)transform.position).normalized;
		return dir * stats.GetStatValue(StatType.moveSpeed);
	}

	private void FixedUpdate()
	{
		if (!isOn) return;
		AddVelocity(CalculateMoveVelocityWithDeltaTime() * overallVelocityMultiplier);
		DecayVelocity();
		ApplyVelocity();
	}

	private void ApplyVelocity()
	{
		transform.position = (Vector2)transform.position + velocity*Time.fixedDeltaTime;
	}

	private void DecayVelocity()
	{
		var tempVel = velocity * velocityDecayFactor;
		velocity = tempVel;
	}

	private void Health_OnDamaged(Attack attack)
	{
		Push(attack.DamageDirection.normalized, PushType.highest);
		OnMoveDirectionChange?.Invoke(!(attack.DamageDirection.x > 0));
	}

	public void FaceDir(bool faceRight)
	{
		OnMoveDirectionChange?.Invoke(faceRight);
	}

	public void Push(Vector3 DamageDirection, PushType pushType)
	{
		var tempVel = Vector3.zero;
		tempVel += new Vector3(DamageDirection.x * GetPushTypeMultiplier(pushType),
			DamageDirection.y * GetPushTypeMultiplier(pushType), 0);

		AddVelocity(tempVel);
	}

	private void AddVelocity(Vector2 tempVel)
	{
		Debug.Log("add velocity " + tempVel + " to "+velocity);
		tempVel += velocity;
		velocity = tempVel;
	}

	private void DashStart()
	{
		health.isInvincible = true;
		gameObject.layer = (int) Mathf.Log(ASSETS.LevelAssets.PlayerDashLayer.value, 2);
	}

	private void Dash()
	{
		Debug.Log("trying to dash");
		Push(moveDir, PushType.highest);

	}

	private void Teleport()
	{
		var destination = (Vector2) transform.position + moveDir * stats.GetStatValue(StatType.teleportSpeed);
		rb.MovePosition(destination);
	}
	private void DashStop()
	{
		isPressingDash = false;
		health.isInvincible = false;
		gameObject.layer = (int) Mathf.Log(ASSETS.LevelAssets.PlayerLayer.value, 2);
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
		moveDir = direction;
		if (!canMove) return;
		OnMoveDirectionChange?.Invoke(direction.x >= 0);

		MoveTo(transform.position + direction * (stats.GetStatValue(StatType.moveSpeed) * 2));
	}

	private void ControllerStopMoving()
	{

		isTryingToMove = false;
		OnMoveStop?.Invoke();
	}


	public Vector3 GetVelocity()
	{
		return velocity;
	}

	public bool CanMove()
	{
		return canMove;
	}

	public Vector2 GetMoveDir()
	{
		return moveDir;
	}
}
