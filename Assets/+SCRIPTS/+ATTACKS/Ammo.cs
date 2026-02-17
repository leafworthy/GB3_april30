using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class Ammo
	{
		public WeaponType weaponType;
		public int reserveAmmo;
		public int clipSize;
		public int AmmoInClip;
		public int maxReserveAmmo;
		public event Action OnAmmoGained;
		public event Action OnAmmoUsed;
		public bool reloads = true;
		public bool unlimited;
		public bool hasSlash = true;
		public bool whiteWhenFull = false;

		public float totalAmmo() => unlimited ? int.MaxValue : reserveAmmo + AmmoInClip;

		public bool hasAmmoInClip() => AmmoInClip > 0;

		public bool hasReserveAmmo(int min = 1)
		{
			if (unlimited) return true;
			return reserveAmmo >= min;
		}

		public bool hasAmmoInReserveOrClip()
		{
			if (unlimited) return true;
			return reserveAmmo + AmmoInClip > 0;
		}

		public void SetAmmoReserve(int amount)
		{
			reserveAmmo = Mathf.Clamp(amount, 0, maxReserveAmmo);
			OnAmmoGained?.Invoke();
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
			if (reloads)
			{
				AmmoInClip = Mathf.Max(AmmoInClip - amount, 0);
				OnAmmoUsed?.Invoke();
			}
			else
			{
				reserveAmmo = Mathf.Max(reserveAmmo - amount, 0);
				OnAmmoUsed?.Invoke();
			}
		}

		public void Reload()
		{
			if (unlimited)
			{
				AmmoInClip = clipSize;
				OnAmmoGained?.Invoke();
				return;
			}

			if (!CanReload()) return;

			var ammoNeeded = clipSize - AmmoInClip;
			if (ammoNeeded > reserveAmmo)
			{
				AmmoInClip += reserveAmmo;
				reserveAmmo = 0;
			}
			else
			{
				reserveAmmo -= ammoNeeded;
				AmmoInClip = clipSize;
			}

			OnAmmoGained?.Invoke();
		}

		public bool hasFullReserve() => reserveAmmo == maxReserveAmmo;

		public bool clipIsFull() => AmmoInClip >= clipSize;

		public void UseAmmo(int amount)
		{
			Use(amount);
		}



		public bool CanReload()
		{
			if (!reloads) return false;
			if (reserveAmmo <= 0) return false;
			if (AmmoInClip >= clipSize) return false;

			return true;
		}

		public void SetAmmo(Ammo ammoToSet)
		{
			reserveAmmo = ammoToSet.reserveAmmo;
			AmmoInClip = ammoToSet.AmmoInClip;
			OnAmmoGained?.Invoke();
		}
	}
}
