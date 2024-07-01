using __SCRIPTS._ABILITIES;
using __SCRIPTS._COMMON;
using __SCRIPTS._OBJECT;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._ATTACKS
{
	public class Kunai : FallToFloor
	{
		private Vector2 direction;
		private float speed = 80;
		private float height;
		private Life owner;
		public GameObject rotationObject;
		private bool isActive;
		private bool isAirThrow;

		public void Throw(Vector3 throwDirection, Vector3 pos, float throwHeight, Life thrower, bool IsAirThrow = false)
		{
			direction = throwDirection;
			transform.position = pos;
			height = throwHeight;
			owner = thrower;
			SetDistanceToGround(height);
			RotateToDirection();
			rotationRate = 0;
			isActive = true;
			mover = GetComponent<MoveAbility>();
			jumper = GetComponent<JumpAbility>();
		}

		private void RotateToDirection()
		{
			var rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			if (rotationObject == null) return;
			rotationObject.transform.eulerAngles = new Vector3(0, 0, rotation);
		}

		protected override void FixedUpdate()
		{
			if (mover == null) mover = GetComponent<MoveAbility>();
			if (jumper == null) jumper = GetComponent<JumpAbility>();
			if (!isActive)
			{
				base.FixedUpdate();
				return;
			}

			var nextPos = direction.normalized * speed * Time.fixedDeltaTime+ (Vector2)transform.position;
			var lineCast = CheckForCollisions(nextPos);
			if (lineCast.collider == null)
			{
				if (mover != null)
				{
					mover.MoveInDirection(direction.normalized, speed);
					if(height >= 0)
					{
						if (isAirThrow)
						{
							height -= speed * Time.fixedDeltaTime;
						}

						SetDistanceToGround(height);
					}
					else
					{
						Land();
					}
				}
				else
				{
					transform.position += (Vector3)nextPos;
				}
			}
			else
			{
				HandleHit(lineCast);
			}
		}

		private void Land()
		{
			rotationRate = 300;
			mover.StopMoving();
			isActive = false;
			Maker.Unmake(gameObject,3);
		}

		private void HandleHit(RaycastHit2D lineCast)
		{
			var hitLife = lineCast.collider.GetComponent<Life>();
			isActive = false;
			if (hitLife == null) return;
			var attack = new Attack(owner, hitLife, owner.Attack2Damage);
			hitLife.TakeDamage(attack);
			rotationRate = 300;
			mover.StopMoving();
			Fire(attack, true);
			Maker.Unmake(gameObject, 3);
		}

		private RaycastHit2D CheckForCollisions(Vector2 target)
		{
			var lineCast = Physics2D.Linecast(transform.position, target, ASSETS.LevelAssets.EnemyLayer);
			Debug.DrawLine(transform.position, target, lineCast.collider != null ? Color.red : Color.green);

			return lineCast;
		}
	}
}