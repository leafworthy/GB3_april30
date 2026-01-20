using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class AmmoInventory : MonoBehaviour
	{
		public Ammo primaryAmmo;
		public Ammo secondaryAmmo;
		public Ammo tertiaryAmmo;
		public Ammo unlimitedAmmo;

		public event Action<Ammo> OnPrimaryAmmoAdded;

		public enum AmmoType
		{
			Primary,
			Secondary,
			Tertiary,
			Unlimited
		}

		public void AddAmmoToReserve(AmmoType ammoType, int amount)
		{
			switch (ammoType)
			{
				case AmmoType.Primary:
					primaryAmmo.AddAmmoToReserve(amount);
					OnPrimaryAmmoAdded ?.Invoke(primaryAmmo);
					break;
				case AmmoType.Secondary:
					secondaryAmmo.AddAmmoToReserve(amount);
					break;
				case AmmoType.Tertiary:
					tertiaryAmmo.AddAmmoToReserve(amount);
					break;
				case AmmoType.Unlimited:
					unlimitedAmmo.AddAmmoToReserve(amount);
					break;
				default:

					break;
			}
		}
	}
}
