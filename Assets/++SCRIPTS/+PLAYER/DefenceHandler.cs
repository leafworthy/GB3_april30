using System;
using UnityEngine;

namespace _SCRIPTS
{
	public class DefenceHandler : MonoBehaviour
	{
		public event Action<Attack> OnDamaged;
		public event Action<float> OnFractionChanged;
		public event Action OnDying;
		public event Action OnDead;
		private event Action OnWounded;

		public float HealthMax
		{

			get { return stats.GetStat(StatType.healthMax); }
			private set { stats.SetStat(StatType.healthMax, value); }
		}

		public float Health
		{
			get
			{
				return stats.GetStat(StatType.health);
			}
			private set
			{
				stats.SetStat(StatType.health, value);
			}
		}


		private float woundHealth = .8f;
		private UnitStats stats;
		public bool isInvincible;
		private bool isDying;
		private bool isDead;
		private float currentPoisonTime;
		private AnimationEvents animationEvents;

		public bool IsWounded()
		{
			return Health / HealthMax <= woundHealth;
		}

		public float GetFraction()
		{
			return stats.GetStat(StatType.health) / HealthMax;
		}
		private void Start()
		{
			animationEvents = GetComponentInChildren<AnimationEvents>();
			animationEvents.OnDieStop += Die;
			stats = GetComponent<UnitStats>();
			Health = HealthMax;
			OnFractionChanged?.Invoke(1);
		}


		public bool TakeDamage(Attack attack)
		{
			if (!IsDeadOrDying())
			{
				stats.ChangeStat(StatType.health, -attack.DamageAmount);
				OnFractionChanged?.Invoke(Health / HealthMax);
				OnDamaged?.Invoke(attack);

				SprayBlood(15, attack.DamagePosition, attack.DamageDirection);

				if (Health <= 0)
				{
					StartDying();
					Health = 0;
					return true;
				}
				else if (IsWounded()) OnWounded?.Invoke();

				return false;
			}

			return false;
		}

		private void SprayBlood(int quantity, Vector3 getPosition, Vector3 bloodDir)
		{
			for (var j = 0; j < quantity; j++)
			{
				if (ASSETS.FX is null) return;
				var newBulletShell = MAKER.Make(ASSETS.FX.blood_debree.GetRandom(), getPosition);
				newBulletShell.GetComponent<FallToFloor>().Fire(bloodDir.normalized, 2);
				var newBulletShell2 = MAKER.Make(ASSETS.FX.blood_debree.GetRandom(), getPosition);
				newBulletShell2.GetComponent<FallToFloor>().Fire(-bloodDir.normalized, 1);
			}
		}

		private void Die()
		{
			OnDead?.Invoke();

			isDead = true;
			isDying = false;
			Debug.Log("DEAD");
			MAKER.Unmake(gameObject, 4);
		}

		private void StartDying()
		{
			OnDying?.Invoke();
			isDying = true;
			gameObject.layer = LayerMask.NameToLayer("Dead");
			foreach (var col in GetComponents<Collider2D>()) Destroy(col);
		}

		public bool IsDead()
		{
			return isDead;
		}

		public bool IsDeadOrDying()
		{
			return isDead || isDying;
		}

		public bool IsInvincible()
		{
			return isInvincible;
		}

		public float GetAimHeight()
		{
			if (stats is null) return 0;
			return stats.GetStat(StatType.aimHeight);
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public bool IsPlayer()
		{
			if (stats is null) stats = GetComponent<UnitStats>();
			return stats.isPlayer;
		}


		public bool IsDying()
		{
			return isDying;
		}

		public void AddHealth(int amount)
		{
			stats.ChangeStat(StatType.health, amount);
			OnFractionChanged?.Invoke(GetFraction());
		}
	}
}
