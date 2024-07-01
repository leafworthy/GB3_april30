using System.Collections.Generic;

namespace __SCRIPTS._PICKUPS
{
	public class HealthPickup : Pickup
	{
		public int amount;
		public override List<PickupEffect> GetEffects()
		{
			var newList = new List<PickupEffect>();
			newList.Add(new HealthPickupEffect(0, amount));
			return newList;
		}
	}
}
