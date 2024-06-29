using System;
using ToolBox.Pools;
using UnityEngine;

[ExecuteInEditMode]
public class Life : UnitStats
{
	public bool cantDie;

	public event Action<Attack> OnDamaged;
	public event Action<float> OnFractionChanged;
	public event Action<Player, Life> OnDying;
	public event Action<Player, Life> OnKilled;
	public event Action<Player> OnDead;
	public event Action<Player> OnResurrected;


	private AnimationEvents animationEvents;
	private Animations anim;
	private bool isDead;
	private float woundingAttackDamage = 25;
	private LayerMask originalLayer;

	[HideInInspector]public float Health;

	public static event Action<Attack, Life> OnAttackHit;
	

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
		}

		originalLayer = gameObject.layer;
		ResetHealthToMax();
		OnFractionChanged?.Invoke(GetFraction());
	}

	private void ResetHealthToMax()
	{
		Health = HealthMax;
	}

	public void TakeDamage(Attack attack)
	{
		if (IsDead()) return;
		OnDamaged?.Invoke(attack);
		ChangeHealth(attack);

		DrawAttack(attack);

		CameraShaker.ShakeCamera(transform.position, CameraShaker.ShakeIntensityType.normal);
		if (anim != null)
		{
			anim.SetTrigger(Animations.HitTrigger);
			if (attack.IsDamaging)
			{
				OnDamaged?.Invoke(attack);
			}
		}

		OnAttackHit?.Invoke(attack, this);
		ASSETS.sounds.GetHitSounds(DebreeType)?.PlayRandom(); //should be different based on material
		
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
		gameObject.Release();
	}

	private void SetHealth(float newHealth)
	{
		Health = Mathf.Min(newHealth, HealthMax);
		OnFractionChanged?.Invoke(Health / HealthMax);
	}

	private void DisableAllColliders()
	{
		if (cantDie) return;
		//Debug.Log("HERE");
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
}