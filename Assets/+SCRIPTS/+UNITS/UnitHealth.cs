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

		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnDead;
		public event Action<Attack> OnAttackHit;

		private bool isInvincible;

		private float maxHealth;

		private float currentHealth;

		private bool isDead;

		private UnitStatsData unitData;

		public UnitHealth(UnitStatsData data)
		{
			unitData = data;
			isInvincible = data.isInvincible;
			maxHealth = data.healthMax;
			Initialize();
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
			Debug.Log("taking damage inside");
			currentHealth = Mathf.Max(0, currentHealth - attack.DamageAmount);
			if (currentHealth <= 0 && !isInvincible) StartDeath(attack);
			OnFractionChanged?.Invoke(GetFraction());
			OnAttackHit?.Invoke(attack);
		}

		public void AddHealth(float amount)
		{
			currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
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
			Debug.Log("on death in unit health");
			isDead = true;
			OnDead?.Invoke(killingAttack);
		}

		public void SetTemporaryInvincible(bool invincible)
		{
			IsTemporarilyInvincible = invincible;
		}

		private void Initialize()
		{
			ResetHealth();
		}
	}
}
