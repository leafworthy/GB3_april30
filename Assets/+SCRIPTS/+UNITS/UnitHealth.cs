using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class UnitHealth
	{
		public float GetFraction() => maxHealth > 0 ? currentHealth / maxHealth : 0f;

		public bool IsTemporarilyInvincible;
		public bool IsShielded;
		public bool IsDead => isDead;
		public float CurrentHealth => currentHealth;
		private float currentHealth;

		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnDead;
		public event Action<Attack> OnAttackHit;

		private bool isInvincible =>  unitStats.isInvincible;

		private float maxHealth => unitStats.MaxHealth();



		private bool isDead;

		private UnitStats unitStats;

		public UnitHealth(UnitStats _unitStats)
		{
			unitStats = _unitStats;
			ResetHealth();
		}

		private void ResetHealth()
		{
			currentHealth = maxHealth;
			isDead = false;
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void TakeDamage(Attack attack)
		{
			if (isDead || IsTemporarilyInvincible || isInvincible) return;
			currentHealth = Mathf.Max(0, currentHealth - attack.DamageAmount);
			if (currentHealth <= 0 && !isInvincible) StartDeath(attack);
			OnFractionChanged?.Invoke(GetFraction());
			OnAttackHit?.Invoke(attack);
		}

		public void SetHealth(float amount)
		{
			currentHealth = amount;
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void KillInstantly()
		{
			if (!isInvincible) return;
			currentHealth = 0;
			StartDeath(new Attack(null, null, 0));
		}

		private void StartDeath(Attack killingAttack)
		{
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
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void FillHealth()
		{
			if (isDead) return;
			currentHealth = maxHealth;
			OnFractionChanged?.Invoke(GetFraction());
		}
	}
}
