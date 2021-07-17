using System;
using UnityEngine;

public class DefenceHandler : MonoBehaviour
{
	public event Action<Attack> OnDamaged;
	public event Action<Attack> OnWounded;
	public event Action<float> OnFractionChanged;
	public event Action OnDying;
	public event Action<IPlayerAttackHandler> OnKilled;
	public event Action OnDead;

	public float HealthMax
	{
		get
		{
			stats ??= GetComponent<UnitStats>();
			return stats.GetStat(StatType.health).maxValue;
		}
		private set
		{
			stats ??= GetComponent<UnitStats>();
			stats.GetStat(StatType.health).maxValue = value;
		}
	}

	public float Health
	{
		get
		{
			stats ??= GetComponent<UnitStats>();
			return stats.GetStatValue(StatType.health);
		}
		private set
		{
			stats ??= GetComponent<UnitStats>();
			stats.SetStat(StatType.health, value);
		}
	}

	private UnitStats stats;
	public bool isInvincible;
	private bool isDying;
	private bool isDead;
	private AnimationEvents animationEvents;
	private float woundingAttackDamage = 25;


	private float GetFraction()
	{
		return Health / HealthMax;
	}

	private void Start()
	{
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnDieStop += Die;
		animationEvents.OnInvincible += OnInvincibleChange;
		stats = GetComponent<UnitStats>();
		Health = HealthMax;
		OnFractionChanged?.Invoke(1);
	}

	private void OnInvincibleChange(bool turnInvincible)
	{
		isInvincible = turnInvincible;
	}

	public bool TakeDamage(Attack attack)
	{
		if (isInvincible) return false;
		if (IsDeadOrDying()) return false;
		stats.ChangeStat(StatType.health, -attack.DamageAmount);
		OnFractionChanged?.Invoke(Health / HealthMax);
		OnDamaged?.Invoke(attack);

		if (attack.DamageAmount >= woundingAttackDamage)
		{
			OnWounded?.Invoke(attack);
		}
		ASSETS.sounds.bloodSounds.PlayRandom();
		SprayBlood(15, attack.DamagePosition, attack.DamageDirection);

		if (!(Health <= 0)) return false;
		if (attack.Owner != null)
		{
			OnKilled?.Invoke(attack.Owner);
		}
		StartDying();
		Health = 0;
		return true;

	}


	private void SprayBlood(int quantity, Vector3 getPosition, Vector3 bloodDir)
	{
		for (var j = 0; j < quantity; j++)
		{
			if (ASSETS.FX is null) return;
			var newBulletShell = MAKER.Make(ASSETS.FX.blood_debree.GetRandom(), getPosition);
			newBulletShell.GetComponent<FallToFloor>().Fire(bloodDir.normalized, 2);
			var newBulletShell2 = MAKER.Make(ASSETS.FX.blood_debree.GetRandom(), getPosition);
			newBulletShell2.GetComponent<FallToFloor>().Fire(-bloodDir.normalized, 1);
		}
	}

	private void Die()
	{
		if (isDead) return;
		OnDead?.Invoke();

		isDead = true;
		isDying = false;
		Debug.Log("DEAD");
		MAKER.Unmake(gameObject, 4);
	}

	private void StartDying()
	{
		OnDying?.Invoke();
		DisableAllColliders();
		isDying = true;
		gameObject.layer = LayerMask.NameToLayer("Dead");
		foreach (var col in GetComponents<Collider2D>()) Destroy(col);
	}

	private void DisableAllColliders()
	{
		var colliders = GetComponents<Collider2D>();
		foreach (var col in colliders) col.enabled = false;

		var moreColliders = GetComponentsInChildren<Collider2D>();
		foreach (var col in moreColliders) col.enabled = false;
	}

	public bool IsDead()
	{
		return isDead;
	}

	public bool IsDeadOrDying()
	{
		return isDead || isDying;
	}

	public bool IsInvincible()
	{
		return isInvincible;
	}

	public float GetAimHeight()
	{
		if (stats is null) return 0;
		return stats.GetStatValue(StatType.aimHeight);
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public bool IsPlayer()
	{
		if (stats is null) stats = GetComponent<UnitStats>();
		return stats.isPlayer;
	}


	public bool IsDying()
	{
		return isDying;
	}

	public void AddHealth(int amount)
	{
		stats.ChangeStat(StatType.health, amount);
		OnFractionChanged?.Invoke(GetFraction());
	}
}
