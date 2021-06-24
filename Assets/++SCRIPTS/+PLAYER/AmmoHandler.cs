using System;
using System.Collections.Generic;
using System.Linq;
using _SCRIPTS;
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
		cash
	}

	private List<Ammo> ammoList = new List<Ammo>();
	public Ammo primaryAmmo;
	public Ammo secondaryAmmo;
	public Ammo tertiaryAmmo;

	public bool isReloading;
	private UnitStats stats;

	private IAttackHandler beanAttackHandler;
	private Ammo ammoCurrentlyReloading;
	public event Action<Ammo> OnAmmoChanged;

	private void Start()
	{
		beanAttackHandler = GetComponent<IAttackHandler>();
		beanAttackHandler.OnUseAmmo += OnUseAmmo;
		stats = GetComponent<UnitStats>();
		ammoList.Add(primaryAmmo);
		ammoList.Add(secondaryAmmo);
		ammoList.Add(tertiaryAmmo);
	}

	private void AddAmmo(AmmoType type, int amount)
	{
		var ammo = ammoList.FirstOrDefault(t=>t.type == type);
		ammo.AddAmmoToReserve(amount);
	}

	public void ReloadStart()
	{
		isReloading = true;
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

	public void ReloadStop()
	{
		isReloading = false;
	}

	private void OnUseAmmo(AmmoType type, int amount)
	{
		var ammo = ammoList.FirstOrDefault(t => t.type == type);
		if (ammo is null) return;
		ammo.Use(amount);
		OnAmmoChanged?.Invoke(ammo);
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
