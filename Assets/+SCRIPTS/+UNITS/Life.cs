using System;
using GangstaBean.Core;
using UnityEngine;
using VInspector;
using IPoolable = GangstaBean.Core.IPoolable;

namespace __SCRIPTS
{
	public class Life : ServiceUser, INeedPlayer, IPoolable
	{
		[HideInInspector] public Player player;
		public UnitStatsData unitData => GetCachedStats();

		[SerializeField] private UnitStatsData _unitData;
		private int _cachedStatsVersion = -1; // Track cache version
		public float ExtraMaxHealthFactor;
		public float ExtraMaxSpeedFactor;
		public float ExtraMaxDamageFactor;

		public float AttackHeight => 5f; // Default value since not in CSV

		public float PrimaryAttackDamageWithExtra =>
			unitData.attack1Damage + ExtraMaxDamageFactor * unitData.attack1Damage;

		public float PrimaryAttackRange => unitData.attack1Range;
		public float PrimaryAttackRate => unitData.attack1Rate;

		public float SecondaryAttackDamageWithExtra =>
			unitData.attack2Damage + ExtraMaxDamageFactor * unitData.attack2Damage;

		public float SecondaryAttackRange => unitData.attack2Range;
		public float SecondaryAttackRate => unitData.attack2Rate;

		public float TertiaryAttackDamageWithExtra =>
			unitData.attack3Damage + ExtraMaxDamageFactor * unitData.attack3Damage;

		public float TertiaryAttackRange => unitData.attack3Range;
		public float TertiaryAttackRate => unitData.attack3Rate;

		public float UnlimitedAttackDamageWithExtra =>
			unitData.attack4Damage + ExtraMaxDamageFactor * unitData.attack4Damage;

		public float UnlimitedAttackRange => unitData.attack4Range;
		public float UnlimitedAttackRate => unitData.attack4Rate;

		public float HealthMax => unitData != null ? unitData.healthMax + ExtraMaxHealthFactor * unitData.healthMax : 100f;
		public float MoveSpeed => unitData.moveSpeed + ExtraMaxSpeedFactor * unitData.moveSpeed;
		public float DashSpeed => unitData.dashSpeed + ExtraMaxSpeedFactor * unitData.dashSpeed;

		public bool IsPlayer => IsThisAPlayer();

		public bool isEnemyOf(Life otherLife) => IsPlayer != otherLife.IsPlayer;

		private bool IsThisAPlayer() => player != null && player.IsPlayer();

		// Note: isObstacle not directly available - inferring from category
		public bool IsObstacle => unitData.category == UnitCategory.Obstacle;

		// Note: isPlayerAttackable mapped to isPlayerSwingHittable (inverted logic)
		public bool IsPlayerAttackable => unitData.isPlayerSwingHittable;

		public DebrisType DebrisType => unitData.debrisType;
		public float JumpSpeed => unitData.jumpSpeed;
		public float AggroRange => unitData.aggroRange;

		private bool playerSet;
		public void SetPlayer(Player _player)
		{
			if (playerSet) return;
			playerSet = true;
			player = _player;
			OnPlayerSet?.Invoke(player);
			var playerComponents = gameObject.GetComponents<INeedPlayer>();
			foreach (var playerComponent in playerComponents)
			{
				playerComponent?.SetPlayer(player);
			}
		}

		private UnitStatsData GetCachedStats()
		{
			if (_unitData != null) return _unitData;
			_unitData = GetStats();
			_cachedStatsVersion = unitStatsManager.CacheVersion;

			return _unitData;
		}

		[Button]
		private UnitStatsData GetStats()
		{
			var original = gameObject.name;
			_unitData = unitStatsManager.GetUnitStats(original);
			return _unitData;
		}

		public bool cantDie;
		public bool isInvincible;

		public event Action<Attack, Life> OnAttackShielded;
		public event Action<Attack, Life> OnAttackHit;
		public event Action<Attack> OnDamaged;
		public event Action<float> OnFractionChanged;
		public event Action<Player, Life> OnDying;
		public event Action<Player, Life> OnKilled;
		public event Action<Player> OnDead;
		public event Action<Player> OnResurrected;
		public event Action<Attack> OnWounded;

		private AnimationEvents animationEvents;
		private Animations anim;
		private bool isDead;
		private LayerMask originalLayer;

		[HideInInspector] public float Health;
		private Color backgroundColor;
		public bool isShielded;
		public event Action<Player> OnPlayerSet;

		public float GetFraction()
		{
			if (HealthMax <= 0) return 0;
			return Health / HealthMax;
		}

		private void Start()
		{
			// Ensure stats are loaded before initializing
			GetCachedStats();

			// Only initialize if we have valid stats or can work without them
			InitializeLife();
		}

		private void InitializeLife()
		{
			anim = GetComponent<Animations>();
			if (anim != null)
			{
				anim.animEvents.OnDieStop += Die;
				anim.animEvents.OnInvincible += SetInvincible;
			}

			originalLayer = gameObject.layer;
			ResetHealthToMax();
			isDead = false;
			isInvincible = false;
			OnFractionChanged?.Invoke(GetFraction());

			// Ensure colliders are enabled
			EnableAllColliders();
			// Reset layer if it was changed to Dead
			gameObject.layer = originalLayer;
		}

		private void SetInvincible(bool _isInvincible)
		{
			isInvincible = _isInvincible;
		}

		private void ResetHealthToMax()
		{
			var maxHealth = HealthMax;
			if (maxHealth > 0)
				Health = maxHealth;
			else
			{
				// Fallback value if HealthMax is invalid
				Health = 100f;
			}
		}

		public void TakeDamage(Attack attack)
		{
			if (IsDead()) return;
			if (attack.DestinationLife.isInvincible) return;
			// Check if this unit is invincible from the data
			if (unitData.isInvincible) return;
			if (isShielded)
			{
				// If shielded, do not take damage but still invoke the hit event
				OnAttackShielded?.Invoke(attack, this);
				return;
			}

			OnDamaged?.Invoke(attack);
			ChangeHealth(attack);

			CameraShaker.ShakeCamera(transform.position, CameraShaker.ShakeIntensityType.normal);
			if (anim != null)
			{
				anim.SetTrigger(Animations.HitTrigger);
				if (attack.IsWounding)
				{
					//OnWounded?.Invoke(attack);
					//do damaging hit
				}
			}

			OnAttackHit?.Invoke(attack, this);
			//should be different based on material

			if (!(Health <= 0)) return;
			Health = 0;
			StartDying(attack);
		}

		private void DrawX(Vector2 pos, float size, Color color)
		{
			var topLeft = new Vector2(pos.x - size, pos.y - size);
			var topRight = new Vector2(pos.x + size, pos.y - size);
			var bottomLeft = new Vector2(pos.x - size, pos.y + size);
			var bottomRight = new Vector2(pos.x + size, pos.y + size);
		}

		private void ChangeHealth(Attack attack)
		{
			Health -= attack.DamageAmount;
			OnFractionChanged?.Invoke(Health / HealthMax);
		}

		private void StartDying(Attack attack)
		{
			if (cantDie) return;

			if (isDead) return;
			OnWounded?.Invoke(attack);
			OnDying?.Invoke(attack.Owner, this);

			if (attack.Owner != null) OnKilled?.Invoke(attack.Owner, this);
			if (anim != null)
			{
				anim.SetTrigger(Animations.DeathTrigger);
				anim.SetBool(Animations.IsDead, true);
			}

			DisableCollidersLayerAndHealth();
		}

		private void DisableCollidersLayerAndHealth()
		{
			DisableAllColliders();
			gameObject.layer = LayerMask.NameToLayer("Dead");
			isDead = true;
			SetHealth(0);
		}

		private void Die()
		{
			if (cantDie) return;
			OnDead?.Invoke(player);
			objectMaker.Unmake(gameObject);
		}

		private void SetHealth(float newHealth)
		{
			Health = Mathf.Min(newHealth, HealthMax);
			OnFractionChanged?.Invoke(Health / HealthMax);
		}

		private void DisableAllColliders()
		{
			if (cantDie) return;
			var colliders = GetComponents<Collider2D>();
			foreach (var col in colliders)
			{
				col.enabled = false;
			}

			var moreColliders = GetComponentsInChildren<Collider2D>();
			foreach (var col in moreColliders)
			{
				col.enabled = false;
			}
		}

		public bool IsDead() => isDead;

		public void AddHealth(float amount)
		{
			Health += amount;
			Health = Math.Min(Health, HealthMax);
			OnFractionChanged?.Invoke(GetFraction());
			if (!isDead) return;
			isDead = false;
			gameObject.layer = originalLayer;
			ResetHealthToMax();
			OnResurrected?.Invoke(player);
		}

		public void Resurrect()
		{
			AddHealth(HealthMax);
		}

		public void DieNow()
		{
			DisableCollidersLayerAndHealth();
		}

		public void SetExtraMaxHealthFactor(float newExtraMaxHealth)
		{
			ExtraMaxHealthFactor = newExtraMaxHealth;
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void SetExtraMaxDamageFactor(float newIncreaseMaxDamage)
		{
			ExtraMaxDamageFactor = newIncreaseMaxDamage;
		}

		public void SetExtraMaxSpeedFactor(float newExtraMaxSpeed)
		{
			ExtraMaxSpeedFactor = newExtraMaxSpeed;
		}

		private void EnableAllColliders()
		{
			var colliders = GetComponents<Collider2D>();
			foreach (var col in colliders)
			{
				col.enabled = true;
			}

			var moreColliders = GetComponentsInChildren<Collider2D>();
			foreach (var col in moreColliders)
			{
				col.enabled = true;
			}
		}

		public void OnPoolSpawn()
		{
			// Reinitialize life when spawned from pool
			InitializeLife();
		}

		public void OnPoolDespawn()
		{
			// Clean up when returning to pool
			if (anim != null)
			{
				anim.animEvents.OnDieStop -= Die;
				anim.animEvents.OnInvincible -= SetInvincible;
			}
		}

		public void SetShielding(bool isOn)
		{
			isShielded = isOn;
		}
	}
}
