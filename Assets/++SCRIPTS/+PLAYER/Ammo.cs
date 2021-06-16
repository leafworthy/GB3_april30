using System;
using UnityEngine;

[Serializable]
public class Ammo
{
	public AmmoHandler.AmmoType type;
	public int reserveAmmo;
	public int clipSize;
	public int AmmoInClip;
	public int maxReserveAmmo;
	public event Action OnAmmoGained;
	public event Action OnAmmoUsed;
	public bool reloads = true;


	public bool hasAmmoInClip()
	{
		return AmmoInClip > 0;
	}

	public bool hasAmmo()
	{
		return reserveAmmo > 0;
	}
	public void AddAmmoToReserve(int amount)
	{
		reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
		OnAmmoGained?.Invoke();
	}

	public void Remove(int amount)
	{
		reserveAmmo = Mathf.Max(reserveAmmo - amount, 0);
		OnAmmoUsed?.Invoke();
	}

	public void Use(int amount)
	{
		if (AmmoInClip < amount)
		{
			amount -= AmmoInClip;
			AmmoInClip = 0;
		}
		else
			AmmoInClip -= amount;

		reserveAmmo = Mathf.Max(reserveAmmo - amount, 0);
		OnAmmoUsed?.Invoke();
	}

	public void Reload()
	{
		if (!reloads) return;
		if (reserveAmmo <= 0) return;
		if (AmmoInClip == clipSize) return;
		if (clipSize - AmmoInClip > reserveAmmo)
		{
			AmmoInClip += reserveAmmo;
			reserveAmmo = 0;
		}
		else
		{
			AmmoInClip = clipSize;
			reserveAmmo -= clipSize;
		}

		OnAmmoGained?.Invoke();
	}

	public bool hasFullAmmo()
	{
		return reserveAmmo == maxReserveAmmo;
	}

	public bool clipIsFull()
	{
		return (AmmoInClip >= clipSize);
	}
}
