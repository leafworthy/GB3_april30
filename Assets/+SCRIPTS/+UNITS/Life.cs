using System;
using GangstaBean.Core;
using Sirenix.Serialization;
using UnityEngine;
using VInspector;

namespace __SCRIPTS
{
	public class Life : ServiceUser, ICanAttack, INeedPlayer
	{


		public Player Player => player;
		[SerializeField] private Player player;
		[SerializeField] private UnitStats unitStats;
		public UnitHealth unitHealth;
		private UnitAnimations animations => _animations ??= GetComponent<UnitAnimations>();
		private UnitAnimations _animations;
		private int enemyLayer;

		public float Health => unitHealth.CurrentHealth;
		public bool IsDead() => unitHealth.IsDead;
		public UnitStatsData UnitData => unitStats.Data;
		public float MaxHealth => unitStats.MaxHealth;
		public bool CanBeAttacked => !unitStats.Data.isInvincible;
		public bool IsShielded => unitHealth.IsShielded;
		public bool CanTakeDamage => !unitHealth.IsTemporarilyInvincible && !unitStats.Data.isInvincible;
		public float ExtraMaxDamageFactor => unitStats.ExtraDamageFactor;

		public float ExtraMaxSpeedFactor => unitStats.ExtraSpeedFactor;
		public float PrimaryAttackDamageWithExtra => unitStats.GetAttackDamage(1);
		public float PrimaryAttackRange => unitStats.GetAttackRange(1);
		public float PrimaryAttackRate => unitStats.GetAttackRate(1);
		public float SecondaryAttackDamageWithExtra => unitStats.GetAttackDamage(2);
		public float SecondaryAttackRange => unitStats.GetAttackRange(2);
		public float SecondaryAttackRate => unitStats.GetAttackRate(2);
		public float TertiaryAttackDamageWithExtra => unitStats.GetAttackDamage(3);
		public float TertiaryAttackRange => unitStats.GetAttackRange(3);
		public float TertiaryAttackRate => unitStats.GetAttackRate(3);

		public float UnlimitedAttackDamageWithExtra => unitStats.GetAttackDamage(4);
		public float UnlimitedAttackRange => unitStats.GetAttackRange(4);
		public float UnlimitedAttackRate => unitStats.GetAttackRate(4);
		public float MoveSpeed => unitStats.MoveSpeed;
		public float DashSpeed => unitStats.DashSpeed;
		public float JumpSpeed => unitStats.JumpSpeed;
		public float AggroRange => unitStats.AggroRange;
		public float AttackHeight => unitStats.AttackHeight;
		public bool IsObstacle => unitStats.IsObstacle;
		public bool IsPlayerAttackable => unitStats.IsPlayerAttackable;
		public DebrisType DebrisType => unitStats.DebrisType;

		public event Action<Attack, Life> OnAttackHit;
		public event Action<Attack> OnDamaged;
		public event Action<float> OnFractionChanged;
		public event Action<Player, Life> OnDying;
		public event Action<Player, Life> OnKilled;
		public event Action<Player> OnDead;
		public event Action<Attack> OnWounded;
		public event Action<Attack> OnShielded;


		[Button]
		public void GetStats()
		{
			unitStats = new UnitStats(gameObject.name);
		}

		[Sirenix.OdinInspector.Button]
		public void Test()
		{
			var assets = ServiceLocator.Get<ASSETS>();
			Debug.Log(assets.LevelAssets.EnemyLayer.ToString());
		}
		private void Start()
		{
			unitStats = new UnitStats(gameObject.name);
			unitHealth = new UnitHealth(gameObject, unitStats.Data.isInvincible, unitStats.Data.healthMax, animations);
			unitHealth.OnDamaged += Health_OnDamaged;
			unitHealth.OnFractionChanged += Health_OnFractionChanged;
			unitHealth.OnDead += Health_OnDead;
			unitHealth.OnKilled += Health_OnKilled;
			unitHealth.OnDying += Health_OnDying;
			unitHealth.OnAttackHit += Health_AttackHit;
			unitHealth.OnWounded += Health_OnWounded;
			if (unitStats.Data.category != UnitCategory.Character) SetPlayer(playerManager.enemyPlayer);
		}

		private void OnDisable()
		{
			if (unitHealth == null) return;
			unitHealth.OnDamaged -= Health_OnDamaged;
			unitHealth.OnFractionChanged -= Health_OnFractionChanged;
			unitHealth.OnDead -= Health_OnDead;
			unitHealth.OnDying -= Health_OnDying;
			unitHealth.OnKilled -= Health_OnKilled;
			unitHealth.OnAttackHit -= Health_AttackHit;
			unitHealth.OnWounded -= Health_OnWounded;
			unitHealth.Cleanup();
		}

		public bool IsHuman => Player != null && Player.IsPlayer();
		public LayerMask EnemyLayer  => IsHuman ? assetManager.LevelAssets.EnemyLayer : assetManager.LevelAssets.PlayerLayer;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;



			foreach (var component in GetComponents<INeedPlayer>())
			{
				if (component != this)
					component.SetPlayer(Player);
			}
		}

		private void Health_OnWounded(Attack obj)
		{
			OnWounded?.Invoke(obj);
		}

		private void Health_AttackHit(Attack attack)
		{
			OnAttackHit?.Invoke(attack, this);
		}

		private void Health_OnDead()
		{
			OnDead?.Invoke(Player);
		}

		public bool IsEnemyOf(Life other) => IsHuman != other.IsHuman;



		private void Health_OnDying()
		{
			OnDying?.Invoke(Player, this);
		}



		private void Health_OnKilled(Player killer)
		{
			OnKilled?.Invoke(killer, this);
		}

		private void Health_OnFractionChanged(float fraction)
		{
			OnFractionChanged?.Invoke(fraction);
		}

		private void Health_OnDamaged(Attack attack)
		{
			OnDamaged?.Invoke(attack);
		}

		public void TakeDamage(Attack attack)
		{
			Debug.Log("taking damage", this);
			if (IsShielded)
			{
				OnShielded?.Invoke(attack);
				return;
			}

			unitHealth?.TakeDamage(attack);
		}

		public void AddHealth(float amount) => unitHealth.AddHealth(amount);
		public void DieNow() => unitHealth.KillInstantly();
		public float GetFraction() => unitHealth.GetFraction;

		public void SetShielding(bool isOn)
		{
			if (unitHealth != null) unitHealth.IsShielded = isOn;
		}

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
