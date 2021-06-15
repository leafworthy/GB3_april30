using System;
using UnityEngine;

namespace _SCRIPTS
{
	[Serializable]
	public class DonutAttackHandler : MonoBehaviour, IAttackHandler
	{
		[SerializeField] private GameObject aimCenter;
		private UnitStats stats;

		private bool isOn;
		private DefenceHandler defence;
		private AnimationEvents animEvents;
		private bool isAttackRolling;
		private MovementHandler movement;
		private float timer;
		private float currentCooldown;
		private float coolDown = .2f;
		private float isCoolingDown;
		private bool isCooledDown;

		protected virtual void Start()
		{
			movement = GetComponent<MovementHandler>();
			animEvents = GetComponentInChildren<AnimationEvents>();
			animEvents.OnMoveStart += AttackStart;
			stats = GetComponent<UnitStats>();
			defence = GetComponent<DefenceHandler>();
			defence.OnDying += AttackStop;
			isOn = true;
			isCooledDown = true;
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
				Debug.Log("HIT");
				currentCooldown = coolDown;
			}
		}



		private void AttackHit(DefenceHandler other)
		{
			if (!isOn) return;
			var position = transform.position;
			var otherPosition = other.transform.position;
			other.TakeDamage(otherPosition - position, stats.attackDamage, position);
			defence.TakeDamage(position - otherPosition, stats.attackDamage, position);
			defence.GetComponent<MovementHandler>().Push(position - otherPosition, stats.attackDamage/2, position);
		}


		public bool CanAttack(Vector3 target)
		{
			if (!isOn) return false;
			var targetDistance = Vector3.Distance(GetAimCenter(), target);
			return targetDistance < stats.attackRange;
		}


		private Vector3 GetAimCenter()
		{
			return aimCenter.transform.position;
		}

		public void Disable()
		{
			isOn = false;
		}

		public event Action OnKillEnemy;
		public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	}
}
