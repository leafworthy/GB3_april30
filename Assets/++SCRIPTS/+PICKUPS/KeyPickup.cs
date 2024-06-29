using System.Collections.Generic;

public class KeyPickup : Pickup
{
	public int amount;

	public override List<PickupEffect> GetEffects()
	{
		var newList = new List<PickupEffect> {new GasPickupEffect(0, amount)};
		return newList;
	}
}