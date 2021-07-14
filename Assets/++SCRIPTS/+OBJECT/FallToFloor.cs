using UnityEngine;

public class FallToFloor : MonoBehaviour
{
	public float rotationRate = 135;


	private Vector3 velocity;
	private float minVelocity = .5f;
	private float maxVelocity = 1f;
	private float floorPoint;
	private bool hasLanded;
	public bool hasDeathTime = true;
	private SpriteRenderer sprite;
	private float rotationVeloctiy;
	private bool hasFired;

	public void Fire(Vector3 shootAngle, float ForceMultiplier = 1, float distanceToFloor = 5)
	{
		Debug.Log("shoot angle" + shootAngle);
		hasLanded = false;
		velocity = new Vector3(
			           Random.Range(minVelocity, maxVelocity) * shootAngle.x*3,
			           Random.Range(minVelocity, maxVelocity) * shootAngle.y * -3, 0f) *
		           ForceMultiplier;
		rotationRate = velocity.x * rotationRate;
		floorPoint = transform.position.y - distanceToFloor - Random.Range(0, distanceToFloor / 2)- velocity.y*ForceMultiplier;

	}

	private void Update()
	{
		if (hasLanded)
			return;
		transform.Rotate(new Vector3(0, 0, rotationRate * Time.deltaTime * 10));
		velocity -= GAME.Gravity * Time.deltaTime;
		transform.position += velocity;
		if (!(transform.position.y <= floorPoint)) return;
		hasLanded = true;
		if (hasDeathTime) MAKER.Unmake(gameObject, 3f);
	}
}
