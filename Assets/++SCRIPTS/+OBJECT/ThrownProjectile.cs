using System;
using UnityEngine;

public class ThrownProjectile : MonoBehaviour, IExplode
{
	private Vector3 gravity = new Vector3(0, 3.5f, 0);
	private Vector3 velocity;
	private float rotationVelocity;

	[SerializeField] private float DamageAmount;
	private bool hasReachedTarget;
	private bool isPlayer = true;
	[SerializeField] private float FloorHeight;
	[SerializeField] public GameObject HeightObject;
	[SerializeField] private float maxYDistance = 8;
	private Vector3 origin;

	public event Action<DefenceHandler, Vector3, Player> OnHitTarget;
	public event Action<Vector3, Player> OnExplode;
	private Vector2 shadowVelocity;
	private Player owner;

	public void Throw(Vector3 initialVelocity, bool _isPlayer, float _FloorHeight, float _ThrowHeight, Vector3 hitPoint,
	                  float throwTime, Player attacker)
	{
		owner = attacker;
		origin = HeightObject.transform.position;
		origin.y = _ThrowHeight;
		HeightObject.transform.position = origin;
		shadowVelocity = (hitPoint - origin) / (throwTime);
		rotationVelocity = initialVelocity.magnitude*10;
		SetRotation(initialVelocity);
		hasReachedTarget = false;
		FloorHeight = _FloorHeight;
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
		var transform1 = transform;
		var localScale = transform1.localScale;
		localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
		transform1.localScale = localScale;
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
				ASSETS.sounds.bean_nade_bounce_sounds.PlayRandom();
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
		Physics2D.RaycastAll(HeightObject.transform.position, targetDirection.normalized, velocity.magnitude * Time.fixedDeltaTime, ASSETS.LevelAssets.EnemyLayer);

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

			Land();
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
			var newAttack = new Attack(hitPoint-velocity.normalized, hitPoint, DamageAmount, false,STUNNER.StunLength.Normal, true, owner);
			target.TakeDamage(newAttack);
			hasReachedTarget = true;
			velocity = Vector3.zero;
			Debug.Log("HIT");
			OnHitTarget?.Invoke(target, hitPoint, owner);
			MAKER.Unmake(gameObject);
		}
	}
	private void Land()
	{
		velocity = Vector3.zero;
		hasReachedTarget = true;
		HeightObject.transform.position = new Vector3(HeightObject.transform.position.x, FloorHeight);
		OnExplode?.Invoke(HeightObject.transform.position, owner);
		MAKER.Unmake(gameObject);
	}
}
