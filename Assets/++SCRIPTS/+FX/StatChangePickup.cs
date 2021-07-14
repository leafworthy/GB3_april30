using System.Collections.Generic;
using UnityEngine;

public class StatChangePickup : Pickup
{
	public StatType type;
	public float changeFactor = 1;
	public float changeAmount = 0;
	public int duration;
	public bool hasTintEffect;

	public override List<PickupEffect> GetEffects()
	{
		var newList = new List<PickupEffect>();
		newList.Add(new StatChangePickupEffect(duration, changeFactor, pickupTintColor, type, hasTintEffect, changeAmount));
		return newList;
	}

	public override AudioClip GetPickupSound()
	{
		if (type == StatType.moveSpeed)
		{
			return ASSETS.sounds.pickup_speed_sounds.GetRandom();
		}
		return base.GetPickupSound();
	}
}
