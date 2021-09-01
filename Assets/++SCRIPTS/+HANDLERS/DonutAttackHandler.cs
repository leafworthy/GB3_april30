using System;
using UnityEngine;

[Serializable]
public class DonutAttackHandler : MonoBehaviour, IAttackHandler
{
	[SerializeField] private GameObject aimCenter;
	private UnitStats stats;

	private bool isOn;
	private DefenceHandler defence;
	private AnimationEvents animEvents;
	private bool isAttackRolling;
	private float currentCooldown;
	private float coolDown = .25f;
	private bool isCooledDown;
	private PushHandler pushHandler;

	protected virtual void Start()
	{
		pushHandler = GetComponent<PushHandler>();
		GetComponent<MovementHandler>();
		animEvents = GetComponentInChildren<AnimationEvents>();
		animEvents.OnMoveStart += AttackStart;
		stats = GetComponent<UnitStats>();
		defence = GetComponent<DefenceHandler>();
		defence.OnDamaged += Damaged;
		defence.OnDying += AttackStop;
		isOn = true;
		isCooledDown = true;
	}

	private void Damaged(Attack obj)
	{
		pushHandler.Push(obj.DamageDirection,obj.DamageAmount*.1f);
	}


	private void FixedUpdate()
	{
		if (currentCooldown > 0)
		{
			isCooledDown = false;
			currentCooldown -= Time.fixedDeltaTime;
			if (currentCooldown <= 0)
			{
				currentCooldown = 0;
				isCooledDown = true;
			}
		}
	}

	private void AttackStop()
	{
		isAttackRolling = false;
	}

	private void AttackStart()
	{
		isAttackRolling = true;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!isAttackRolling) return;
		if(!isCooledDown) return;
		CheckForHit(other);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (!isAttackRolling) return;
		if (!isCooledDown) return;
		CheckForHit(other);
	}

	private void CheckForHit(Collider2D other)
	{
		if (other.transform == transform) return;
		var otherDefence = other.GetComponent<DefenceHandler>();
		if (otherDefence is null) return;
		if (otherDefence.IsPlayer())
		{
			AttackHit(otherDefence);
			currentCooldown = coolDown;
		}
	}



	private void AttackHit(DefenceHandler other)
	{
		if (!isOn) return;
		var position = transform.position;
		var otherPosition = other.transform.position;
		var otherAttack = new Attack(position,otherPosition, stats.GetStatValue(StatType.attackDamage));
		var bouncebackAttack = new Attack(otherPosition, position, stats.GetStatValue(StatType.attackDamage));
		other.TakeDamage(otherAttack);
		defence.TakeDamage(bouncebackAttack);
		pushHandler.Push(bouncebackAttack.DamageDirection, bouncebackAttack.DamageAmount * .3f);
	}



	public bool CanAttack(Vector3 target)
	{
		if (!isOn) return false;
		var targetDistance = Vector3.Distance(GetAimCenter(), target);
		return targetDistance < stats.GetStatValue(StatType.attackRange);
	}


	private Vector3 GetAimCenter()
	{
		return aimCenter.transform.position;
	}

	public void Disable()
	{
		isOn = false;
	}
}
