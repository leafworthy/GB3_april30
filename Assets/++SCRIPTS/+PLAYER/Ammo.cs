using System;
using UnityEngine;

namespace GangstaBean.Player
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

		public bool hasAmmoInClip() => AmmoInClip > 0;

		public bool hasReserveAmmo(int min = 0)
		{
			if (unlimited) return true;
			return reserveAmmo > min;
		}

		public bool hasAmmoInReserveOrClip()
		{
			if (unlimited) return true;
			return reserveAmmo + AmmoInClip > 0;
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

			if (!reloads) return;
			if (reserveAmmo <= 0) return;
			if (AmmoInClip >= clipSize) return;
			var ammoNeeded = clipSize - AmmoInClip;

			if (ammoNeeded > reserveAmmo)
			{
				AmmoInClip += reserveAmmo;
				reserveAmmo = 0;
			}
			else
			{
				reserveAmmo -= ammoNeeded;
				AmmoInClip += ammoNeeded;
			}

			OnAmmoGained?.Invoke();
		}

		public bool hasFullReserve() => reserveAmmo == maxReserveAmmo;

		public bool clipIsFull() => AmmoInClip >= clipSize;

		public void UseAmmo(int amount)
		{
			Use(amount);
		}
	}
}