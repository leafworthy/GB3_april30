using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float MovementSpeed;
	[SerializeField] private float RotationSpeed;
	[SerializeField] private Transform HitPoint;
	[SerializeField] private float DamageAmount;
	private bool hasReachedTarget;
	private GameObject hitObject;
	private bool isPlayer = true;
	private Vector3 velocity;
	[SerializeField] private float FloorHeight;
	[SerializeField] public GameObject HeightObject;
	[SerializeField] private float maxYDistance = 8;
	private bool hasGravity;
	private Vector3 gravity = new Vector3(0, 2.5f, 0);
	private bool hitsGround;

	private void FixedUpdate()
	{
		if (hasReachedTarget) return;
		if (hasGravity) velocity += gravity;

		if (hitsGround)
			if (IsLowerThanFloor())
				HitFloor();

		var hitPoint = CheckRaycastHit(velocity.normalized);

		if (hitPoint == null)
		{
			UpdateMovement();
		}
		else
			HitTarget(hitPoint.Value);
	}

	private void UpdateMovement()
	{
		if (hitsGround)
		{
			var newPosition = transform.position;
			newPosition.x += velocity.x * Time.fixedDeltaTime;
			transform.position = newPosition;
			var newHeightPosition = HeightObject.transform.position;
			newHeightPosition.y += velocity.y * Time.fixedDeltaTime;
			HeightObject.transform.position = newHeightPosition;
		}
		else
		{
			var newPosition = transform.position;
			newPosition += velocity * Time.fixedDeltaTime;
			transform.position = newPosition;
		}
	}

	private void HitFloor()
	{
		velocity = Vector3.zero;
		hasReachedTarget = true;
		HeightObject.transform.position = new Vector3(HeightObject.transform.position.x, FloorHeight);
	}

	private bool IsLowerThanFloor()
	{
		return HeightObject.transform.position.y <= FloorHeight;
	}

	private void HitTarget(Vector3 hitPoint)
	{
		var enemy = hitObject.GetComponent<DefenceHandler>();
		if (enemy.IsPlayer() != isPlayer)
		{
			enemy.TakeDamage(new Attack(hitPoint - velocity.normalized, hitPoint, DamageAmount));
			hasReachedTarget = true;
			velocity = Vector3.zero;
			transform.SetParent(hitObject.transform);
			Debug.Log("HIT");
			MAKER.Unmake(gameObject, 1);
		}
	}


	public void Fire(Vector3 AimDirection, bool _isPlayer, float _FloorHeight, float _ThrowHeight,
	                 bool _hitsGround = true)
	{
		var newHeightObjectPosition = HeightObject.transform.position;
		newHeightObjectPosition.y = _ThrowHeight;
		HeightObject.transform.position = newHeightObjectPosition;


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

		hitsGround = _hitsGround;
		FloorHeight = _FloorHeight;
		isPlayer = _isPlayer;
		velocity = AimDirection.normalized * MovementSpeed;
		if (velocity.x <= 0)
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	private Vector3? CheckRaycastHit(Vector3 targetDirection)
	{
		var raycastHits = Physics2D.RaycastAll(HitPoint.position, targetDirection.normalized,
			velocity.magnitude * Time.fixedDeltaTime,
			isPlayer ? ASSETS.LevelAssets.EnemyLayer : ASSETS.LevelAssets.PlayerLayer);
		foreach (var raycastHit in raycastHits)
			if (raycastHit.collider != null)
			{
				var enemy = raycastHit.collider.gameObject.GetComponent<DefenceHandler>();
				if (enemy is null) continue;

				var yDiff = Mathf.Abs(enemy.GetPosition().y - transform.position.y);
				if (yDiff < maxYDistance)
				{
					hitObject = raycastHit.collider.gameObject;
					return raycastHit.point;
				}
			}


		return null;
	}
}
