using System;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
	public event Action<Vector3> OnMoveStart;
	public event Action OnMoveStop;
	public event Action<bool> OnMoveDirectionChange;

	private AnimationEvents animEvents;
	private DefenceHandler health;
	private IMovementController controller;
	private UnitStats stats;

	public bool canMove;
	private bool isOn;
	public bool isTryingToMove;

	private Vector2 moveVelocity;
	public Vector2 moveDir;
	private Vector2 targetPosition;
	public Vector2 velocity;

	private static float velocityDecayFactor = .9f;
	public event Action OnStopAllMovement;
	private const float overallVelocityMultiplier = 4;

	private void Start()
	{
		stats = GetComponent<UnitStats>();

		controller = GetComponent<IMovementController>();
		controller.OnMovePress += ControllerMoveInDirection;
		controller.OnMoveRelease += ControllerStopMoving;

		animEvents = GetComponentInChildren<AnimationEvents>();
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
		animEvents.OnReset += Reset;

		health = GetComponent<DefenceHandler>();
		health.OnDamaged += Health_OnDamaged;
		health.OnDying += DisableMovement;
		health.OnDead += Dead;
		isOn = true;
		canMove = true;
	}

	private void Reset()
	{
		isOn = true;
		canMove = true;
	}

	private void OnDisable()
	{
		Dead();
	}

	private void Dead()
	{
		controller.OnMovePress -= ControllerMoveInDirection;
		controller.OnMoveRelease -= ControllerStopMoving;
		animEvents.OnAttackStart -= DisableMovement;
		animEvents.OnAttackStop -= EnableMovement;
		animEvents.OnLandingStart -= DisableMovement;
		animEvents.OnLandingStop -= EnableMovement;
		animEvents.OnHitStart -= DisableMovement;
		animEvents.OnHitStop -= EnableMovement;
		animEvents.OnThrowStart -= DisableMovement;
		animEvents.OnThrowStop -= EnableMovement;
		animEvents.OnMoveStart -= EnableMovement;
		animEvents.OnMoveStop -= DisableMovement;
		health.OnDamaged -= Health_OnDamaged;
		health.OnDying -= DisableMovement;
		health.OnDead -= Dead;
		OnStopAllMovement?.Invoke();
	}


	private void Health_OnDamaged(Attack attack)
	{
		FaceDir(!(attack.DamageDirection.x > 0));
	}


	private void FixedUpdate()
	{
		if (!isOn || PAUSE.isPaused) return;
		AddVelocity(CalculateMoveVelocityWithDeltaTime() * overallVelocityMultiplier);
		ApplyVelocity();
		DecayVelocity();
	}

	private void ApplyVelocity()
	{
		MoveObjectTo((Vector2) transform.position + velocity * Time.fixedDeltaTime);
	}

	private void DecayVelocity()
	{
		var tempVel = velocity * velocityDecayFactor;
		velocity = tempVel;
	}


	public void EnableMovement()
	{
		EnableMovement(0);
	}

	public void DisableMovement()
	{
		DisableMovement(0);
	}

	public void FaceDir(bool faceRight)
	{
		OnMoveDirectionChange?.Invoke(faceRight);
	}

	public void AddVelocity(Vector2 tempVel)
	{
		tempVel += velocity;
		velocity = tempVel;
	}

	public void MoveObjectTo(Vector2 destination, bool teleport = false)
	{
		var layerInt = 0;
		if (stats.isPlayer)
			layerInt = teleport ? ASSETS.LevelAssets.BuildingLayer : ASSETS.LevelAssets.EnemyLayer;
		else
			layerInt = teleport ? ASSETS.LevelAssets.BuildingLayer : ASSETS.LevelAssets.PlayerLayer;

		var ray = Physics2D.LinecastAll(transform.position, destination, ASSETS.LevelAssets.BuildingLayer);
		if (ray.Length > 0)
		{
			velocity = Vector2.zero;
			destination = ray[0].point;
		}

		transform.position = destination;
	}


	private void DisableMovement(int attackType)
	{
		canMove = false;
	}

	private void EnableMovement(int attackType)
	{
		if (!health.IsDeadOrDying()) canMove = true;
	}

	private void ControllerMoveTo(Vector3 target)
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

		ControllerMoveTo(transform.position + direction * (stats.GetStatValue(StatType.moveSpeed) * 2));
	}

	private void ControllerStopMoving()
	{
		isTryingToMove = false;
		OnMoveStop?.Invoke();
	}


	private Vector2 CalculateMoveVelocityWithDeltaTime()
	{
		if (!canMove) return Vector3.zero;
		moveVelocity = isTryingToMove ? GetMoveVelocity() : Vector2.zero;
		return moveVelocity * Time.fixedDeltaTime;
	}

	private Vector2 GetMoveVelocity()
	{
		var dir = (targetPosition - (Vector2) transform.position).normalized;
		return dir * stats.GetStatValue(StatType.moveSpeed);
	}
}
