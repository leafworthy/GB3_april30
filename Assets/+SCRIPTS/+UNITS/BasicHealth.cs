using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(BasicStats))]
	public class BasicHealth : MonoBehaviour, IGetAttacked
	{
		//Takes damage, manages healthbar, fires events on death/hit
		[SerializeField] float MaxHealth;
		[SerializeField] float CurrentHealth;
		public bool IsDead() => CurrentHealth <= 0;

		public float GetFraction() => CurrentFraction;

		public bool CanTakeDamage() => !IsDead() && !IsTemporarilyInvincible && !Data.Data.isInvincible;
		bool IsTemporarilyInvincible;
		float CurrentFraction => MaxHealth != 0 ? CurrentHealth / MaxHealth : 0;
		float targetFraction;
		HealthBar HealthBar => _healthbar ??= GetComponentInChildren<HealthBar>(true);
		HealthBar _healthbar;
		IHaveData Data => data ??= GetComponent<IHaveData>();
		IHaveData data;
		List<SpriteRenderer> renderersToTint => _renderersToTint ??= GetComponentsInChildren<SpriteRenderer>(true).ToList();
		List<SpriteRenderer> _renderersToTint;

		public Player player => _player;
		Player _player;
		public DebrisType debrisType => Data.Data.debrisType;
		public Color debrisColor => DebreeTint;
		public Color DebreeTint = Color.red;
		Color materialTintColor;
		bool isShielding;
		public UnitCategory category => Data.Data.category;

		public event Action<Attack> OnDead;
		public event Action<Attack> OnAttackHit;
		public event Action<Attack> OnShielded;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnFlying;

		public void SetTemporarilyInvincible(bool i)
		{
			IsTemporarilyInvincible = i;
		}

		public void SetShielding(bool isOn)
		{
			isShielding = isOn;
		}

		public bool IsEnemyOf(ICanAttack life) => life.IsEnemyOf(this);

		public event Action<Player, bool> OnDeathComplete;

		void FixedUpdate()
		{
			AttackUtilities.FadeOutTintAlpha(ref materialTintColor, renderersToTint);
		}

		public void DieNow()
		{
			var killingBlow = Attack.Create(null, this).WithDamage(9999);
			TakeDamage(killingBlow);
			CompleteDeath(true);
		}

		void CompleteDeath(bool isRespawning)
		{
			OnDeathComplete?.Invoke(_player, isRespawning);
			Services.objectMaker.Unmake(gameObject);
		}

		[Sirenix.OdinInspector.Button]
		public void SetStats()
		{
			MaxHealth = Data.Data.healthMax;
			_healthbar = GetComponentInChildren<HealthBar>(true);
			_healthbar?.UpdateHealthBar(CurrentFraction);
		}

		void Update()
		{
			if (CurrentFraction == targetFraction) return;
			HealthBar?.UpdateHealthBar(CurrentFraction);
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
				AttackUtilities.StartTint(Color.yellow, renderersToTint);
				attack.DamageAmount *= 0.1f;
			}

			if (!CanTakeDamage()) return;

			CurrentHealth = Mathf.Max(0, CurrentHealth - attack.DamageAmount);
			if (CurrentHealth <= 0 && !Data.Data.isInvincible) StartDeath(attack);

			OnAttackHit?.Invoke(attack);
			AttackUtilities.AttackHitFX(attack, renderersToTint, Data.Data.debrisType, DebreeTint);
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
			_player = newPlayer;
		}
	}
}
