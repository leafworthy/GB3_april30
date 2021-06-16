using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;



namespace _SCRIPTS
{
	public class ThrownProjectile : MonoBehaviour
	{
		private Vector3 gravity = new Vector3(0, 3.5f, 0);
		private Vector3 velocity;
		private float minVelocity = .2f;
		private float maxVelocity = .5f;
		private float floorPoint;
		private bool hasLanded;
		private SpriteRenderer sprite;
		private float rotationVelocity;

		[SerializeField] private float DamageAmount;
		private bool hasReachedTarget;
		private GameObject hitObject;
		private bool isPlayer = true;
		[SerializeField] private float FloorHeight;
		[SerializeField] public GameObject HeightObject;
		[SerializeField] private float maxYDistance = 8;
		private Vector3 origin;
		[SerializeField]private float bounceYFactor = 5;

		public event Action<DefenceHandler, Vector3> OnHitTarget;
		public event Action<Vector3> OnHitNothing;
		private Vector3 HitPoint;
		private float ThrowTime;
		private Vector2 shadowVelocity;
		private int bounces;
		private int maxBounces = 2;


		public void Throw(Vector3 initialVelocity, bool _isPlayer, float _FloorHeight, float _ThrowHeight, Vector3 hitPoint,
		                  float throwTime)
		{
			ThrowTime = throwTime;
			HitPoint = hitPoint;
			origin = HeightObject.transform.position;
			origin.y = _ThrowHeight;
			HeightObject.transform.position = origin;
			shadowVelocity = (hitPoint - origin) / (throwTime);
			rotationVelocity = initialVelocity.magnitude*10;
			SetRotation(initialVelocity);
			hasReachedTarget = false;
			FloorHeight = _FloorHeight;
			isPlayer = _isPlayer;
			bounces = 0;
			velocity = initialVelocity;
			if (velocity.x <= 0)
			{
				FlipLocalXScale();
			}
		}

		private void SetRotation(Vector3 AimDirection)
		{
			if (AimDirection.x < 0)
			{
				var angle = Mathf.Atan2(AimDirection.y, -AimDirection.x) * Mathf.Rad2Deg;
				HeightObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
			else
			{
				var angle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;
				HeightObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
		}

		private void FlipLocalXScale()
		{
			var localScale = transform.localScale;
			localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
			transform.localScale = localScale;
		}


		void FixedUpdate()
		{

			if (hasReachedTarget) return;
			HeightObject.transform.Rotate(new Vector3(0, 0, rotationVelocity * Time.fixedDeltaTime * 10));
			DragVelocityAndRotation();

			var raycastHit = CheckRaycastHit(velocity);

			if (raycastHit != null)
			{
				Debug.Log("hit something");
				var defense = raycastHit.Value.collider.gameObject.GetComponent<DefenceHandler>();
				if (defense is null)
				{
					Bounce(false);
				}
				else if(!defense.IsPlayer())
				{
					HitTarget(defense, raycastHit.Value.point);
				}
			}

			if ((HeightObject.transform.position.y < FloorHeight) && (velocity.y < 0))
			{
				LandOrBounceY();
			}

			ApplyVelocity();
		}

		private void DragVelocityAndRotation()
		{
			rotationVelocity = rotationVelocity * .8f;
			velocity -= gravity * Time.fixedDeltaTime;
			velocity = new Vector3(velocity.x, velocity.y, 0);
		}

		private void ApplyVelocity()
		{
			transform.position += new Vector3(shadowVelocity.x, shadowVelocity.y, 0);
			HeightObject.transform.position += new Vector3(0, velocity.y, 0);
		}



		private RaycastHit2D? CheckRaycastHit(Vector3 targetDirection)
		{
			RaycastHit2D[] results = new RaycastHit2D[] { };
			Physics2D.RaycastAll(HeightObject.transform.position, targetDirection.normalized, velocity.magnitude * Time.fixedDeltaTime, ASSETS.layers.EnemyLayer);

			foreach (var raycastHit in results)
			{
				if (raycastHit.collider == null) continue;
				var enemy = raycastHit.collider.gameObject.GetComponent<DefenceHandler>();
				if (enemy is null)
				{
					continue;
				}

				var yDiff = Mathf.Abs(enemy.GetPosition().y - transform.position.y);
				if (yDiff < maxYDistance)
				{
					return raycastHit;
				}
			}


			return null;
		}

		private void LandOrBounceY()
		{

			if (bounces > maxBounces)
			{
				Debug.Log("enough bounces");
				Land();
			}
			else
			{
				bounces++;
				Debug.Log(bounces);
				HeightObject.transform.position =
					new Vector3(HeightObject.transform.position.x, transform.position.y, 0);
				Bounce(true);
			}
		}

		private void Bounce(bool flipYVelocity)
		{
			if (flipYVelocity)
			{
				velocity = new Vector3(velocity.x, Mathf.Abs(velocity.y)*.7f, 0);
				var temp = HeightObject.transform.localPosition;
				temp.y = 0;
				HeightObject.transform.localPosition = temp;
				FloorHeight -= shadowVelocity.y*.7f;
			}
			else
			{

				velocity = new Vector3(-velocity.x * .7f, velocity.y, 0);
			}
		}

		private void HitTarget(DefenceHandler target, Vector3 hitPoint)
		{

			if (target.IsPlayer() != isPlayer)
			{
				var newAttack = new Attack(hitPoint-velocity.normalized, hitPoint, DamageAmount);
				target.TakeDamage(newAttack);
				hasReachedTarget = true;
				velocity = Vector3.zero;
				Debug.Log("HIT");
				OnHitTarget?.Invoke(target, hitPoint);
				MAKER.Unmake(gameObject);
			}
		}
		private void Land()
		{
			velocity = Vector3.zero;
			hasReachedTarget = true;
			HeightObject.transform.position = new Vector3(HeightObject.transform.position.x, FloorHeight);
			OnHitNothing?.Invoke(HeightObject.transform.position);
			MAKER.Unmake(gameObject);
		}
	}
}
