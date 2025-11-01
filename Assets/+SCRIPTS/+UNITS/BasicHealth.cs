using System;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(BasicStats))]
	public class BasicHealth : MonoBehaviour, IGetAttacked
	{
		//Takes damage, manages healthbar, fires events on death/hit
		[SerializeField] private float MaxHealth;
		[SerializeField] private float CurrentHealth;
		public bool IsDead() => CurrentHealth <= 0;
		private float CurrentFraction => MaxHealth != 0 ? CurrentHealth / MaxHealth : 0;
		private HealthBar HealthBar => _healthbar ??= GetComponentInChildren<HealthBar>(true);
		[SerializeField]private HealthBar _healthbar;
		private BasicStats Stats => _stats ??= GetComponent<BasicStats>();
		private BasicStats _stats;
		private float currentFraction;

		public Player player => _player;
		private Player _player;
		public DebrisType debrisType => Stats.DebrisType;
		public UnitCategory category => Stats.category;
		public event Action<Attack> OnDead;
		public event Action<Attack> OnAttackHit;
		public event Action<Attack> OnShielded;
		private event Action<Attack> OnFlying;

		private bool IsTemporarilyInvincible;
		private UnitCategory category1;

		public bool CanTakeDamage() => !IsDead() && !IsTemporarilyInvincible && !Stats.Data.isInvincible;


		public event Action<Player, bool> OnDeathComplete;
		public void DieNow()
		{
		}

		[Sirenix.OdinInspector.Button]
		public void SetStats()
		{
			MaxHealth = Stats.Data.healthMax;
			_healthbar = GetComponentInChildren<HealthBar>(true);
			_healthbar.UpdateHealthBar(CurrentFraction);
		}

		private void Update()
		{
			if (CurrentFraction == currentFraction) return;
			HealthBar?.UpdateHealthBar(CurrentFraction);
			currentFraction = CurrentFraction;
		}

		[Sirenix.OdinInspector.Button]
		public void FillHealth()
		{
			CurrentHealth = MaxHealth;
		}

		public void TakeDamage(Attack attack)
		{
			if (IsDead() || IsTemporarilyInvincible || _stats.isInvincible) return;
			CurrentHealth = Mathf.Max(0, CurrentHealth - attack.DamageAmount);
			if (CurrentHealth <= 0 && !_stats.isInvincible) StartDeath(attack);
			if (attack.CausesFlying)
			{
				Debug.Log("on attack sent flying");
				OnFlying?.Invoke(attack);
			}

			OnAttackHit?.Invoke(attack);
		}

		public void AddHealth(float amount)
		{
			if (IsDead()) return;
			CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

		}

		private void StartDeath(Attack killingAttack)
		{
			CurrentHealth = 0;
			OnDead?.Invoke(killingAttack);
		}

		public void SetPlayer(Player _player)
		{
			this._player   = _player;
		}
	}
}
