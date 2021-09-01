using System;
using UnityEngine;

public class NadeLaunchHandler:MonoBehaviour, IExplode  {
	private Vector2 velocity;
	private Vector2 start;
	private float throwTime;
	private Vector2 end;
	public GameObject jumpObject;
	public float timeLeft;
	private bool isJumping;
	private Vector3 jumpVel;
	private float rotationSpeed = 330;
	private float currentRotationSpeed = 130;
	private Player owner;

	public void Launch(Vector2 _start, Vector2 _end, Vector2 startingVelocity, float _throwTime,
	                   Player _owner)
	{
		owner = _owner;
		currentRotationSpeed = rotationSpeed;
		throwTime = _throwTime*Time.fixedDeltaTime;
		start = _start;
		jumpVel.y = throwTime * LEVELS.Gravity.y/2;
		jumpObject.transform.localPosition = new Vector3(0, 1.24f,0);
		transform.position = start;
		end = _end;
		velocity = startingVelocity;
		timeLeft = throwTime;
		isJumping = true;
	}

	private void FixedUpdate()
	{
		if (!isJumping) return;


		transform.position = (Vector2) transform.position + (Vector2) velocity ;
		jumpObject.transform.localEulerAngles += new Vector3(0,0, currentRotationSpeed * Time.fixedDeltaTime);
		currentRotationSpeed *= .99f;
		jumpObject.transform.localPosition = jumpObject.transform.localPosition+jumpVel ;
		jumpVel.y = jumpVel.y - LEVELS.Gravity.y*Time.fixedDeltaTime;
		if (jumpObject.transform.localPosition.y < 0)
		{
			jumpObject.transform.localPosition = new Vector3(0, 1, 0);
		}

		timeLeft -= Time.fixedDeltaTime;
		if (!(timeLeft <= 0)) return;
		jumpObject.transform.localPosition = new Vector3(0, 1, 0);
		isJumping = false;
		OnExplode?.Invoke(transform.position, owner);
		MAKER.Unmake(gameObject);
		return;

	}

	public event Action<Vector3, Player> OnExplode;

}
