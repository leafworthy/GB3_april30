using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class UnitHealth
	{
		public float GetFraction() => maxHealth > 0 ? currentHealth / maxHealth : 0f;

		public bool IsTemporarilyInvincible;
		public bool IsShielded;
		public bool IsDead => isDead;
		public float CurrentHealth => currentHealth;

		public event Action<Attack> OnDamaged;
		public event Action<float> OnFractionChanged;
		public event Action<Attack> OnWounded;
		public event Action<Life> OnDying;
		public event Action OnDead;
		public event Action<Life, Player> OnKilled;
		public event Action<Attack> OnAttackHit;

		private bool isInvincible;

		private float maxHealth;

		private float currentHealth;

		private bool isDead;

		private LayerMask originalLayer;
		private GameObject gameObject;
		private Collider2D[] colliders;
		private Life life;

		private UnitAnimations unitAnim => _unitAnim ??= gameObject.GetComponent<UnitAnimations>();
		private UnitAnimations _unitAnim;

		public UnitHealth(GameObject _gameObject, bool _isInvincible, float _MaxHealth)
		{
			gameObject = _gameObject;
			isInvincible = _isInvincible;
			colliders = gameObject.GetComponentsInChildren<Collider2D>(true);
			maxHealth = _MaxHealth;
			life = gameObject.GetComponent<Life>();
			Initialize();
		}

		private void ResetHealth()
		{
			currentHealth = maxHealth;
			isDead = false;
			EnableColliders(true);
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void TakeDamage(Attack attack)
		{
			if (isDead || IsTemporarilyInvincible || isInvincible) return;
			Debug.Log("taking damage inside");
			currentHealth = Mathf.Max(0, currentHealth - attack.DamageAmount);
			OnDamaged?.Invoke(attack);
			OnAttackHit?.Invoke(attack);
			OnFractionChanged?.Invoke(GetFraction());

			CameraShaker.ShakeCamera(attack.DestinationFloorPoint, CameraShaker.ShakeIntensityType.normal);
			if(unitAnim != null) unitAnim?.SetTrigger(UnitAnimations.HitTrigger);

			if (!(currentHealth <= 0) || isInvincible) return;
			OnWounded?.Invoke(attack);
			if (attack.OriginLife.Player != null) OnKilled?.Invoke(life, attack.OriginLife.Player);
			StartDeath();
		}

		public void AddHealth(float amount)
		{
			currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
			OnFractionChanged?.Invoke(GetFraction());
		}

		public void KillInstantly()
		{
			if (!isInvincible) return;
			currentHealth = 0;
			StartDeath();
		}

		private void StartDeath()
		{
			OnDying?.Invoke(life);
			isDead = true;
			EnableColliders(false);
			gameObject.layer = LayerMask.NameToLayer("Dead");

			unitAnim?.SetTrigger(UnitAnimations.DeathTrigger);
			unitAnim?.SetBool(UnitAnimations.IsDead, true);
		}

		private void CompleteDeath()
		{
			if (!isInvincible) return;
			OnDead?.Invoke();
			var objectMaker = ServiceLocator.Get<ObjectMaker>();
			objectMaker?.Unmake(gameObject);
			unitAnim.animEvents.OnDieStop -= CompleteDeath;
			unitAnim.animEvents.OnInvincible -= SetInvincible;
		}

		private void SetInvincible(bool invincible)
		{
			IsTemporarilyInvincible = invincible;
		}

		private void EnableColliders(bool enable)
		{
			if (!isInvincible && !enable) return;

			foreach (var col in colliders)
			{
				col.enabled = enable;
			}
		}


		private void SetupAnimationEvents()
		{
if(unitAnim == null ) return;

			unitAnim.animEvents.OnDieStop += CompleteDeath;
			unitAnim.animEvents.OnInvincible += SetInvincible;
		}

		private void Initialize()
		{
			SetupAnimationEvents();
			originalLayer = gameObject.layer;
			ResetHealth();
		}
	}
}
