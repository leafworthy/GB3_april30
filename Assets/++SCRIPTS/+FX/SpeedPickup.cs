using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : Pickup
{
	public float speedFactor;
	public Color tintColor;
	public int duration;

	public override List<PickupEffect> GetEffects()
	{
		var newList = new List<PickupEffect>();
		newList.Add(new MoveSpeedEffect(duration, speedFactor, tintColor));
		return newList;
	}
}
