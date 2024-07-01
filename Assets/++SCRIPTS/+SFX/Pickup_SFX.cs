using __SCRIPTS._COMMON;
using __SCRIPTS._PICKUPS;
using UnityEngine;

namespace __SCRIPTS._SFX
{
	public class Pickup_SFX : MonoBehaviour
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
			SFX.PlaySound(SFX.sounds.pickup_pickup_sounds.GetRandom());
		}
	}
}