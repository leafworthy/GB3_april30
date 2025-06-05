using System.Collections.Generic;

namespace __SCRIPTS
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