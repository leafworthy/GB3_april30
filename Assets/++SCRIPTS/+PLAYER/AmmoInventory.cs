using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class AmmoInventory : MonoBehaviour
	{
		public enum AmmoType
		{
			primaryAmmo,
			nades,
			pistol,
			specialCooldown,
			meleeCooldown,
			cash,
			glock,
			nothing,
			unlimited
		}

		private List<Ammo> ammoList = new List<Ammo>();
		public Ammo primaryAmmo;
		public Ammo secondaryAmmo;
		public Ammo tertiaryAmmo;
		public Ammo unlimitedAmmo;

		private Ammo ammoCurrentlyReloading;

		private void Start()
		{
			ammoList.Add(primaryAmmo);
			ammoList.Add(secondaryAmmo);
			ammoList.Add(tertiaryAmmo);
			ammoList.Add(unlimitedAmmo);
		}



		public void Reload(AmmoType ammoType)
		{
			if (!HasReserveAmmo(ammoType))
			{
				return;
			}
			var ammo = ammoList.FirstOrDefault(t => t.type == ammoType);
			ammo?.Reload();
		}


		public bool HasReserveAmmo(AmmoType type, int min = 0)
		{

			var ammo = ammoList.FirstOrDefault(t => t.type == type);
			if (ammo is null) return false;
			return ammo.hasReserveAmmo(min);
		}

		public bool HasAmmoInReserveOrClip(AmmoType type)
		{

			var ammo = ammoList.FirstOrDefault(t => t.type == type);
			if (ammo is null) return false;
			return ammo.hasAmmoInReserveOrClip();
		}

		public bool HasFullReserve(AmmoType type)
		{
			var ammo = ammoList.FirstOrDefault(t => t.type == type);
			if (ammo is null) return false;
			return ammo.hasFullReserve();
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

		public void UseAmmo(AmmoType type, int amount)
		{
			var ammo = ammoList.FirstOrDefault(t => t.type == type);
			if (ammo is null) return;
			ammo.Use(amount);
		}
	}
}