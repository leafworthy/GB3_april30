using System.Collections.Generic;

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