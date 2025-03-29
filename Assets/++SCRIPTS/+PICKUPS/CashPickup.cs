using System.Collections.Generic;

namespace __SCRIPTS
{
	public class CashPickup : Pickup
	{
		public int amount;

		public override List<PickupEffect> GetEffects()
		{
			var newList = new List<PickupEffect>();
			newList.Add(new CashPickupEffect(0, amount));
			return newList;
		}
	}
}