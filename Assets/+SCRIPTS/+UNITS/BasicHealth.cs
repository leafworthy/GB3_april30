using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class BasicHealth : MonoBehaviour, IGetAttacked
	{
		//Takes damage, manages healthbar, fires events on death/hit
		public float MaxHealth { get; private set; }
		public float CurrentHealth { get; private set; }
		public bool IsDead() => CurrentHealth <= 0;
		public float GetFraction() => CurrentFraction;
		public bool CanTakeDamage() => Stats.CanTakeDamage();
		float CurrentFraction => MaxHealth != 0 ? CurrentHealth / MaxHealth : 0;
		float targetFraction;
		LineBar LineBar => _healthbar ??= GetComponentInChildren<LineBar>(true);
		LineBar _healthbar;
		public UnitStats Stats => _stats ??= GetComponent<UnitStats>();
		UnitStats _stats;
		List<SpriteRenderer> renderersToTint => _renderersToTint ??= GetComponentsInChildren<SpriteRenderer>(true).ToList();
		List<SpriteRenderer> _renderersToTint;

		public Player player { get; private set; }
		Color materialTintColor;
		bool isShielding;
		UnitStats stats;
		DebrisType debrisType;
		public DebrisType DebrisType => Stats.DebrisType;
		public UnitCategory category => Stats.Category;

		public event Action<Attack> OnDead;
		public event Action<Attack> OnAttackHit;
		public event Action<Attack> OnShielded;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnFlying;

		public void SetTemporarilyInvincible(bool i)
		{
		}

		public void SetShielding(bool isOn)
		{
			isShielding = isOn;
		}

		public bool IsEnemyOf(ICanAttack life) => life.IsEnemyOf(this);

		public event Action<Player, bool> OnDeathComplete;

		void FixedUpdate()
		{
			MyAttackUtilities.FadeOutTintAlpha(ref materialTintColor, renderersToTint);
		}

		public void DieNow()
		{
			var killingBlow = Attack.Create(null, this).WithDamage(9999);
			TakeDamage(killingBlow);
			CompleteDeath(true);
		}

		void CompleteDeath(bool isRespawning)
		{
			OnDeathComplete?.Invoke(player, isRespawning);
			Services.objectMaker.Unmake(gameObject);
		}

		[Sirenix.OdinInspector.Button]
		public void SetStats()
		{
			MaxHealth = Stats.RealMaxHealth;
			_healthbar = GetComponentInChildren<LineBar>(true);
			_healthbar?.UpdateBar(CurrentFraction);
		}

		void Update()
		{
			if (CurrentFraction == targetFraction) return;
			LineBar?.UpdateBar(CurrentFraction);
			targetFraction = CurrentFraction;
		}

		[Sirenix.OdinInspector.Button]
		public void FillHealth()
		{
			CurrentHealth = MaxHealth;
		}

		public void TakeDamage(Attack attack)
		{
			if (isShielding)
			{
				OnShielded?.Invoke(attack);
				MyAttackUtilities.StartTint(Color.yellow, renderersToTint);
				attack.DamageAmount *= 0.1f;
			}

			if (!CanTakeDamage()) return;

			CurrentHealth = Mathf.Max(0, CurrentHealth - attack.DamageAmount);
			OnFractionChanged?.Invoke(CurrentFraction);
			if (CurrentHealth <= 0 && !Stats.IsInvincible) StartDeath(attack);

			OnAttackHit?.Invoke(attack);
			MyAttackUtilities.AttackHitFX(attack, renderersToTint, Stats.DebrisType, Stats.DebrisTint);
			if (!attack.CausesFlying) return;
			OnFlying?.Invoke(attack);
		}

		public void AddHealth(float amount)
		{
			if (IsDead()) return;
			CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
		}

		public void AddHealth(int amount)
		{
			if (IsDead()) return;
			CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
		}

		void StartDeath(Attack killingAttack)
		{
			CurrentHealth = 0;
			OnDead?.Invoke(killingAttack);
		}

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
		}

	}
}
