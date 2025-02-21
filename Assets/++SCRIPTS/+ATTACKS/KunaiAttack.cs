using System;
using UnityEngine;

public class KunaiAttack : MonoBehaviour
{
	private AnimationEvents animationEvents;

	private Life life;
	private AimAbility aim;
	private Animations anim;
	private Body body;
	private AmmoInventory ammo;
	private string KunaiVerbName = "throwing kunai";
	private JumpAbility jumper;
	public event Action<Vector3, Vector3, float, Life, bool> OnThrow;
	private void Start()
	{
		Init();
	}

	private void Init()
	{
		jumper = GetComponent<JumpAbility>();
		ammo = GetComponent<AmmoInventory>();
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		aim = GetComponent<AimAbility>();
		life = GetComponent<Life>();

		life.player.Controller.Attack2LeftTrigger.OnPress += StartAttack;
		animationEvents = anim.animEvents;
		animationEvents.OnThrow += Anim_Throw;
		animationEvents.OnThrowStop += Anim_ThrowStop;
		animationEvents.OnAirThrowStop += Anim_AirThrowStop;
		animationEvents.OnAirThrow += Anim_AirThrow;
	}

	private void OnDisable()
	{ 
		life.player.Controller.Attack2LeftTrigger.OnPress -= StartAttack;
		animationEvents.OnThrow -= Anim_Throw;
		animationEvents.OnThrowStop -= Anim_ThrowStop;
		animationEvents.OnAirThrowStop -= Anim_AirThrowStop;
		animationEvents.OnAirThrow -= Anim_AirThrow;
	
	}

	private void Anim_Throw()
	{
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.kunai)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.kunai, 1);
	
		var throwHeight = body.ThrowPoint.transform.position.y - transform.position.y;
		OnThrow?.Invoke(aim.AimDir, transform.position, throwHeight, life, false);
	}

	private void Anim_ThrowStop()
	{
		body.arms.Stop(KunaiVerbName);
	}

	private void Anim_AirThrowStop()
	{
		body.arms.Stop(KunaiVerbName);
	}

	private void StartAttack(NewControlButton newControlButton)
	{
		
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.kunai))
		{
			return;
		}
		

		if (!aim.hasEnoughMagnitude() && !jumper.IsJumping) return;
		if (!body.arms.Do(KunaiVerbName) )
		{
			return;
		}

		anim.SetTrigger(Animations.ThrowTrigger);
		body.arms.currentActivity = KunaiVerbName;
	}


	private void Anim_AirThrow()
	{
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.kunai, 3)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.kunai, 3);
		var directionMult = body.BottomIsFacingRight ? 1 : -1;
		CreateProjectile(new Vector3(directionMult, -.5f, 0));
		CreateProjectile(new Vector3(directionMult, -1f, 0));
		CreateProjectile(new Vector3(directionMult, -.75f, 0));
	}

	private void CreateProjectile(Vector3 direction)
	{
		ThrowKunai(direction, body.AirThrowPoint.transform.position);
	}

	private void ThrowKunai(Vector3 direction, Vector3 pos)
	{
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.kunai)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.kunai, 1);
		OnThrow?.Invoke(direction, pos, life.AttackHeight, life, true);
	}
}