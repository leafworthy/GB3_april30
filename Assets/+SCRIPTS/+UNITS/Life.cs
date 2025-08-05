using System;
using GangstaBean.Core;
using UnityEngine;
using IPoolable = GangstaBean.Core.IPoolable;

namespace __SCRIPTS
{
[RequireComponent(typeof(UnitStats), typeof(UnitHealth))]
	public class Life : ServiceUser, ICanAttack, INeedPlayer
	{
		// Core components
		private UnitStats unitStats => _unitStats ??= GetComponent<UnitStats>();
		private UnitStats _unitStats;
		private UnitHealth unitHealth => _unitHealth ??= GetComponent<UnitHealth>();
		private UnitHealth _unitHealth;


		public UnitStatsData unitData => unitStats?.Data;
		public float Health => unitHealth?.CurrentHealth ?? 0f;
		public float HealthMax => unitHealth?.MaxHealth ?? 100f;
		public bool IsDead() => unitHealth?.IsDead ?? false;
		public bool cantDie => unitHealth?.CanDie == false;

		public bool IsInvincible => unitHealth?.IsInvincible ?? false;

		public float ExtraMaxDamageFactor => unitStats?.ExtraDamageFactor ?? 0f;

		public float ExtraMaxSpeedFactor => unitStats?.ExtraSpeedFactor ?? 0f;

		public float PrimaryAttackDamageWithExtra => unitStats?.GetAttackDamage(1) ?? 0f;
		public float PrimaryAttackRange => unitStats?.GetAttackRange(1) ?? 0f;
		public float PrimaryAttackRate => unitStats?.GetAttackRate(1) ?? 0f;

		public float SecondaryAttackDamageWithExtra => unitStats?.GetAttackDamage(2) ?? 0f;
		public float SecondaryAttackRange => unitStats?.GetAttackRange(2) ?? 0f;
		public float SecondaryAttackRate => unitStats?.GetAttackRate(2) ?? 0f;

		public float TertiaryAttackDamageWithExtra => unitStats?.GetAttackDamage(3) ?? 0f;
		public float TertiaryAttackRange => unitStats?.GetAttackRange(3) ?? 0f;
		public float TertiaryAttackRate => unitStats?.GetAttackRate(3) ?? 0f;

		public float UnlimitedAttackDamageWithExtra => unitStats?.GetAttackDamage(4) ?? 0f;
		public float UnlimitedAttackRange => unitStats?.GetAttackRange(4) ?? 0f;
		public float UnlimitedAttackRate => unitStats?.GetAttackRate(4) ?? 0f;

		// Other legacy properties
		public float MoveSpeed => unitStats?.MoveSpeed ?? 0f;
		public float DashSpeed => unitStats?.DashSpeed ?? 0f;
		public float JumpSpeed => unitStats?.JumpSpeed ?? 0f;
		public float AggroRange => unitStats?.AggroRange ?? 0f;
		public float AttackHeight => unitStats?.AttackHeight ?? 5f;
		public bool IsObstacle => unitStats?.IsObstacle ?? false;
		public bool IsPlayerAttackable => unitStats?.IsPlayerAttackable ?? false;
		public Player player => _player;
		private Player _player;
		public DebrisType DebrisType => unitStats?.DebrisType ?? default;

		public event Action<Attack, Life> OnAttackHit;
		public event Action<Attack> OnDamaged;
		public event Action<float> OnFractionChanged;
		public event Action<Player, Life> OnDying;
		public event Action<Player, Life> OnKilled;
		public event Action<Player> OnDead;
		public event Action<Attack> OnWounded;


		public bool IsPlayer => player != null && player.IsPlayer();

		public void SetPlayer(Player newPlayer)
		{
			Debug.Log( "Setting player for Life component: " + newPlayer?.name);
			_player = newPlayer;

			// Notify other components that need the player
			foreach (var component in GetComponents<INeedPlayer>())
			{
				if (component != this)
					component.SetPlayer(player);
			}
		}

		public bool IsEnemyOf(Life other) => IsPlayer != other.IsPlayer;
		private void Awake()
		{
			SetupEventForwarding();
		}

		private void SetupEventForwarding()
		{
			unitHealth.OnDamaged += attack => OnDamaged?.Invoke(attack);
			unitHealth.OnFractionChanged += fraction => OnFractionChanged?.Invoke(fraction);
			unitHealth.OnDying += (p, u) => OnDead?.Invoke(p);
		}

		public void TakeDamage(Attack attack)
		{
			unitHealth?.TakeDamage(attack);

			// Fire additional legacy events
			if (!unitHealth.IsDead)
				OnAttackHit?.Invoke(attack, this);

			if (unitHealth.CurrentHealth <= 0)
			{
				OnWounded?.Invoke(attack);
				OnDying?.Invoke(attack.Owner, this);
				if (attack.Owner != null)
					OnKilled?.Invoke(attack.Owner, this);
			}
		}

		public void AddHealth(float amount) => unitHealth?.AddHealth(amount);
		public void Resurrect() => unitHealth?.Resurrect();
		public void DieNow() => unitHealth?.KillInstantly();
		public float GetFraction() => unitHealth?.GetFraction ?? 0f;

		public void SetShielding(bool isOn)
		{
			if (unitHealth != null) unitHealth.IsShielded = isOn;
		}

		// Factor methods (same math as original)
		public void SetExtraMaxHealthFactor(float factor)
		{
			if (unitStats != null) unitStats.ExtraHealthFactor = factor;
		}

		public void SetExtraMaxDamageFactor(float factor)
		{
			if (unitStats != null) unitStats.ExtraDamageFactor = factor;
		}

		public void SetExtraMaxSpeedFactor(float factor)
		{
			if (unitStats != null) unitStats.ExtraSpeedFactor = factor;
		}

	}
}
