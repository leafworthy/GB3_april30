using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
	public enum AmmoType
	{
		ak47,
		nades,
		pistol,
		specialCooldown,
		meleeCooldown,
		kunai,
		cash,
		glock
	}

	private List<Ammo> ammoList = new List<Ammo>();
	public Ammo primaryAmmo;
	public Ammo secondaryAmmo;
	public Ammo tertiaryAmmo;
	public Ammo unlimitedAmmo;


	private IPlayerAttackHandler beanAttackHandler;
	private Ammo ammoCurrentlyReloading;

	private void Start()
	{
		beanAttackHandler = GetComponent<IPlayerAttackHandler>();
		beanAttackHandler.OnUseAmmo += OnUseAmmo;
		ammoList.Add(primaryAmmo);
		ammoList.Add(secondaryAmmo);
		ammoList.Add(tertiaryAmmo);
		ammoList.Add(unlimitedAmmo);
	}

	private void AddAmmo(AmmoType type, int amount)
	{
		var ammo = ammoList.FirstOrDefault(t=>t.type == type);
		ammo.AddAmmoToReserve(amount);
	}


	public void Reload(AmmoType ammoType)
	{
		if (!HasAmmo(ammoType))
		{
			Debug.Log("Out of ammo");
			return;
		}
		var ammo = ammoList.FirstOrDefault(t => t.type == ammoType);
		ammo?.Reload();
	}



	private void OnUseAmmo(AmmoType type, int amount)
	{
		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return;
		ammo.Use(amount);
	}

	public bool HasAmmo(AmmoType type)
	{

		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return false;
		return ammo.hasAmmo();
	}

	public bool HasFullAmmo(AmmoType type)
	{
		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return false;
		return ammo.hasFullAmmo();
	}

	public bool HasAmmoInClip(AmmoType type)
	{
		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return false;
		return ammo.hasAmmoInClip();
	}

	public void AddAmmoToReserve(AmmoType type, int amount)
	{
		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return;
		ammo.AddAmmoToReserve(amount);
	}

	public bool clipIsFull(AmmoType type)
	{
		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return false;
		return ammo.clipIsFull();
	}
}
