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

		[SerializeField] private Transform HitPoint;
		[SerializeField] private float DamageAmount;
		private bool hasReachedTarget;
		private GameObject hitObject;
		private bool isPlayer = true;
		[SerializeField] private float FloorHeight;
		[SerializeField] public GameObject HeightObject;
		[SerializeField] private float maxYDistance = 8;
		private Vector2 origin;

		public event Action<DefenceHandler, Vector3> OnHitTarget;
		public event Action<Vector3> OnHitNothing;





		public void Throw(Vector3 initialVelocity, bool _isPlayer, float _FloorHeight, float _ThrowHeight)
		{
			origin = transform.position;
			var newHeightObjectPosition = HeightObject.transform.position;
			newHeightObjectPosition.y = _ThrowHeight;
			HeightObject.transform.position = newHeightObjectPosition;

			rotationVelocity = initialVelocity.magnitude*10;
			SetRotation(initialVelocity);

			FloorHeight = _FloorHeight;
			Debug.Log(FloorHeight + "floorheight");
			Debug.Log(_ThrowHeight + "throwHeight");
			isPlayer = _isPlayer;
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
				else
				{
					HitTarget(defense, raycastHit.Value.point);
				}
			}

			if (HeightObject.transform.position.y < transform.position.y)
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
			transform.position += new Vector3(velocity.x, 0, 0);
			HeightObject.transform.position += new Vector3(0, velocity.y, 0);
		}



		private RaycastHit2D? CheckRaycastHit(Vector3 targetDirection)
		{
			var raycastHits = Physics2D.RaycastAll(HitPoint.position, targetDirection.normalized,
				velocity.magnitude * Time.fixedDeltaTime,
				isPlayer ? ASSETS.layers.EnemyLayer : ASSETS.layers.PlayerLayer);
			foreach (var raycastHit in raycastHits)
			{
				if (raycastHit.collider != null)
				{
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
			}


			return null;
		}

		private void LandOrBounceY()
		{
			HeightObject.transform.position =
				new Vector3(HeightObject.transform.position.x, transform.position.y, 0);
			if (velocity.magnitude <= (gravity.y * Time.deltaTime * 15))
			{
				Land();
			}
			else
			{
				Bounce(true);
			}
		}

		private void Bounce(bool flipYVelocity)
		{
			if (flipYVelocity)
			{
				velocity = new Vector3(velocity.x, -velocity.y*.7f, 0);
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
				target.TakeDamage(velocity.normalized, DamageAmount, hitPoint);
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
		}
	}
}
