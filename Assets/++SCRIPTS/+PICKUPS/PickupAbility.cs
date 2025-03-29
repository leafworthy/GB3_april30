using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class PickupAbility : MonoBehaviour
	{
		private List<PickupEffect> currentEffects = new List<PickupEffect>();
		private List<PickupEffect> effectsToRemove = new List<PickupEffect>();
		public static event Action<Pickup, Player> OnPickup;

		private void Update()
		{
			foreach (var currentEffect in currentEffects)
			{
				currentEffect.UpdateEffect();
			}

			foreach (var currentEffect in effectsToRemove)
			{
				currentEffects.Remove(currentEffect);
			}
			effectsToRemove.Clear();
		}

		public void PickUp(Pickup pickup)
		{
			var life = GetComponent<Life>();
			if (life is null) return;
			OnPickup?.Invoke(pickup, life.player);

			foreach (var pickupEffect in pickup.GetEffects())
			{
				var alreadyHaveIt = currentEffects.FirstOrDefault(t => t == pickupEffect);
				if (alreadyHaveIt != null && pickup.pickupItem.itemType != PickupItem.ItemType.cash) continue;

				pickupEffect.StartEffect(life);
				pickupEffect.OnDone += EffectDone;
				currentEffects.Add(pickupEffect);
			}
		}

		private void EffectDone(PickupEffect effect)
		{
			effectsToRemove.Add(effect);
		}
	}
}