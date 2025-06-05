using System.Collections.Generic;

namespace __SCRIPTS
{
	public class GasPickup : Pickup
	{
		public int amount;
		public override List<PickupEffect> GetEffects()
		{
			var newList = new List<PickupEffect> {new GasPickupEffect(0, amount)};
			return newList;
		}
	}
}