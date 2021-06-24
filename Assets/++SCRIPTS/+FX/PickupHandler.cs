using System;
using System.Collections.Generic;
using System.Linq;
using _SCRIPTS;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
	public event Action<Pickup> OnPickup;
	private UnitStats stats;
	private List<PickupEffect> currentEffects = new List<PickupEffect>();
	private List<PickupEffect> effectsToRemove = new List<PickupEffect>();

	private void Update()
	{
		foreach (var currentEffect in currentEffects)
		{
			if (!currentEffect.CanUpdateEffect())
			{
				effectsToRemove.Add(currentEffect);
			}
		}

		foreach (var pickupEffect in effectsToRemove)
		{
			currentEffects.Remove(pickupEffect);

		}
	}

	public void PickUp(Pickup pickup)
	{
		stats = GetComponent<UnitStats>();
		foreach (var pickupEffect in pickup.GetEffects())
		{
			var alreadyHaveIt = currentEffects.FirstOrDefault(t => t == pickupEffect);
			if (alreadyHaveIt != null) continue;

			pickupEffect.StartEffect(stats);
			currentEffects.Add(pickupEffect);
		}
		OnPickup?.Invoke(pickup);
	}


}
