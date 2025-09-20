using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{

	public class Life : MonoBehaviour, INeedPlayer
	{
		public Player Player => player;
		[SerializeField] private Player player;
		[SerializeField] private UnitStats unitStats;
		[SerializeField] private UnitHealth unitHealth;
		private UnitAnimations animations => _animations ??= GetComponent<UnitAnimations>();
		private UnitAnimations _animations;
		public bool IsDead() => unitHealth.IsDead;
		public bool IsShielded => unitHealth.IsShielded;
		public float CurrentHealth => unitHealth.CurrentHealth;

		#region UnitStats Wrappers

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
		public float ExtraDamageFactor => unitStats.ExtraDamageFactor;
		public float MoveSpeed => unitStats.MoveSpeed;
		public float DashSpeed => unitStats.DashSpeed;
		public float JumpSpeed => unitStats.JumpSpeed;
		public float AggroRange => unitStats.AggroRange;
		public float AttackHeight => unitStats.AttackHeight;
		public bool IsObstacle => unitStats.IsObstacle;
		public bool IsPlayerAttackable => unitStats.IsPlayerAttackable;
		public DebrisType DebrisType => unitStats.DebrisType;
		public bool showLifeBar => unitStats.Data.showLifeBar;
		public float MaxHealth => unitStats.MaxHealth;
		public bool IsNotInvincible => !unitStats.Data.isInvincible;

		#endregion


		public bool IsHuman => Player != null && Player.IsPlayer();
		public LayerMask EnemyLayer => IsHuman ? Services.assetManager.LevelAssets.EnemyLayer : Services.assetManager.LevelAssets.PlayerLayer;
		public bool CanTakeDamage => !unitHealth.IsDead && !unitHealth.IsTemporarilyInvincible && !unitStats.Data.isInvincible;



		public event Action<Attack> OnAttackHit;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnDying;
		public event Action<Player, Life> OnKilled;
		public event Action<Player> OnDeathComplete;
		public event Action<Attack> OnShielded;
		private Collider2D[] colliders;

		[Sirenix.OdinInspector.Button]
		public void ClearStats()
		{
			unitStats = null;
		}
		[Sirenix.OdinInspector.Button]
		public void GetStats()
		{
			unitStats = new UnitStats(gameObject.name);
		}


		private void Awake()
		{
			unitStats = new UnitStats(gameObject.name);
			unitHealth = new UnitHealth(unitStats.Data);
			unitHealth.OnFractionChanged += Health_OnFractionChanged;
			unitHealth.OnDead += HealthOnDead;
			unitHealth.OnAttackHit += Health_AttackHit;
			if (unitStats.Data.category != UnitCategory.Character) SetPlayer(Services.playerManager.enemyPlayer);
		}

		private void SetupAnimationEvents()
		{
			if (animations == null) return;

			animations.animEvents.OnDieStop += CompleteDeath;
			animations.animEvents.OnInvincible += SetInvincible;
		}

		private void OnDisable()
		{
			if (unitHealth == null) return;
			unitHealth.OnFractionChanged -= Health_OnFractionChanged;
			unitHealth.OnDead -= HealthOnDead;
			unitHealth.OnAttackHit -= Health_AttackHit;
		}



		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;


			SetupAnimationEvents();
			foreach (var component in GetComponents<INeedPlayer>())
			{
				if (component != this)
					component.SetPlayer(Player);
			}
		}



		private void SetInvincible(bool inOn)
		{
			unitHealth.SetTemporaryInvincible(inOn);
		}

		private void CompleteDeath()
		{
			OnDeathComplete?.Invoke(Player);
			Services.objectMaker.Unmake(gameObject);
		}


		private void Health_AttackHit(Attack attack)
		{
			OnAttackHit?.Invoke(attack);
		}



		public bool IsEnemyOf(Life other) => IsHuman != other.IsHuman;



		private void HealthOnDead(Attack attack)
		{
			EnableColliders(false);
			OnDying?.Invoke(attack);
			if (attack.OriginLife.player != null)
			{
				OnKilled?.Invoke(attack.OriginLife.player, attack.DestinationLife);
			}
			gameObject.layer = LayerMask.NameToLayer("Dead");
			animations?.SetTrigger(UnitAnimations.DeathTrigger);
			animations?.SetBool(UnitAnimations.IsDead, true);
		}

		private void EnableColliders(bool enable)
		{
			if (!unitStats.Data.isInvincible && !enable) return;
			colliders = gameObject.GetComponentsInChildren<Collider2D>(true);

			foreach (var col in colliders)
			{
				col.enabled = enable;
			}
		}

		private void Health_OnFractionChanged(float fraction)
		{
			OnFractionChanged?.Invoke(fraction);
		}

		public void TakeDamage(Attack attack)
		{
			if (IsShielded)
			{
				OnShielded?.Invoke(attack);
				return;
			}

			unitHealth?.TakeDamage(attack);
		}

		public void AddHealth(float amount) => unitHealth.AddHealth(amount);
		public void DieNow() => unitHealth.KillInstantly();
		public float GetFraction()  {
			if(unitHealth != null)
			{
				return unitHealth.GetFraction();
			}
			return 1;
		}

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
