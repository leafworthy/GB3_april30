using UnityEngine;

namespace __SCRIPTS.Projectiles
{
	public class Nade:MonoBehaviour  {
		private Vector2 velocity;
		public GameObject jumpObject;
		public float timeLeft;
		private bool hasLaunched;
		private Vector3 jumpVel;
		private float rotationSpeed = 330;
		private float currentRotationSpeed = 130;
		private Player owner;
		private const float explosionRadius = 30;
		private const float explosionDamage = 30;


		public void Launch(Vector2 _start, Vector2 startingVelocity, float _throwTime, Player _owner)
		{
			owner = _owner;
			currentRotationSpeed = rotationSpeed;
			timeLeft = _throwTime*Time.fixedDeltaTime;
			jumpVel.y = timeLeft * ASSETS.Vars.Gravity.y/2;
			jumpObject.transform.localPosition = new Vector3(0, 1.24f,0);
			transform.position = _start;
			velocity = startingVelocity;
			hasLaunched = true;
		}

		private void FixedUpdate()
		{
			if (!hasLaunched) return;
			Move();
			timeLeft -= Time.fixedDeltaTime;
			if (!(timeLeft <= 0)) return;
			Explosion_FX.Explode(transform.position, explosionRadius, explosionDamage,owner);
			ObjectMaker.I.Unmake(transform.gameObject);
		}

		private void Move()
		{
			transform.position = (Vector2)transform.position + (Vector2)velocity;
			jumpObject.transform.localEulerAngles += new Vector3(0, 0, currentRotationSpeed * Time.fixedDeltaTime);
			currentRotationSpeed *= .99f;
			jumpObject.transform.localPosition = jumpObject.transform.localPosition + jumpVel;
			jumpVel.y -= ASSETS.Vars.Gravity.y * Time.fixedDeltaTime;
			if (jumpObject.transform.localPosition.y < 0)
			{
				jumpObject.transform.localPosition = new Vector3(0, 1, 0);
			}
		}

	
	}
}