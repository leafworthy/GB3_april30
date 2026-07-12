using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class NewMoveAbility : MonoBehaviour, ICanMove
	{


		MovementController controller => _controller ??= GetComponent<MovementController>();
		MovementController _controller;
		Rigidbody2D rb => _rb ??= GetComponent<Rigidbody2D>();
		Rigidbody2D _rb;
		Body body => _body ??= GetComponent<Body>();
		Body _body;
		Life life => _life ??= GetComponent<Life>();
		Life _life;

		NewUnitAnimations anim => _anim ??= GetComponent<NewUnitAnimations>();
		 NewUnitAnimations _anim;

		const float velocityDecayFactor = .90f;
		const float overallVelocityMultiplier = 2;
		const float pushMultiplier = 1;
		const float maxPushVelocity = 10;
		const float maxAimDistance = 30;
		const float acceleratatonRate = 3;
		const float acceleratatonMax = 20;
		Vector2 decelerationFactor = new(.97f, .97f);
		public Vector2 GetMoveDir() => moveDir;
		Vector2 moveDir;



		public float GetMoveSpeed() => moveSpeed;
		float moveSpeed;
		Vector2 moveVelocity => GetMoveDir() * GetMoveSpeed();

		public float GetPushSpeed() => pushSpeed;
		float pushSpeed;
		Vector2 pushVelocity  => GetPushDir() * GetPushSpeed();
		public Vector2 GetMoveAimDir() => controller.GetMoveAimDir();
		public Vector2 GetMoveAimPoint() => (Vector2) body.AimCenter.transform.position + GetMoveAimDir().normalized * maxAimDistance;

		Vector2 GetMoveVelocityWithDeltaTime() => moveVelocity * Time.fixedDeltaTime;

		float acceleration;


		bool isTryingToMove;

		public bool IsMoving() => isMoving;
		bool isMoving;
		bool isDragging = true;
		public bool IsIdle() => controller.IsMoving();
		bool isActive = true;
		bool isPushed;
		bool canMove = true;

		public Vector2 GetLastMoveAimDirOffset() => lastMoveAimDirOffset;
		Vector2 lastMoveAimDirOffset;

		public bool accelerates;
		public float SturdyFactor = 1;








		public void SetCanMove(bool _canMove)
		{
			canMove = _canMove;
			if (!_canMove || !isTryingToMove)
				StopMoving();
			else
			{
				if (isTryingToMove) MoveInDirection(GetMoveAimDir(), life.Stats.MoveSpeed);
			}
		}

		void LifeOnDead(Attack attack)
		{
			if (Services.pauseManager.IsPaused) return;
			body.BottomFaceDirection(attack.Direction.x < 0);
			isActive = false;
			StopMoving();
			SetCanMove(false);
			StopListeningToPlayer();
		}

		void TryToMoveInDirection(Vector2 direction)
		{
			if (!IsActive()) return;
			lastMoveAimDirOffset = GetMoveAimDir() * maxAimDistance;
			isTryingToMove = true;
			MoveInDirection(direction, life.Stats.MoveSpeed);
		}

		bool IsActive()
		{
			if (Services.pauseManager.IsPaused) return true;
			if (life.IsDead()) return true;
			return false;
		}

		void Player_MoveInDirection(IControlAxis controlAxis, Vector2 direction) => TryToMoveInDirection(direction);

		void Life_DeathComplete(Player obj, bool b)
		{
			pushVelocity = Vector2.zero;
			moveSpeed = 0;
		}

		void StopListeningToPlayer()
		{
			if (life == null) return;
			if (life != null) life.OnDead -= LifeOnDead;
			if (controller == null) return;
			controller.OnMoveInDirection -= TryToMoveInDirection;
			controller.OnStopMoving -= ControllerStopTryingToMove;
		}

		void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isTryingToMove) MoveInDirection(GetMoveAimDir(), life.Stats.MoveSpeed);

			if (IsMoving() && IsActive())
			{
				moveVelocity += GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier;
			}

			ApplyVelocity();
			DecaySpeed();
		}

		void ApplyVelocity()
		{
			var totalVelocity = moveVelocity + pushVelocity;
			var destination = (Vector2) transform.position + totalVelocity * Time.deltaTime;
			var hitWall = Physics2D.Linecast(transform.position, destination, Services.assetManager.LevelAssets.BuildingLayer);
			if(life != null && hitWall) return;
			MoveObjectTo((Vector2) transform.position + totalVelocity * Time.deltaTime);
		}

		void DecaySpeed()
		{
			if (!isDragging) return;
			moveSpeed *= velocityDecayFactor;
			pushSpeed *= velocityDecayFactor;
			if (pushSpeed < .1f) pushSpeed = 0;
		}





		public void MoveInDirection(Vector2 direction, float newSpeed)
		{
			if (!IsActive()) return;

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
			if(!isDragging) tempVel = Vector2.ClampMagnitude(tempVel, maxPushVelocity);
			AddPushVelocity(tempVel);
			PushInDirection(direction, tempVel);
		}

		void PushInDirection(Vector2 newDirection, float newSpeed)
		{
			if (!IsActive()) return;
			if (!canMove)
			{
				StopMoving();
				return;
			}
			if (newDirection.magnitude != 0)
			{
				moveDir = newDirection.normalized;
				body?.BottomFaceDirection(newDirection.x > 0);
			}
			else
			{
				StopMoving();
				return;
			}

			anim.SetMoving(true);
			isMoving = true;
		}



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

		void OnDisable()
		{
			StopListeningToPlayer();
		}

		void ControllerStopTryingToMove()
		{
			isTryingToMove = false;
			StopMoving();
		}

		void Start()
		{
			if (life == null) return;
			life.OnAttackHit += Life_AttackHit;
			life.OnDead += LifeOnDead;
			life.OnFlying += LifeOnFlying;
			life.OnDeathComplete += Life_DeathComplete;

			if (controller == null) return;
			controller.OnMoveInDirection += TryToMoveInDirection;
			controller.OnStopMoving += ControllerStopTryingToMove;
		}

		void LifeOnFlying(Attack attack)
		{
			StopMoving();
			SetDragging(false);
			Push(attack.Direction, attack.DamageAmount + attack.ExtraPush);
		}

		void Life_AttackHit(Attack attack)
		{
			MyDebugUtilities.DrawAttack(attack, Color.red);
			Push(attack.Direction, (attack.DamageAmount + attack.ExtraPush)*SturdyFactor);
		}

		public Vector2 GetTotalVelocity() => moveVelocity + pushVelocity;
	}
}
