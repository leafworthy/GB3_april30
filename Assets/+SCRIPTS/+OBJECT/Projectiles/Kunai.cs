using UnityEngine;

namespace __SCRIPTS.Projectiles
{
	public class Kunai : FallToFloor
	{
		private Vector2 direction;
		private float speed = 100;
		private float height;
		private Life owner;
		public GameObject rotationObject;
		private bool isActive;
		private bool isAirThrow;

		public void Throw(Vector3 throwDirection, Vector3 pos, float throwHeight, Life thrower, bool _isAirThrow)
		{
			direction = throwDirection;
			transform.position = pos;
			height = throwHeight;
			Debug.Log("throw height: " + height, this);
			owner = thrower;
			isAirThrow = _isAirThrow;
			SetDistanceToGround(height);
			RotateToDirection();
			rotationRate = 0;
			isActive = true;
		}

		private void RotateToDirection()
		{
			var rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			if (rotationObject == null) return;
			rotationObject.transform.eulerAngles = new Vector3(0, 0, rotation);
		}

		protected override void FixedUpdate()
		{
			if (!isActive)
			{
				Debug.Log("not active");
				base.FixedUpdate();
				return;
			}

			var nextPos = direction.normalized * speed * Time.fixedDeltaTime + (Vector2) transform.position;

			var colliderLife = AttackUtilities.CheckForCollisions(nextPos, gameObject, owner.EnemyLayer);
			if (colliderLife != null)
			{
				HandleHit(colliderLife);
			}
			else
			{
				if (moveAbility != null)
				{
					moveAbility.MoveInDirection(direction.normalized, speed);
					if (height >= 0)
					{
						if (isAirThrow) height -= speed * Time.fixedDeltaTime;

						SetDistanceToGround(height);
					}
					else
						Land();
				}
				else
					transform.position += (Vector3) nextPos;
			}
		}

		private void Land()
		{
			Debug.Log("land",this);
			rotationRate = 300;
			moveAbility.StopMoving();
			isActive = false;
			Services.objectMaker.Unmake(gameObject, 3);
		}

		private void HandleHit(Life hitLife)
		{
			Debug.Log("hit",this);
			isActive = false;
			if (hitLife == null) return;
			var attack = new Attack(owner, hitLife, owner.PrimaryAttackDamageWithExtra);
			hitLife.TakeDamage(attack);
			rotationRate = 300;
			moveAbility.StopMoving();
			Services.sfx.sounds.kunai_hit_sounds.PlayRandomAt(transform.position);

			Services.objectMaker.Make(Services.assetManager.FX.hit5_xstrike, transform.position);
			Fire(attack, true);
			Services.objectMaker.Unmake(gameObject, 3);
		}


	}
}
