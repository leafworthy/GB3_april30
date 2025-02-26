﻿using System;
using UnityEngine;

[ExecuteInEditMode]
public class Life : MonoBehaviour
{
	
	[HideInInspector] public Player player;
	public UnitStatsData unitData;


	private float ExtraMaxHealth;
	private float ExtraMaxSpeed;
	private float ExtraMaxDamage;
	protected virtual void OnValidate()
	{
		if (unitData != null) return;
		unitData = ASSETS.LevelAssets.DefaultUnitData;
	}


	public float AttackHeight => unitData.AttackHeight;
	public float AttackDamage => unitData.AttackDamage+ ExtraMaxDamage;
	public float AttackRange => unitData.AttackRange;
	public float AttackRate => unitData.AttackRate;

	public float Attack2Damage => unitData.Attack2Damage+ ExtraMaxDamage;
	public float Attack2Rate => unitData.Attack2Rate;


	public float HealthMax => unitData.HealthMax+ExtraMaxHealth;
	public float MoveSpeed => unitData.MoveSpeed + ExtraMaxSpeed;
	public float DashSpeed => unitData.DashSpeed + ExtraMaxSpeed;

	public bool IsPlayer => IsThisAPlayer();

	public bool isEnemyOf(Life otherLife) => IsPlayer != otherLife.IsPlayer;

	private bool IsThisAPlayer() => player != null && player.IsPlayer();

	public bool IsObstacle => unitData.isObstacle;
	public bool IsPlayerAttackable => unitData.isPlayerAttackable;

	public DebrisType DebrisType => unitData.debrisType;
	public float JumpSpeed => unitData.JumpSpeed;
	public float AggroRange => unitData.AggroRange;

	public void SetPlayer(Player _player)
	{
		player = _player;
	}
	public bool cantDie;
	public bool isInvincible;

	public event Action<Attack, Life> OnAttackHit;
	public event Action<Attack> OnDamaged;
	public event Action<float> OnFractionChanged;
	public event Action<Player, Life> OnDying;
	public event Action<Player, Life> OnKilled;
	public event Action<Player> OnDead;
	public event Action<Player> OnResurrected;


	private AnimationEvents animationEvents;
	private Animations anim;
	private bool isDead;
	private LayerMask originalLayer;

	[HideInInspector]public float Health;
	private Color backgroundColor;

	public static event Action<Attack, Life> OnAnyAttackHit;
	

	public float GetFraction()
	{
		if (HealthMax <= 0) return 0;
		return Health / HealthMax;
	}

	

	private void Start()
	{
		anim = GetComponent<Animations>();
		if (anim != null)
		{
			anim.animEvents.OnDieStop += Die;
			anim.animEvents.OnInvincible += SetInvincible;
		}

		originalLayer = gameObject.layer;
		ResetHealthToMax();
		OnFractionChanged?.Invoke(GetFraction());
	}

	private void SetInvincible(bool _isInvincible)
	{
		isInvincible = _isInvincible;
	}

	private void ResetHealthToMax()
	{
		Health = HealthMax;
	}

	public void TakeDamage(Attack attack)
	{
		if (IsDead()) return;
		if (attack.DestinationLife.isInvincible) return;
		//OnDamaged?.Invoke(attack);
		ChangeHealth(attack);

		MyDebugUtilities.DrawAttack(attack);

		CameraShaker_FX.ShakeCamera(transform.position, CameraShaker_FX.ShakeIntensityType.normal);
		if (anim != null)
		{
			anim.SetTrigger(Animations.HitTrigger);
			if (attack.IsDamaging)
			{
				OnDamaged?.Invoke(attack);
				//do damaging hit
			}
		}

		OnAnyAttackHit?.Invoke(attack, this);
		OnAttackHit?.Invoke(attack, this);
		//should be different based on material
		
		if (!(Health <= 0)) return;
		Health = 0;
		StartDying(attack);
	}

	private void DrawAttack(Attack attack)
	{
		DrawX(attack.OriginWithHeight, 2, Color.red);
		DrawX(attack.OriginFloorPoint, 2, Color.yellow);
		DrawX(attack.DestinationFloorPoint, 2, Color.green);
		DrawX(attack.DestinationWithHeight, 2, Color.blue);
	}

	private void DrawX(Vector2 pos, float size, Color color)
	{
		var topLeft = new Vector2(pos.x - size, pos.y - size);
		var topRight = new Vector2(pos.x + size, pos.y - size);
		var bottomLeft = new Vector2(pos.x - size, pos.y + size);
		var bottomRight = new Vector2(pos.x + size, pos.y + size);
		Debug.DrawLine(topLeft, bottomRight, color);
		Debug.DrawLine(topRight,bottomLeft,color);
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
		OnDamaged?.Invoke(attack);
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
		ObjectMaker.Unmake(gameObject);
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
		foreach (var col in colliders) col.enabled = false;

		var moreColliders = GetComponentsInChildren<Collider2D>();
		foreach (var col in moreColliders) col.enabled = false;
	}


	public bool IsDead()
	{
		return isDead;
	}


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

	public void SetExtraMaxHealth(float newExtraMaxHealth)
	{
		Debug.Log("extra max health set to: " + newExtraMaxHealth);
		ExtraMaxHealth = newExtraMaxHealth;
		OnFractionChanged?.Invoke(GetFraction());
	}

	public void SetExtraMaxDamage(float newIncreaseMaxDamage)
	{
		ExtraMaxDamage = newIncreaseMaxDamage;
	}

	public void SetExtraMaxSpeed(float newExtraMaxSpeed)
	{
		ExtraMaxSpeed = newExtraMaxSpeed;
	}
}