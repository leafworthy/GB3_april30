using UnityEngine;

namespace __SCRIPTS
{
	public class Pickup_SFX : ServiceUser
	{
		private Pickup pickup;

		private void Start()
		{
			pickup = GetComponent<Pickup>();
			if (pickup == null) return;
			pickup.OnPickup += StartPickup;
		}

		private void StartPickup(Collider2D col, Color pickupTintColor)
		{
			sfx.PlayUISound(sfx.sounds.pickup_pickup_sounds.GetRandom());
		}
	}
}
