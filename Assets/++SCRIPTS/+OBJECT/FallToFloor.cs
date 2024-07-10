using UnityEngine;

[RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(JumpAbility))]
public class FallToFloor : ThingWithHeight
{
	public SpriteRenderer spriteRendererToTint;
	protected JumpAbility jumper;
	protected MoveAbility mover;
	protected float rotationRate = 100;
	private float RotationRate;
	private float PushSpeed = 25;
	private float bounceSpeed = 1;
	private float jumpSpeed = 1;

	public void Fire(Vector3 shootAngle, Color color, float height)
	{
		RotationRate = Random.Range(0, rotationRate);
		jumper = GetComponent<JumpAbility>();
		jumper.OnBounce += Jumper_OnBounce;
		mover = GetComponent<MoveAbility>();
		jumper.Jump(height, jumpSpeed, bounceSpeed);
		mover.SetDragging(false);
		mover.Push(shootAngle, Random.Range(0, PushSpeed));
		spriteRendererToTint.color = color;
	}

	private void Jumper_OnBounce()
	{
		mover.SetDragging(true);
	}

	public void Fire(Attack attack, bool isFlipped = false)
	{
		RotationRate = Random.Range(0, rotationRate);
		jumper = GetComponent<JumpAbility>();
		mover = GetComponent<MoveAbility>();
		jumper.Jump(attack.OriginHeight, Random.Range(0, bounceSpeed));
		mover.SetDragging(false);
		mover.Push(isFlipped ? attack.FlippedDirection : attack.Direction.normalized + new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, 5f)),
			Random.Range(0, PushSpeed));
	}

	protected override void FixedUpdate()
	{
		if (mover == null) mover = GetComponent<MoveAbility>();
		if (jumper == null) jumper = GetComponent<JumpAbility>();
		base.FixedUpdate();
		if (jumper.IsJumping)
			Rotate(RotationRate);
		else
			mover.SetDragging(true);
	}

	private void Rotate(float rotationSpeed)
	{
		JumpObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime * 10));
	}
}