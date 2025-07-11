using System.Collections.Generic;

namespace __SCRIPTS
{
	public class KeyPickup : Pickup
	{
		public int amount;

		public override List<PickupEffect> GetEffects()
		{
			var newList = new List<PickupEffect> {new KeyPickupEffect(0, amount)};
			return newList;
		}
	}
}