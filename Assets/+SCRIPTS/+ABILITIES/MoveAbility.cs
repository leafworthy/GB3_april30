using System;
using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public interface IMove
	{
		event Action<Vector2> OnMoveInDirection;
		event Action OnStopMoving;
	}
	public class MoveAbility : Ability, IActivity, IPoolable
	{
		private Vector2 moveVelocity;
		private Vector2 pushVelocity;

		private const float velocityDecayFactor = .90f;
		private const float overallVelocityMultiplier = 2;
		private float pushMultiplier = 1;
		private float maxPushVelocity = 3000;
		private Rigidbody2D rb;

		private bool isMoving;
		private bool isDragging = true;

		private Vector2 lastAimDirOffset;

		private IMove ai;

		private float moveSpeed;
		private bool isTryingToMove;
		private bool IsActive = true;
		private bool IsPushed;
		private float maxAimDistance = 30;
		public override string VerbName => "Move";
		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => false;

		protected override void DoAbility()
		{
		}

		public bool CanMove => canMove;
		private bool canMove;
		private Vector2 MoveAimDir { get; set; }
		public Vector2 MoveDir { get; private set; }

		public override void SetPlayer(Player _player)
		{
			player = _player;

			canMove = true;
			StartListeningToPlayer();
		}

		public Vector2 GetMoveAimPoint()
		{
			if (life == null || !life.IsHuman) return Vector2.zero;
			MoveAimDir = GetMoveAimDir();
			lastAimDirOffset = MoveAimDir * maxAimDistance;
			if (!life.Player.isUsingMouse) return (Vector2) body.AimCenter.transform.position + MoveAimDir.normalized * maxAimDistance;
			var mousePos = CursorManager.GetMousePosition();

			return (Vector2) body.AimCenter.transform.position + MoveAimDir.normalized * maxAimDistance;
		}

		public Vector2 GetLastMoveAimDirOffset() => lastAimDirOffset;

		public Vector2 GetMoveAimDir()
		{
			if (life.Player.isUsingMouse) return MoveDir;

			return life.Player.Controller.MoveAxis.GetCurrentAngle();
		}

		public void SetCanMove(bool _canMove)
		{
			canMove = _canMove;
			if (!_canMove)
				StopMoving();
			else
			{
				if (player.IsPlayer()) MoveInDirection(player.Controller.MoveAxis.GetCurrentAngle(), life.MoveSpeed);
			}
		}

		private void Life_OnDying(Attack attack)
		{
			Push(attack.Direction, attack.DamageAmount);
			body.BottomFaceDirection(attack.Direction.x < 0);
			IsActive = false;
			StopMoving();
			SetCanMove(false);
			pushVelocity = Vector2.zero;
			StopListeningToPlayer();
		}

		private void Controller_MoveInDirection(Vector2 direction)
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			if (!CanMove)
			{
				Debug.Log("can't move");
				return;
			}

			if (body.doableLegs.IsActive || direction.magnitude < .5f)
			{
				StopMoving();
				return;
			}

			StartMoving(direction);
		}

		private void Player_MoveInDirection(IControlAxis controlAxis, Vector2 direction)
		{
			Controller_MoveInDirection(direction);
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			if (!CanMove) return;

			if (body.doableLegs.IsActive || direction.magnitude < .5f)
			{
				StopMoving();
				return;
			}

			StartMoving(direction);
		}

		private void Life_DeathComplete(Player obj)
		{
			SetCanMove(false);
			pushVelocity = Vector2.zero;
		}

		private void StopListeningToPlayer()
		{
			if (life == null) return;
			if (life != null) life.OnDying -= Life_OnDying;
			if (life.IsHuman)
			{
				player.Controller.MoveAxis.OnChange -= Player_MoveInDirection;
				player.Controller.MoveAxis.OnInactive -= Player_StopMoving;
			}
			else
			{
				if (ai == null) return;
				ai.OnMoveInDirection -= Controller_MoveInDirection;
				ai.OnStopMoving -= AI_StopMoving;
			}
		}

		private void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isMoving && IsActive) AddMoveVelocity(GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier);

			ApplyVelocity();
			DecayVelocity();
		}

		private void ApplyVelocity()
		{
			var totalVelocity = moveVelocity + pushVelocity;
			MoveObjectTo((Vector2) transform.position + totalVelocity * Time.deltaTime);
		}

		private void DecayVelocity()
		{
			if (!isDragging) return;
			var tempVel = moveVelocity * velocityDecayFactor;
			moveVelocity = tempVel;
			tempVel = pushVelocity * velocityDecayFactor;
			pushVelocity = tempVel;
			if (pushVelocity.magnitude < .1f) pushVelocity = Vector2.zero;
		}

		private Vector2 GetMoveVelocityWithDeltaTime() => GetVelocity() * Time.fixedDeltaTime;

		private Vector2 GetVelocity()
		{
			moveVelocity = MoveDir * moveSpeed;
			return moveVelocity;
		}

		public void MoveInDirection(Vector2 direction, float newSpeed)
		{
			if (!IsActive) return;

			if (direction.x != 0) body.BottomFaceDirection(direction.x > 0);
			anim.SetBool(UnitAnimations.IsMoving, true);

			MoveDir = direction.normalized;
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
			tempVel += pushVelocity;
			pushVelocity = tempVel;
		}

		private void MoveObjectTo(Vector2 destination, bool teleport = false)
		{
			rb = GetComponent<Rigidbody2D>();
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

			MoveInDirection(direction, life.MoveSpeed);
		}
		public void StopMoving()
		{
			if (!life.IsHuman) Debug.Log("stop moving");
			isMoving = false;
			moveVelocity = Vector2.zero;
			anim.SetBool(UnitAnimations.IsMoving, false);
		}

		public void StopPush()
		{
			pushVelocity = Vector2.zero;
		}

		public bool IsMoving() => isMoving;

		public bool IsIdle() => life.Player.Controller.MoveAxis.currentMagnitudeIsTooSmall();

		public void OnPoolSpawn()
		{
			IsActive = true;
			isMoving = false;
			moveVelocity = Vector2.zero;
			pushVelocity = Vector2.zero;
		}

		public void OnPoolDespawn()
		{
			IsActive = false;
			isMoving = false;
			moveVelocity = Vector2.zero;
			pushVelocity = Vector2.zero;
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void Player_StopMoving(IControlAxis controlAxis) => StopMoving();

		private void AI_StopMoving() => StopMoving();

		private void StartListeningToPlayer()
		{
			if (life == null) return;
			life.OnDying += Life_OnDying;
			life.OnDeathComplete += Life_DeathComplete;
			if (player.IsPlayer())
			{
				player.Controller.MoveAxis.OnChange += Player_MoveInDirection;
				player.Controller.MoveAxis.OnInactive += Player_StopMoving;
			}
			else
			{
				ai = GetComponent<IMove>();
				if (ai == null) return;

				ai.OnMoveInDirection += Controller_MoveInDirection;
				ai.OnStopMoving += AI_StopMoving;
			}
		}
	}
}
