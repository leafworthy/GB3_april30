using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class MoveAbility : MonoBehaviour, ICanMove
	{
		const float velocityDecayFactor = .90f;
		const float overallVelocityMultiplier = 2;
		const float pushMultiplier = 1;
		const float maxPushVelocity = 200;
		const float maxAimDistance = 30;

		Rigidbody2D rb => _rb ??= GetComponent<Rigidbody2D>();
		Rigidbody2D _rb;
		Body body => _body ??= GetComponent<Body>();
		Body _body;
		ISetBool anim => _anim ??= GetComponent<ISetBool>();
		ISetBool _anim;
		ICanMoveThings mover => _mover ??= GetComponent<ICanMoveThings>();
		ICanMoveThings _mover;
		IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
		IHaveUnitStats _stats;
		IGetAttacked health => _health ??= GetComponent<IGetAttacked>();
		IGetAttacked _health;

		Vector2 moveVelocity;
		Vector2 pushVelocity;
		float moveSpeed;
		bool isTryingToMove;
		bool isMoving;
		bool isDragging = true;
		bool IsActive = true;
		bool IsPushed;
		bool canMove = true;
		Vector2 moveDir;
		Vector2 lastMoveAimDirOffset;

		[SerializeField] float acceleration;
		public bool accelerates;
		public float acceleratatonRate = 3;
		public float acceleratatonMax = 20;
		public Vector2 decelerationFactor = new(.97f, .97f);
		public bool splats;

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
				if (isTryingToMove) MoveInDirection(GetMoveAimDir(), stats.Stats.MoveSpeed);
			}
		}

		void LifeOnDead(Attack attack)
		{
			if (Services.pauseManager.IsPaused) return;
			body.BottomFaceDirection(attack.Direction.x < 0);
			IsActive = false;
			StopMoving();
			SetCanMove(false);
			StopListeningToPlayer();
		}

		void MoveInDirection(Vector2 direction)
		{
			if (Services.pauseManager.IsPaused) return;
			if (health.IsDead()) return;
			lastMoveAimDirOffset = GetMoveAimDir() * maxAimDistance;
			isTryingToMove = true;
			StartMoving(direction);
		}

		void Player_MoveInDirection(IControlAxis controlAxis, Vector2 direction) => MoveInDirection(direction);

		void Life_DeathComplete(Player obj, bool b)
		{
			pushVelocity = Vector2.zero;
			moveVelocity = Vector2.zero;
		}

		void StopListeningToPlayer()
		{
			if (health == null) return;
			if (health != null) health.OnDead -= LifeOnDead;
			if (mover == null) return;
			mover.OnMoveInDirection -= MoveInDirection;
			mover.OnStopMoving -= MoverStopTryingToMove;
		}

		void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isTryingToMove) MoveInDirection(GetMoveAimDir(), stats.Stats.MoveSpeed);

			if (isMoving && IsActive) AddMoveVelocity(GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier);

			ApplyVelocity();
			DecayVelocity();
		}

		void ApplyVelocity()
		{
			var totalVelocity = moveVelocity + pushVelocity;
			//detect if hit a wall
			var destination = (Vector2) transform.position + totalVelocity * Time.deltaTime;
			if(splats)
			{
				var hitWall = Physics2D.Linecast(transform.position, destination, Services.assetManager.LevelAssets.BuildingLayer);
				if (hitWall)
				{
					var effectSurface = hitWall.collider.GetComponent<EffectSurface>();
					if (effectSurface == null) return;
					OnHitWall?.Invoke(hitWall, effectSurface.surfaceAngle);
					return;
				}
			}

			MoveObjectTo((Vector2) transform.position + totalVelocity * Time.deltaTime);
		}

		void DecayVelocity()
		{
			if (!isDragging) return;
			var tempVel = accelerates ? moveVelocity * decelerationFactor : moveVelocity * velocityDecayFactor;
			moveVelocity = tempVel;

			tempVel = pushVelocity * velocityDecayFactor;
			pushVelocity = tempVel;
			if (pushVelocity.magnitude < .1f) pushVelocity = Vector2.zero;
		}

		Vector2 GetMoveVelocityWithDeltaTime() => GetSpeed() * Time.fixedDeltaTime;

		Vector2 GetSpeed()
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

		void AddMoveVelocity(Vector2 tempVel)
		{
			tempVel += moveVelocity;
			moveVelocity = tempVel;
		}

		void AddPushVelocity(Vector2 tempVel)
		{
			tempVel += pushVelocity;
			pushVelocity = tempVel;
		}

		void MoveObjectTo(Vector2 destination)
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

		void StartMoving(Vector2 direction)
		{
			MoveInDirection(direction, stats.Stats.MoveSpeed);
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

		void OnDisable()
		{
			StopListeningToPlayer();
		}

		void MoverStopTryingToMove()
		{
			isTryingToMove = false;
			StopMoving();
		}

		void Start()
		{
			if (health == null) return;
			health.OnAttackHit += Life_AttackHit;
			health.OnDead += LifeOnDead;
			health.OnFlying += LifeOnFlying;
			health.OnDeathComplete += Life_DeathComplete;

			if (mover == null) return;
			mover.OnMoveInDirection += MoveInDirection;
			mover.OnStopMoving += MoverStopTryingToMove;
		}

		void LifeOnFlying(Attack attack)
		{
			StopMoving();
			Push(attack.Direction, attack.DamageAmount + attack.ExtraPush);
		}

		void Life_AttackHit(Attack attack)
		{
			Push(attack.Direction, attack.DamageAmount + attack.ExtraPush);
		}

		public Vector2 GetTotalVelocity() => moveVelocity + pushVelocity;

	}
}
