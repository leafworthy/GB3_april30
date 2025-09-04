using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class MoveAbility : MonoBehaviour, IActivity
	{
		private Vector2 moveVelocity;
		private Vector2 pushVelocity;

		private const float velocityDecayFactor = .90f;
		private const float overallVelocityMultiplier = 2;
		private float pushMultiplier = 1;
		private float maxPushVelocity = 3000;

		private Rigidbody2D rb;

		public Vector2 moveDir;
		private bool isMoving;
		private bool isDragging = true;

		private Vector2 lastAimDirOffset;

		private float moveSpeed;
		private bool isTryingToMove;
		private bool IsActive = true;
		private bool IsPushed;
		private Life life;
		private Body body;
		private float maxAimDistance = 30;
		public string VerbName => "Move";

		public Vector2 MoveAimDir { get; set; }

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

		public Vector3 GetMoveAimDir()
		{
			if (life.Player.isUsingMouse) return moveDir;

			return life.Player.Controller.MoveAxis.GetCurrentAngle();
		}

		private void OnEnable()
		{
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			if (life == null) return;
			life.OnDying += Life_OnDying;
		}



		private void OnDisable()
		{
			if (life == null) return;
			life.OnDying -= Life_OnDying;
		}

		private void Life_OnDying(Player player, Life life1)
		{
			IsActive = false;
			StopMoving();
			pushVelocity = Vector2.zero;
		}

		private void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			if (isMoving && IsActive)
			{
				AddMoveVelocity(GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier);
			}

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
			moveVelocity = moveDir * moveSpeed;
			return moveVelocity;
		}

		public void MoveInDirection(Vector2 direction, float newSpeed)
		{
			if (!IsActive)
			{
				Debug.Log("not active");
				return;
			}
			moveDir = direction.normalized;
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
			{
				rb.MovePosition(destination);
			}
			else
			{
				transform.position = destination;
			}
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

		public void StopMoving()
		{

			if(!life.IsHuman)Debug.Log("stop moving");
			isMoving = false;
			moveVelocity = Vector2.zero;
		}

		public void StopPush()
		{
			pushVelocity = Vector2.zero;
		}

		public bool IsMoving() => isMoving;

		public bool IsIdle() => life.Player.Controller.MoveAxis.currentMagnitudeIsTooSmall();


	}
}
