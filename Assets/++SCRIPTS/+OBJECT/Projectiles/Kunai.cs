using UnityEngine;

namespace GangstaBean.Objects.Projectiles.Projectiles
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

		public void Throw(Vector3 throwDirection, Vector3 pos, float throwHeight, Life thrower)
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
			var colliderLife = CheckForCollisions(nextPos);
			if (colliderLife == null)
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
				HandleHit(colliderLife);
			}
		}

		private void Land()
		{
			rotationRate = 300;
			mover.StopMoving();
			isActive = false;
			ObjectMaker.I.Unmake(gameObject,3);
		}

		private void HandleHit(Life hitLife)
		{
			isActive = false;
			if (hitLife == null) return;
			var attack = new Attack(owner, hitLife, owner.PrimaryAttackDamageWithExtra);
			hitLife.TakeDamage(attack);
			rotationRate = 300;
			mover.StopMoving();
			SFX.I.sounds.kunai_hit_sounds.PlayRandomAt(transform.position);

			ObjectMaker.I.Make(ASSETS.FX.hit5_xstrike, transform.position);
			Fire(attack, true);
			ObjectMaker.I.Unmake(gameObject, 3);
		}

		private Life CheckForCollisions(Vector2 target)
		{
			var lineCast = Physics2D.LinecastAll(transform.position, target, ASSETS.LevelAssets.EnemyLayer);
			foreach (var hit2D in lineCast)
			{
				if (hit2D.collider == null) continue;
				if (hit2D.collider.gameObject == gameObject) continue;
				var life = hit2D.collider.GetComponent<Life>();
				if(life == null)
				{
					Debug.Log("hit but no life", hit2D.collider.gameObject);
			
					continue;
				}
				Debug.DrawLine( transform.position, target, Color.red);
		
				return life;
			}

			Debug.DrawLine(transform.position, target, Color.green);
		
			return null;
		}
	}
}