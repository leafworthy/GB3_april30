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
		const float maxPushVelocity = 10;
		const float maxAimDistance = 30;

		Rigidbody2D rb => _rb ??= GetComponent<Rigidbody2D>();
		Rigidbody2D _rb;
		Body body => _body ??= GetComponent<Body>();
		Body _body;
		UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
		UnitAnimations _anim;
		MovementController mover => _mover ??= GetComponent<MovementController>();
		MovementController _mover;
		IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
		IHaveUnitStats _stats;
		Life health => _health ??= GetComponent<Life>();
		Life _health;

		Vector2 moveVelocity;
		Vector2 pushVelocity;
		float currentMoveSpeed;
		bool isTryingToMove;
		bool isMoving;
		bool isDragging = true;
		bool IsActive = true;
		bool IsPushed;
		bool canMove = true;
		Vector2 moveDir;
		Vector2 lastMoveAimDirOffset;

		float acceleration;
		public bool accelerates;
		public float acceleratatonRate = 3;
		public float acceleratatonMax = 20;
		public Vector2 decelerationFactor = new(.97f, .97f);
		public float SturdyFactor = 1;
		float speedMultiplier = 1;
		public Vector2 GetTotalVelocity() => moveVelocity + pushVelocity;
		public Vector2 GetLastMoveAimDirOffset() => lastMoveAimDirOffset;
		public Vector2 GetMoveDir() => moveDir;
		public Vector2 GetMoveAimDir() => mover.GetMoveAimDir();
		public Vector2 GetMoveAimPoint() => (Vector2) body.AimCenter.transform.position + GetMoveAimDir().normalized * maxAimDistance;
		public bool IsMoving() => isMoving;

		public void SetCanMove(bool _canMove)
		{
			canMove = _canMove;
			if (!_canMove || !isTryingToMove)
				StopMoving();
			else
			{
				if (isTryingToMove) MoveInDirection(GetMoveAimDir(), GetMoveSpeed());
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

		void Life_DeathComplete(Player obj, bool b)
		{
			pushVelocity = Vector2.zero;
			moveVelocity = Vector2.zero;
		}

		void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isTryingToMove) MoveInDirection(GetMoveAimDir(), GetMoveSpeed());

			if (isMoving && IsActive) AddMoveVelocity(GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier);

			ApplyVelocity();
			DecayVelocity();
		}

		void ApplyVelocity()
		{
			var totalVelocity = moveVelocity + pushVelocity;
			if (health != null && health.IsDead())
			{
				var destination = (Vector2) transform.position + totalVelocity * Time.deltaTime;

				var hitWall = Physics2D.Linecast(transform.position, destination, Services.assetManager.LevelAssets.BuildingLayer);
				if (health != null && hitWall) return;
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

		Vector2 GetMoveVelocityWithDeltaTime() => GetCurrentSpeed() * Time.fixedDeltaTime;

		Vector2 GetCurrentSpeed()
		{
			moveVelocity = GetMoveDir() * currentMoveSpeed;
			return moveVelocity;
		}

		public void MoveInDirection(Vector2 direction, float newSpeed)
		{
			if (!IsActive) return;

			if (direction.magnitude == 0 || !canMove)
			{
				StopMoving();
				return;
			}

			moveDir = direction.normalized;
			body?.BottomFaceDirection(direction.x > 0);


			anim?.SetBool(UnitAnimations.IsMoving, true);
			if (accelerates)
			{
				acceleration += acceleratatonRate;
				if (acceleration > acceleratatonMax) acceleration = acceleratatonMax;
				currentMoveSpeed = newSpeed + acceleration;
			}
			else
				currentMoveSpeed = newSpeed;

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
			if (!isDragging) tempVel = Vector2.ClampMagnitude(tempVel, maxPushVelocity);
			AddPushVelocity(tempVel);
		}

		void StartMoving(Vector2 direction)
		{
			MoveInDirection(direction, GetMoveSpeed());
		}

		float GetMoveSpeed() => stats.Stats.MoveSpeed * speedMultiplier;

		public void StopMoving()
		{
			acceleration = 0;
			isMoving = false;
			if (!accelerates) moveVelocity = Vector2.zero;
			anim?.SetBool(UnitAnimations.IsMoving, false);
		}

		public void StopAllMovement()
		{
			StopMoving();
			StopPush();
			moveVelocity = Vector2.zero;
			pushVelocity = Vector2.zero;
			acceleration = 0;
			anim?.SetBool(UnitAnimations.IsMoving, false);
		}

		public void StopPush()
		{
			pushVelocity = Vector2.zero;
		}

		void Start()
		{
			if (health == null) return;
			health.OnAttackHit += Life_AttackHit;
			health.OnDead += LifeOnDead;
			health.OnDeathComplete += Life_DeathComplete;

			if (mover == null) return;
			mover.OnMoveInDirection += MoveInDirection;
			mover.OnStopMoving += MoverStopTryingToMove;
		}

		void OnDisable()
		{
			StopListeningToPlayer();
		}

		void StopListeningToPlayer()
		{
			if (health == null) return;
			if (health != null) health.OnDead -= LifeOnDead;
			health.OnAttackHit -= Life_AttackHit;
			health.OnDeathComplete -= Life_DeathComplete;

			if (mover == null) return;
			mover.OnMoveInDirection -= MoveInDirection;
			mover.OnStopMoving -= MoverStopTryingToMove;
		}

		void MoverStopTryingToMove()
		{
			isTryingToMove = false;
			StopMoving();
		}

		void Life_AttackHit(Attack attack)
		{

			if (attack.CausesFlying && !attack.DestinationLife.cantFly)
			{
				StopMoving();
				SetDragging(false);
				Push(attack.Direction, attack.DamageAmount / 2);
				return;
			}

			Push(attack.Direction, (attack.DamageAmount + attack.ExtraPush) * SturdyFactor);
		}
	}
}
