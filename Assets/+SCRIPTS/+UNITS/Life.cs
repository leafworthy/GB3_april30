using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class Life : MonoBehaviour, IGetAttacked, IHaveAttackStats
	{
		public Player player => _player;
		[SerializeField] private Player _player;
		[SerializeField] private UnitStats unitStats;
		[SerializeField] private UnitHealth unitHealth;


		private UnitAnimations animations => _animations ??= GetComponent<UnitAnimations>();
		private UnitAnimations _animations;
		public bool IsDead() => unitHealth.IsDead;
		public bool IsShielded => false; //changed this
		public float CurrentHealth => unitHealth.CurrentHealth;

		#region UnitStats Wrappers

		public Color debrisColor => GetComponent<Life_FX>().DebreeTint;
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
		public float ExtraDamageFactor
		{
			get => unitStats.ExtraDamageFactor;
			set => unitStats.ExtraDamageFactor = value;
		}
		public float MoveSpeed => unitStats.MoveSpeed;
		public float DashSpeed => unitStats.DashSpeed;
		public float JumpSpeed => unitStats.JumpSpeed;
		public float AggroRange => unitStats.AggroRange;
		public float AttackHeight => unitStats.AttackHeight;
		public bool IsObstacle => unitStats.IsObstacle;
		public bool IsPlayerAttackable => unitStats.IsPlayerAttackable;
		public DebrisType debrisType => unitStats.DebrisType;
		public bool showLifeBar => unitStats.Data.showLifeBar;
		public float MaxHealth => unitHealth.MaxHealth + unitStats.GetExtraHealth();

		#endregion

		public IHaveAttackStats Stats => stats ??= GetComponent<IHaveAttackStats>();
		private IHaveAttackStats stats;

		public bool CanTakeDamage() => !unitHealth.IsDead && !unitHealth.IsTemporarilyInvincible && !unitStats.Data.isInvincible;
		public bool IsEnemyOf(IGetAttacked other) => player.IsHuman() != other.player.IsHuman();

		public event Action<Attack> OnAttackHit;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnDead;
		public event Action<Player, IGetAttacked> OnKilled;
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
			FillHealth();
			Debug.Log("done initializing for " + player?.playerIndex);
			if (unitStats.Data.category == UnitCategory.Enemy) SetPlayer(Services.playerManager.enemyPlayer);
			else if (unitStats.Data.category == UnitCategory.NPC) SetPlayer(Services.playerManager.NPCPlayer);
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
		}

		public void SetPlayer(Player newPlayer)
		{
			_player = newPlayer;
			Debug.Log("Life setting _player", this);
			Init();

			SetupAnimationEvents();
			foreach (var component in GetComponentsInChildren<INeedPlayer>())
			{
				Debug.Log("setplayer for: " + component);
				if (component != this)
					component.SetPlayer(_player);
			}
		}

		private void SetInvincible(bool inOn)
		{
			unitHealth.SetTemporaryInvincible(inOn);
		}

		private void CompleteDeath(bool isRespawning)
		{
			Debug.Log("complete death", this);
			OnDeathComplete?.Invoke(_player, isRespawning);
			Services.objectMaker.Unmake(gameObject);
		}





		private void Health_OnDead(Attack attack)
		{
			EnableColliders(false);
			OnDead?.Invoke(attack);
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


		public void TakeDamage(Attack attack)
		{
			if (IsShielded)
			{
				OnShielded?.Invoke(attack);
				return;
			}



			if (!attack.CausesFlying)
			{
				animations?.SetTrigger(UnitAnimations.HitTrigger);
			}
			else
			{
				Debug.Log("on offence sent flying");
				OnFlying?.Invoke(attack);
				animations?.SetTrigger(UnitAnimations.FlyingTrigger);
			}

			unitHealth?.TakeDamage(attack);
			OnAttackHit?.Invoke(attack);
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
			var killingBlow = Attack.Create(null, this).WithDamage(9999);
			unitHealth.TakeDamage(killingBlow);
			OnAttackHit?.Invoke(killingBlow);
			CompleteDeath(true);
		}

		public float GetFraction() => unitHealth.GetFraction();

		public void SetShielding(bool isOn)
		{
			if (unitHealth != null) unitHealth.IsShielded = isOn;
		}

		public bool IsEnemyOf(ICanAttack life) =>   player.IsHuman() != life.player.IsHuman();



		[Sirenix.OdinInspector.Button]
		public void SetEnemyTier(int tier)
		{
			unitStats.SetEnemyTier(tier);
			unitHealth.FillHealth();
			OnFractionChanged?.Invoke(GetFraction());
			var paletteSwapper = GetComponent<PalletteSwapper>();
			paletteSwapper?.SetPallette(tier);
		}

		public void SetTemporarilyInvincible(bool i)
		{
			if (unitHealth != null) unitHealth.IsTemporarilyInvincible = i;
		}
	}
}
