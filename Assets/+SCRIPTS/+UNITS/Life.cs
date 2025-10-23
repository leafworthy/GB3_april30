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
		public bool IsShielded => false; //changed this
		public float CurrentHealth => unitHealth.CurrentHealth;

		#region UnitStats Wrappers

		public UnitCategory category => unitStats.Data.category;
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
		public float MaxHealth => unitHealth.MaxHealth + unitStats.GetExtraHealth();
		public bool IsNotInvincible => !unitStats.Data.isInvincible;

		#endregion

		public bool IsHuman => Player != null && Player.IsHuman();
		public LayerMask EnemyLayer => IsHuman ? Services.assetManager.LevelAssets.EnemyLayer : Services.assetManager.LevelAssets.PlayerLayer;
		public bool CanTakeDamage()
		{
			Debug.Log(!unitHealth.IsDead +"[BOY]dead"+!unitHealth.IsTemporarilyInvincible + "is temporary invis" + !unitStats.Data.isInvincible + "is invincible" );
			return !unitHealth.IsDead && !unitHealth.IsTemporarilyInvincible && !unitStats.Data.isInvincible;
		}
		public event Action<Attack> OnAttackHit;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnDying;
		public event Action<Player, Life> OnKilled;
		public event Action<Player, bool> OnDeathComplete;
		public event Action<Attack> OnShielded;
		public event Action<Attack> OnFlying;
		private Collider2D[] colliders;
		private bool hasInitialized;

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
			Init();
		}

		private void Init()
		{
			if (hasInitialized) return;
			Debug.Log("initializing", this);
			hasInitialized = true;
			unitStats = new UnitStats(gameObject.name);
			unitHealth = new UnitHealth(unitStats);
			unitHealth.OnDead += Health_OnDead;
			unitHealth.OnFlying += Health_OnFlying;
			unitHealth.OnAttackHit += Health_AttackHit;
			FillHealth();
			Debug.Log("done initializing for " + player?.playerIndex);
			if (unitStats.Data.category == UnitCategory.Enemy) SetPlayer(Services.playerManager.enemyPlayer);
			else if (unitStats.Data.category == UnitCategory.NPC) SetPlayer(Services.playerManager.NPCPlayer);
		}

		private void Health_OnFlying(Attack attack)
		{
			Debug.Log("on life flying");
			OnFlying?.Invoke(attack);
		}

		private void FillHealth()
		{
			unitHealth.FillHealth();
		}

		private void SetupAnimationEvents()
		{
			if (animations == null) return;
			if (animations.animEvents == null) return;
			animations.animEvents.OnDieStop -= CompleteDeath;
			animations.animEvents.OnDieStop += CompleteDeath;
			animations.animEvents.OnInvincible += SetInvincible;
		}

		private void OnDisable()
		{
			if (unitHealth == null) return;
			unitHealth.OnDead -= Health_OnDead;
			unitHealth.OnAttackHit -= Health_AttackHit;
			unitHealth.OnFlying -= Health_OnFlying;
		}

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			Debug.Log("Life setting player",this);
			Init();

			SetupAnimationEvents();
			foreach (var component in GetComponentsInChildren<INeedPlayer>())
			{
				Debug.Log("setplayer for: " + component);
				if (component != this)
					component.SetPlayer(Player);
			}
		}

		private void SetInvincible(bool inOn)
		{
			unitHealth.SetTemporaryInvincible(inOn);
		}

		public void CompleteDeath(bool isRespawning)
		{
			Debug.Log("complete death", this);
			OnDeathComplete?.Invoke(Player, isRespawning);
			Services.objectMaker.Unmake(gameObject);
		}

		private void Health_AttackHit(Attack attack)
		{
			OnAttackHit?.Invoke(attack);
		}

		public bool IsEnemyOf(Life other) => IsHuman != other.IsHuman;

		private void Health_OnDead(Attack attack)
		{
			EnableColliders(false);
			OnDying?.Invoke(attack);
			if (attack.OriginLife.player != null) OnKilled?.Invoke(attack.OriginLife.player, attack.DestinationLife);
			gameObject.layer = LayerMask.NameToLayer("Dead");
			Debug.Log("set here");
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
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void AddHealth(float amount)
		{
			unitHealth.AddHealth(amount);
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void DieNow()
		{
			Debug.Log("die now");
			var killingBlow = Attack.Create(this,this).WithDamage(9999);
			unitHealth.TakeDamage(killingBlow);
			 CompleteDeath(true);
		}

		public float GetFraction() => unitHealth.GetFraction();

		public void SetShielding(bool isOn)
		{
			if (unitHealth != null) unitHealth.IsShielded = false;
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

		[Sirenix.OdinInspector.Button]
		public void SetEnemyTier(int tier)
		{
			unitStats.SetEnemyTier(tier);
			unitHealth.FillHealth();
			OnFractionChanged?.Invoke(GetFraction());
			var paletteSwapper = GetComponent<PalletteSwapper>();
			paletteSwapper?.SetPallette(tier);
		}

		public void SetTemporarilyInvincible(bool isOn)
		{
			if (unitHealth != null) unitHealth.IsTemporarilyInvincible = isOn;
		}
	}
}
