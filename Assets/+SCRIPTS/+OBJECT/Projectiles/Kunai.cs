using UnityEngine;

namespace __SCRIPTS.Projectiles
{
	public class Kunai : MonoBehaviour
	{
		private Vector2 direction;
		private float speed = 100;
		private float height;
		private Life owner;
		public GameObject rotationObject;

		private bool isAirThrow;
		private IDebree fallToFloor => _fallToFloor ??= GetComponent<IDebree>();
		private IDebree _fallToFloor;
		private IRotate rotationAbility => _rotationAbility ??= GetComponent<IRotate>();
		private IRotate _rotationAbility;
		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;
		private bool isFlying;

		public void Throw(Vector3 throwDirection, Vector3 pos, float throwHeight, Life thrower, bool _isAirThrow)
		{
			direction = throwDirection;
			transform.position = pos;
			height = throwHeight;
			owner = thrower;
			isAirThrow = _isAirThrow;
			fallToFloor.SetDistanceToGround(height);
			rotationAbility.RotateToDirection(direction, rotationObject);
			rotationAbility.SetRotationRate(0);
			isFlying = true;
		}

		protected void FixedUpdate()
		{
			if (!isFlying) return;
			var nextPos = direction.normalized * speed * Time.fixedDeltaTime + (Vector2) transform.position;

			var colliderLife = AttackUtilities.CheckForCollisions(nextPos, gameObject, owner.EnemyLayer);
			if (colliderLife != null)
				HandleHit(colliderLife);
			else
				MoveTo(nextPos);
		}

		private void MoveTo(Vector2 nextPos)
		{
			if (moveAbility != null)
			{
				moveAbility.MoveInDirection(direction.normalized, speed);
				if (height >= 0)
				{
					if (isAirThrow) height -= speed * Time.fixedDeltaTime;

					_fallToFloor.SetDistanceToGround(height);
				}
				else
					Land();
			}
			else
				transform.position += (Vector3) nextPos;
		}

		private void Land()
		{
			Debug.Log("land", this);
			rotationAbility.SetFreezeRotation(true);
			isFlying = false;
			moveAbility.StopMoving();
			Services.objectMaker.Unmake(gameObject, 3);
		}

		private void HandleHit(Life hitLife)
		{
			Debug.Log("hit", this);
			isFlying = false;
			if (hitLife == null) return;
			var attack = Attack.Create(owner, hitLife).WithDamage(owner.PrimaryAttackDamageWithExtra);
			hitLife.TakeDamage(attack);
			rotationAbility.SetRotationRate(300);
			moveAbility.StopMoving();
			Services.sfx.sounds.kunai_hit_sounds.PlayRandomAt(transform.position);

			Services.objectMaker.Make(Services.assetManager.FX.hit5_xstrike, transform.position);
			_fallToFloor.FireFlipped(attack);
			Services.objectMaker.Unmake(gameObject, 3);
		}
	}
}
