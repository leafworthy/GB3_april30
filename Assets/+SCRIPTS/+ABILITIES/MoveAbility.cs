using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{


	[ExecuteAlways]
	public class MoveAbility : MonoBehaviour
	{
		private const float velocityDecayFactor = .90f;
		private const float overallVelocityMultiplier = 2;
		private const float pushMultiplier = 1;
		private const float maxPushVelocity = 3000;
		private const float maxAimDistance = 30;

		private Rigidbody2D rb => _rb ??= GetComponent<Rigidbody2D>();
		private Rigidbody2D _rb;
		private Body body => _body ??= GetComponent<Body>();
		private Body _body;
		private UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		private UnitAnimations _anim;
		private ICanMoveThings mover => _mover ??= GetComponent<ICanMoveThings>();
		private ICanMoveThings _mover;

		private IHaveAttackStats stats => _stats ??= GetComponent<IHaveAttackStats>();
		private IHaveAttackStats _stats;

		private Vector2 moveVelocity;
		private Vector2 pushVelocity;
		private float moveSpeed;
		private bool isTryingToMove;
		private bool isMoving;
		private bool isDragging = true;
		private bool IsActive = true;
		private bool IsPushed;
		private bool canMove = true;
		private Vector2 moveDir;
		private Vector2 lastMoveAimDirOffset;

		[SerializeField] private float acceleration;
		public bool accelerates;
		public float acceleratatonRate = 3;
		public float acceleratatonMax = 20;
		public Vector2 decelerationFactor = new(.97f, .97f);
		private IGetAttacked health  => _health ??= GetComponent<IGetAttacked>();
		private IGetAttacked _health;

		public bool GetCanMove() => canMove;
		public Vector2 GetLastMoveAimDirOffset() => lastMoveAimDirOffset;
		public Vector2 GetMoveDir() => moveDir;
		public Vector2 GetMoveAimDir() => mover.GetMoveAimDir();
		public Vector2 GetMoveAimPoint() => (Vector2) body.AimCenter.transform.position + GetMoveAimDir().normalized * maxAimDistance;
		public bool IsMoving() => isMoving;

		public bool IsMovingQuickly(float amount) => GetTotalVelocity().magnitude > amount;

		public bool IsIdle() => mover.IsMoving();

		public event Action<RaycastHit2D, EffectSurface.SurfaceAngle> OnHitWall;


		public void SetCanMove(bool _canMove)
		{
			canMove = _canMove;
			if (!_canMove || !isTryingToMove)
				StopMoving();
			else
			{
				if (isTryingToMove) MoveInDirection(GetMoveAimDir(), stats.MoveSpeed);
			}
		}

		private void LifeOnDead(Attack attack)
		{
			if (Services.pauseManager.IsPaused) return;
			body.BottomFaceDirection(attack.Direction.x < 0);
			IsActive = false;
			StopMoving();
			SetCanMove(false);
			StopListeningToPlayer();
		}

		private void MoveInDirection(Vector2 direction)
		{
			if (Services.pauseManager.IsPaused) return;
			if (health.IsDead()) return;
			lastMoveAimDirOffset = GetMoveAimDir() * maxAimDistance;
			isTryingToMove = true;
			StartMoving(direction);
		}

		private void Player_MoveInDirection(IControlAxis controlAxis, Vector2 direction) => MoveInDirection(direction);

		private void Life_DeathComplete(Player obj, bool b)
		{
			pushVelocity = Vector2.zero;
			moveVelocity = Vector2.zero;
		}

		private void StopListeningToPlayer()
		{
			if (health == null) return;
			if (health != null) health.OnDead -= LifeOnDead;
			if (mover == null) return;
			mover.OnMoveInDirection -= MoveInDirection;
			mover.OnStopMoving -= MoverStopTryingToMove;
		}

		private void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isTryingToMove) MoveInDirection(GetMoveAimDir(), stats.MoveSpeed);

			if (isMoving && IsActive) AddMoveVelocity(GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier);

			ApplyVelocity();
			DecayVelocity();
		}

		private void ApplyVelocity()
		{
			var totalVelocity = moveVelocity + pushVelocity;
			//detect if hit a wall
			var destination = (Vector2) transform.position + totalVelocity * Time.deltaTime;
			var hitWall = Physics2D.Linecast(transform.position, destination, Services.assetManager.LevelAssets.BuildingLayer);
			if (hitWall)
			{
				var effectSurface = hitWall.collider.GetComponent<EffectSurface>();
				if (effectSurface == null) return;
				Debug.Log("hit wall " + hitWall.collider.name + " at " + hitWall.point + " angle " + effectSurface.surfaceAngle, this);
				OnHitWall?.Invoke(hitWall, effectSurface.surfaceAngle);
				return;
			}

			MoveObjectTo((Vector2) transform.position + totalVelocity * Time.deltaTime);
		}

		private void DecayVelocity()
		{
			if (!isDragging) return;
			var tempVel = accelerates ? moveVelocity * decelerationFactor : moveVelocity * velocityDecayFactor;
			moveVelocity = tempVel;

			tempVel = pushVelocity * velocityDecayFactor;
			pushVelocity = tempVel;
			if (pushVelocity.magnitude < .1f) pushVelocity = Vector2.zero;
		}

		private Vector2 GetMoveVelocityWithDeltaTime() => GetSpeed() * Time.fixedDeltaTime;

		private Vector2 GetSpeed()
		{
			moveVelocity = GetMoveDir() * moveSpeed;
			return moveVelocity;
		}

		public void MoveInDirection(Vector2 direction, float newSpeed)
		{
			if (!IsActive) return;

			if (direction.magnitude != 0)
			{
				moveDir = direction.normalized;
				body?.BottomFaceDirection(direction.x > 0);
			}
			else
			{
				StopMoving();
				return;
			}

			if (!canMove)
			{
				StopMoving();
				return;
			}

			anim?.SetBool(UnitAnimations.IsMoving, true);
			if (accelerates)
			{
				acceleration += acceleratatonRate;
				if (acceleration > acceleratatonMax) acceleration = acceleratatonMax;
				moveSpeed = newSpeed + acceleration;
			}
			else
				moveSpeed = newSpeed;

			isMoving = true;
		}

		private void AddMoveVelocity(Vector2 tempVel)
		{
			tempVel += moveVelocity;
			moveVelocity = tempVel;
		}

		private void AddPushVelocity(Vector2 tempVel)
		{
			Debug.Log("push vel before " + pushVelocity + " adding " + tempVel);
			tempVel += pushVelocity;
			pushVelocity = tempVel;
		}

		private void MoveObjectTo(Vector2 destination)
		{
			if (Services.pauseManager.IsPaused) return;
			if (rb != null)
				rb.MovePosition(destination);
			else
				transform.position = destination;
		}

		public void SetDragging(bool isNowDragging)
		{
			isDragging = isNowDragging;
		}

		public void Push(Vector2 direction, float speed)
		{
			direction = direction.normalized * pushMultiplier;
			var tempVel = new Vector2(direction.x * speed, direction.y * speed);
			tempVel = Vector2.ClampMagnitude(tempVel, maxPushVelocity);
			AddPushVelocity(tempVel);
		}

		private void StartMoving(Vector2 direction)
		{
			MoveInDirection(direction, stats.MoveSpeed);
		}

		public void StopMoving()
		{
			acceleration = 0;
			isMoving = false;
			if (!accelerates) moveVelocity = Vector2.zero;
			anim?.SetBool(UnitAnimations.IsMoving, false);
		}

		public void StopPush()
		{
			pushVelocity = Vector2.zero;
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void MoverStopTryingToMove()
		{
			isTryingToMove = false;
			StopMoving();
		}

		private void Start()
		{
			if (health == null) return;
			health.OnAttackHit += Life_AttackHit;
			health.OnDead += LifeOnDead;
			health.OnDeathComplete += Life_DeathComplete;

			if (mover == null) return;
			mover.OnMoveInDirection += MoveInDirection;
			mover.OnStopMoving += MoverStopTryingToMove;
		}

		private void Life_AttackHit(Attack attack)
		{
			//if (stats.IsDead()) return;
			Push(attack.Direction, attack.DamageAmount + attack.ExtraPush);
		}

		public Vector2 GetTotalVelocity() => moveVelocity + pushVelocity;

	}

}
