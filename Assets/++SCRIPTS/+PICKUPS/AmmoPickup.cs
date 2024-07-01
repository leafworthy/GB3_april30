using System.Collections.Generic;
using __SCRIPTS._PLAYER;

namespace __SCRIPTS._PICKUPS
{
	public class AmmoPickup : Pickup
	{
		public int amount;
		public AmmoInventory.AmmoType ammoType;

		public override List<PickupEffect> GetEffects()
		{
			var newList = new List<PickupEffect>();
			newList.Add(new AmmoPickupEffect(0, amount, ammoType));
			return newList;
		}
	}
}