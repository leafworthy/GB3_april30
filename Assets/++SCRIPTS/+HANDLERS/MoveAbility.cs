using UnityEngine;

public class MoveAbility : MonoBehaviour
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


	private float moveSpeed;
	private bool isTryingToMove;
	private bool IsActive = true;
	private bool IsPushed;
	private Life life;
	private void Start()
	{
		life = GetComponent<Life>();
		if (life == null) return;
		life.OnDying += Life_OnDying;
	}



	private void Life_OnDying(Player arg1, Life arg2)
	{
		IsActive = false;
		StopMoving();
		pushVelocity = Vector2.zero;
	}

	private void FixedUpdate()
	{
		if (Game_GlobalVariables.IsPaused) return;
		
		if (isMoving && IsActive)
		{
			AddMoveVelocity(GetMoveVelocityWithDeltaTime() * overallVelocityMultiplier);
			
		}

		ApplyVelocity();
		DecayVelocity();
	}

	private void ApplyVelocity()
	{
		var totalVelocity =  moveVelocity + pushVelocity;
		MoveObjectTo((Vector2) transform.position+ totalVelocity * Time.fixedDeltaTime);
	}

	private void DecayVelocity()
	{
		if (!isDragging) return;
		var tempVel = moveVelocity * velocityDecayFactor;
		moveVelocity = tempVel;
		tempVel = pushVelocity * velocityDecayFactor;
		pushVelocity = tempVel;
	}

	private Vector2 GetMoveVelocityWithDeltaTime()
	{
		return GetVelocity() * Time.fixedDeltaTime;
	}

	private Vector2 GetVelocity()
	{
		moveVelocity = moveDir * moveSpeed;
		return moveVelocity;
	}

	public void MoveInDirection(Vector2 direction, float newSpeed)
	{
		if (!IsActive) return;
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
		if (Game_GlobalVariables.IsPaused) return;
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

	public void StopMoving()
	{
		isMoving = false;
		moveVelocity = Vector2.zero;
	}

	public void SetRBKinematic(bool on)
	{
		rb = GetComponent<Rigidbody2D>();
		rb.isKinematic = on;
	}

	public void ActivateMovement()
	{
		IsActive = true;
	}

	public void StopPush()
	{
		pushVelocity = Vector2.zero;
	}
}