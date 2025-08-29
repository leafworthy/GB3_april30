using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class UnitHealth : ServiceUser, IPoolable, ICanAttack, INeedPlayer
	{
		public bool CanDie => !unitStats.Data.isInvincible;
		public bool IsInvincible;
		public bool IsShielded;
		public float AttackHeight => unitStats.AttackHeight;

		public Player _player;
		public Player player => _player;
		public DebrisType DebrisType => unitStats.DebrisType;

		private float currentHealth;
		private bool isDead;

		public event Action<Attack> OnDamaged;
		public event Action<Attack> OnShielded;
		public event Action<float> OnFractionChanged; // passes health fraction
		public event Action<Player,ICanAttack> OnDying;
		public event Action OnResurrected;

		private UnitStats unitStats => _unitStats ??= GetComponent<UnitStats>();
		private UnitStats _unitStats;
		private UnitAnimations unitAnim => _unitAnim ??= GetComponent<UnitAnimations>();
		private UnitAnimations _unitAnim;

		private LayerMask originalLayer;
		private Player player1;
		public float CurrentHealth => currentHealth;
		public float MaxHealth => unitStats?.MaxHealth ?? 100f;
		public float GetFraction => MaxHealth > 0 ? currentHealth / MaxHealth : 0f;
		public bool IsDead => isDead;

		private void Awake()
		{
			originalLayer = gameObject.layer;
		}



		private void SetupAnimationEvents()
		{
			if (unitAnim?.animEvents == null) return;
			unitAnim.animEvents.OnDieStop += CompleteDeath;
			unitAnim.animEvents.OnInvincible += SetInvincible;
		}

		private void ResetHealth()
		{
			currentHealth = MaxHealth;
			isDead = false;
			gameObject.layer = originalLayer;
			EnableColliders(true);
			OnFractionChanged?.Invoke(GetFraction);
		}

		public void TakeDamage(Attack attack)
		{
			if (isDead || IsInvincible || unitStats?.Data?.isInvincible == true) return;

			if (IsShielded)
			{
				OnShielded?.Invoke(attack);
				return;
			}

			// Apply damage
			currentHealth = Mathf.Max(0, currentHealth - attack.DamageAmount);
			OnDamaged?.Invoke(attack);
			OnFractionChanged?.Invoke(GetFraction);

			// Handle hit reaction
			CameraShaker.ShakeCamera(transform.position, CameraShaker.ShakeIntensityType.normal);
			unitAnim?.SetTrigger(UnitAnimations.HitTrigger);

			// Check for death
			if (currentHealth <= 0 && CanDie) StartDeath();
		}

		public void AddHealth(float amount)
		{
			if (isDead && amount > 0)
			{
				Resurrect();
				return;
			}

			currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
			OnFractionChanged?.Invoke(GetFraction);
		}

		public void Resurrect()
		{
			if (!isDead) return;

			ResetHealth();
			unitAnim?.SetBool(UnitAnimations.IsDead, false);
			OnResurrected?.Invoke();
		}

		public void KillInstantly()
		{
			if (!CanDie) return;
			currentHealth = 0;
			StartDeath();
		}

		private void StartDeath()
		{
			isDead = true;
			EnableColliders(false);
			gameObject.layer = LayerMask.NameToLayer("Dead");

			unitAnim?.SetTrigger(UnitAnimations.DeathTrigger);
			unitAnim?.SetBool(UnitAnimations.IsDead, true);
		}

		private void CompleteDeath()
		{
			if (!CanDie) return;
			OnDying?.Invoke(player, this);
			objectMaker?.Unmake(gameObject);
		}

		private void SetInvincible(bool invincible)
		{
			IsInvincible = invincible;
		}

		private void EnableColliders(bool enable)
		{
			if (!CanDie && !enable) return;

			foreach (var col in GetComponentsInChildren<Collider2D>())
			{
				col.enabled = enable;
			}
		}

		// IPoolable implementation
		public void OnPoolSpawn() => ResetHealth();
		public void OnPoolDespawn() => CleanupEvents();

		private void CleanupEvents()
		{
			if (unitAnim?.animEvents != null)
			{
				unitAnim.animEvents.OnDieStop -= CompleteDeath;
				unitAnim.animEvents.OnInvincible -= SetInvincible;
			}
		}

		private void OnDestroy() => CleanupEvents();

		public void SetPlayer(Player p)
		{
			 _player  = p;
			 ResetHealth();
			 SetupAnimationEvents();
		}
	}
}
