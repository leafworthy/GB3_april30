using System;
using GangstaBean.Core;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace __SCRIPTS
{
	public class NewMoveAbility : MonoBehaviour, ICanMove
	{
		public AnimationClip moveAnimationClip;
		public AnimationClip standAnimationClip;
		MovementController controller => _controller ??= GetComponent<MovementController>();
		MovementController _controller;
		Rigidbody2D rb => _rb ??= GetComponent<Rigidbody2D>();
		Rigidbody2D _rb;
		Body body => _body ??= GetComponent<Body>();
		Body _body;
		Life life => _life ??= GetComponent<Life>();
		Life _life;

		NewUnitAnimationPlayer anim => _anim ??= GetComponent<NewUnitAnimationPlayer>();
		NewUnitAnimationPlayer _anim;

		const float speedDecayFactor = .90f;
		const float overallVelocityMultiplier = 2;
		const float pushMultiplier = 1;
		public float PushSturdyMultiplier = 1;
		const float maxPushSpeed = 10;
		const float maxAimDistance = 30;

		Vector2 moveDir;
		float moveSpeed;
		Vector2 moveVelocity => moveSpeed * moveDir;

		Vector2 pushDir;
		float pushSpeed;
		Vector2 pushVelocity => pushDir * pushSpeed;
		public Vector2 GetMoveAimDir() => controller.GetMoveAimDir();
		public Vector2 GetMoveAimPoint() => (Vector2) body.AimCenter.transform.position + GetMoveAimDir().normalized * maxAimDistance;
		public Vector2 GetTotalVelocity() => moveVelocity + pushVelocity;

		bool canMove = true;
		bool isTryingToMove;
		bool isMoving;
		bool isPushing;
		bool isDragging = true;

		public Vector2 GetLastMoveAimDirOffset() => lastMoveAimDirOffset;
		Vector2 lastMoveAimDirOffset;

		public event Action OnPush;
		public event Action OnMove;
		public event Action OnStopMoving;
		public event Action OnStopPushing;

		void Start()
		{
			if (life == null) return;
			life.OnAttackHit += Life_AttackHit;
			life.OnDead += LifeOnDead;
			life.OnFlying += LifeOnFlying;
			life.OnDeathComplete += Life_DeathComplete;

			if (controller == null) return;
			controller.OnMoveInDirection += ControllerTryToMove;
			controller.OnStopMoving += ControllerStopTryingToMove;
		}
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
			StopMoving();
			SetCanMove(false);
			StopListeningToPlayer();
		}

		void ControllerTryToMove(Vector2 direction)
		{
			if (!IsActive()) return;
			lastMoveAimDirOffset = GetMoveAimDir() * maxAimDistance;
			isTryingToMove = true;
			MoveInDirection(direction, life.Stats.MoveSpeed);
		}

		bool IsActive()
		{
			if (Services.pauseManager.IsPaused) return false;
			if (life.IsDead()) return false;
			return true;
		}


		void Life_DeathComplete(Player obj, bool b)
		{
			pushSpeed = 0;
			moveSpeed = 0;
		}

		void StopListeningToPlayer()
		{
			if (life == null) return;
			if (life != null) life.OnDead -= LifeOnDead;
			if (controller == null) return;
			controller.OnMoveInDirection -= ControllerTryToMove;
			controller.OnStopMoving -= ControllerStopTryingToMove;
		}

		void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isTryingToMove) MoveInDirection(GetMoveAimDir(), life.Stats.MoveSpeed);

			if (isMoving && IsActive()) moveSpeed = life.Stats.MoveSpeed * Time.fixedDeltaTime * overallVelocityMultiplier;

			ApplyVelocity();
			DecaySpeed();
		}

		void ApplyVelocity()
		{
			var totalVelocity = moveVelocity + pushVelocity;
			var destination = (Vector2) transform.position + totalVelocity * Time.deltaTime;
			var hitWall = Physics2D.Linecast(transform.position, destination, Services.assetManager.LevelAssets.BuildingLayer);
			if (life != null && hitWall) return;
			MoveObjectTo((Vector2) transform.position + totalVelocity * Time.deltaTime);
		}

		void DecaySpeed()
		{
			if (!isDragging) return;
			moveSpeed *= speedDecayFactor;
			pushSpeed *= speedDecayFactor;
			if (pushSpeed < .1f) pushSpeed = 0;
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
			if (!IsActive()) return;
			if (!canMove || direction.magnitude == 0)
			{
				StopMoving();
				return;
			}

			pushDir = direction.normalized * pushMultiplier;
			pushSpeed = Mathf.Clamp(speed, 0, maxPushSpeed);
			isPushing = true;
			OnPush?.Invoke();
		}

		public void MoveInDirection(Vector2 direction, float newSpeed)
		{
			if (!IsActive()) return;

			if (direction.magnitude == 0 || !canMove)
			{
				StopMoving();
				return;
			}

			moveDir = direction.normalized;
			moveSpeed = newSpeed;
			body?.BottomFaceDirection(direction.x > 0);
			isMoving = true;
			OnMove?.Invoke();
		}

		public void StopMoving()
		{
			isMoving = false;
			moveSpeed = 0;
			moveDir = Vector2.zero;
			OnStopMoving?.Invoke();
		}

		public void StopPushing()
		{
			pushSpeed = 0;
			pushDir = Vector2.zero;
			isPushing = false;
			OnStopPushing?.Invoke();
		}

		public void StopAllMovement()
		{
			StopMoving();
			StopPushing();
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



		void LifeOnFlying(Attack attack)
		{
			StopMoving();
			SetDragging(false);
			Push(attack.Direction, attack.DamageAmount + attack.ExtraPush);
		}

		void Life_AttackHit(Attack attack)
		{
			Push(attack.Direction, (attack.DamageAmount + attack.ExtraPush) * PushSturdyMultiplier);
		}


	}
}
