using System;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityUtils;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(Life_FX))]
	public class Life : SerializedMonoBehaviour, IGetAttacked
	{
		public string OverrideName;
		public Player player { get; private set; }
		[OdinSerialize] public UnitStats Stats { get; private set; }
		UnitAnimations animations => _animations ??= GetComponent<UnitAnimations>();
		UnitAnimations _animations;
		public bool IsDead() => Stats.IsDead;
		public bool IsShielded;
		public bool IsInvincible;
		public float CurrentHealth => Stats.CurrentHealth;
		public UnitCategory category => Stats.Data.category;
		public DebrisType DebrisType => Stats.DebrisType;
		public float MaxHealth => Stats.RealMaxHealth + Stats.GetExtraHealth();
		public bool CanTakeDamage() => Stats.CanTakeDamage();
		public bool IsEnemyOf(IGetAttacked other) => player.IsHuman() != other.player.IsHuman();

		public event Action<Attack> OnAttackHit;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnDead;
		public event Action<Player, IGetAttacked> OnKilled;
		public event Action<Player, bool> OnDeathComplete;
		public event Action<Attack> OnShielded;
		public event Action<Attack> OnFlying;
		Collider2D[] colliders;
		bool hasInitialized;
		float deathTime = 3;

		[Button]
		public void ClearStats()
		{
			Stats = null;
		}

		[Button]
		public UnitStats GetStats()
		{
			Stats = new UnitStats(OverrideName.IsBlank() ? gameObject.name : OverrideName);
			return Stats;
		}

		void Start()
		{
			Init();
		}

		void Init()
		{
			if (hasInitialized) return;
			hasInitialized = true;
			Stats = GetStats();
			Stats.OnDead += Health_OnDead;
			Stats.FillHealth();
			if (Stats.Data.category == UnitCategory.Enemy) SetPlayer(Services.playerManager.enemyPlayer);
			if (Stats.Data.category == UnitCategory.NPC) SetPlayer(Services.playerManager.NPCPlayer);
		}

		void SetupAnimationEvents()
		{
			if (animations == null) return;
			if (animations.animEvents == null) return;
			animations.animEvents.OnInvincible += SetInvincible;
		}

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			Init();

			SetupAnimationEvents();
			foreach (var component in GetComponentsInChildren<INeedPlayer>())
			{
				if (!ReferenceEquals(component, this)) // was !=
					component.SetPlayer(player);
			}
		}

		void SetInvincible(bool inOn)
		{
			Stats.IsTemporarilyInvincible = inOn;
		}

		void CompleteDeath(bool isRespawning)
		{
			Debug.Log("death complete", this);
			OnDeathComplete?.Invoke(player, isRespawning);
			Services.objectMaker.Unmake(gameObject);
		}

		void CompleteDeathDelayed()
		{
			CompleteDeath(false); // or just CompleteDeath() since false is default
		}

		void Health_OnDead(Attack attack)
		{
			Debug.Log("health on dead");
			EnableColliders(false);
			OnDead?.Invoke(attack);
			if (attack.OriginLife.player != null) OnKilled?.Invoke(attack.OriginLife.player, attack.DestinationLife);
			gameObject.layer = LayerMask.NameToLayer("Dead");
			Invoke(nameof(CompleteDeathDelayed), deathTime);

			animations?.SetTrigger(UnitAnimations.DeathTrigger);
			animations?.SetBool(UnitAnimations.IsDead, true);
		}

		void EnableColliders(bool enable)
		{
			if (Stats.Data.isInvincible && !enable) return;
			colliders = gameObject.GetComponentsInChildren<Collider2D>(true);

			foreach (var col in colliders)
			{
				col.enabled = enable;
			}
		}

		public void TakeDamage(Attack attack)
		{
			if (IsShielded)
			{
				OnShielded?.Invoke(attack);
				return;
			}

			if (IsInvincible) return;

			Stats?.TakeDamage(attack);
			OnAttackHit?.Invoke(attack);
			OnFractionChanged?.Invoke(GetFraction());
			if (!attack.CausesFlying)
				animations?.SetTrigger(UnitAnimations.HitTrigger);
			else
			{
				OnFlying?.Invoke(attack);
				animations?.SetTrigger(UnitAnimations.FlyingTrigger);
			}
		}

		public void AddHealth(float amount)
		{
			Stats.AddHealth(amount);
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void DieNow()
		{
			var killingBlow = Attack.Create(null, this).WithDamage(9999);
			Stats.TakeDamage(killingBlow);
			OnAttackHit?.Invoke(killingBlow);
			CompleteDeath(true);
		}

		public float GetFraction() => Stats.FractionWithExtraHealth;

		public void SetShielding(bool isOn)
		{
			if (Stats != null) Stats.IsShielded = isOn;
		}

		public bool IsEnemyOf(ICanAttack life) => player.IsHuman() != life.player.IsHuman();

		[Button]
		public void SetEnemyTypeAndTier(EnemySpawner.EnemyType enemyType, int tier)
		{
			Stats.EnemyTier = tier;
			Stats.FillHealth();
			OnFractionChanged?.Invoke(GetFraction());
			var paletteSwapper = GetComponent<PalletteSwapper>();
			paletteSwapper?.SetPallette(enemyType, tier);
		}

		public void SetTemporarilyInvincible(bool i)
		{
			if (Stats != null) Stats.IsTemporarilyInvincible = i;
		}
	}
}
