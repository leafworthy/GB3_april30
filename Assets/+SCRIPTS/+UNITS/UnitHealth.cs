using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class UnitHealth
	{

		public bool IsTemporarilyInvincible;
		public bool IsShielded;
		public bool IsDead => isDead;
		public float CurrentHealth => currentHealth;
		private float currentHealth;

		public event Action<Attack> OnDead;
		public event Action<Attack> OnAttackHit;
		public event Action<Attack> OnFlying;

		private bool isInvincible =>  unitStats.Data.isInvincible;

		private float maxHealth => Data.healthMax + unitStats.GetExtraHealth();

		public float GetFraction() => currentHealth / maxHealth;

		private UnitStatsData Data;


		private bool isDead;

		private UnitStats unitStats;
		public float MaxHealth  => maxHealth;

		public UnitHealth(UnitStats _unitStats)
		{
			unitStats = _unitStats;
			Data = unitStats.Data;
			FillHealth();
		}

		public void FillHealth()
		{
			currentHealth = maxHealth;
			isDead = false;

		}

		public void TakeDamage(Attack attack)
		{
			if (isDead || IsTemporarilyInvincible || isInvincible) return;
			currentHealth = Mathf.Max(0, currentHealth - attack.DamageAmount);
			if (currentHealth <= 0 && !isInvincible) StartDeath(attack);
			if (attack.CausesFlying)
			{
				Debug.Log("on attack sent flying");
				OnFlying?.Invoke(attack);
			}
			OnAttackHit?.Invoke(attack);
		}




		private void StartDeath(Attack killingAttack)
		{
			Debug.Log("death started");
			isDead = true;
			OnDead?.Invoke(killingAttack);
		}

		public void SetTemporaryInvincible(bool invincible)
		{
			IsTemporarilyInvincible = invincible;
		}

		public void AddHealth(float amount)
		{
			if (isDead) return;
			currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

		}

	}
}
