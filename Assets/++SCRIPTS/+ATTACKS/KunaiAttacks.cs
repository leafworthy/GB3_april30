using UnityEngine;

public class KunaiAttacks : MonoBehaviour
{
	private AnimationEvents animationEvents;

	private Life life;
	private GunAimer aim;
	private Animations anim;
	private Body body;
	private AmmoInventory ammo;
	private string KunaiVerbName = "throwing kunai";
	private JumpAbility jumper;
	private void Start()
	{
		jumper = GetComponent<JumpAbility>();
		ammo = GetComponent<AmmoInventory>();
		body = GetComponent<Body>();
		anim = GetComponent<Animations>();
		aim = GetComponent<GunAimer>();
		life = GetComponent<Life>();

		life.player.Controller.Attack2.OnPress += StartAttack;

		animationEvents = anim.animEvents;
		animationEvents.OnThrow += Anim_Throw;
		animationEvents.OnThrowStop += Anim_ThrowStop;
		animationEvents.OnAirThrowStop += Anim_AirThrowStop;
		animationEvents.OnAirThrow += Anim_AirThrow;
	}

	private void Anim_Throw()
	{
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.nades, 1);
		var newProjectile = Maker.Make(ASSETS.FX.kunaiPrefab, transform.position);
		var kunaiScript = newProjectile.GetComponent<Kunai>();
		var throwHeight = body.ThrowPoint.transform.position.y - transform.position.y;
		kunaiScript.Throw( aim.AimDir, transform.position, throwHeight, life);
		ASSETS.sounds.brock_bat_swing_sounds.PlayRandom();
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
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades))
		{
			return;
		}
		if (!body.arms.Do(KunaiVerbName) && !jumper.IsJumping)
		{
			return;
		}
		if(jumper.IsJumping && body.arms.currentActivity == "AirKunai")
		{
			anim.SetTrigger(Animations.ThrowTrigger);
			body.arms.currentActivity = "AirKunai";
			return;
		}

		

		anim.SetTrigger(Animations.ThrowTrigger);
	}


	private void Anim_AirThrow()
	{
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.nades, 3);
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
		if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades)) return;
		ammo.UseAmmo(AmmoInventory.AmmoType.nades, 1);
		var newProjectile = Maker.Make(ASSETS.FX.kunaiPrefab, transform.position);
		var projectileScript = newProjectile.GetComponent<Kunai>();
		projectileScript.Throw( direction, pos, life.AttackHeight, life);
		ASSETS.sounds.brock_bat_swing_sounds.PlayRandom();
		//FIRE
	}
}