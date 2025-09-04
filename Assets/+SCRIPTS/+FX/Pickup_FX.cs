using __SCRIPTS.RisingText;
using UnityEngine;

namespace __SCRIPTS
{
	public class Pickup_FX : MonoBehaviour
	{
		[SerializeField] private CameraShaker.ShakeIntensityType shakeIntensity;
		[SerializeField] private CameraStunner_FX.StunLength stunLength;
		private Pickup pickup;
		private Life_FX lifeFX;
		private PickupItem pickupItem;

		private void Start()
		{
			pickupItem = GetComponent<PickupItem>();
			lifeFX = GetComponentInChildren<Life_FX>();
			pickup = GetComponent<Pickup>();
			if (pickup == null) return;
			pickup.OnPickup += StartPickup;
		}

		private void StartPickup(Collider2D col, Color pickupTintColor)
		{
			lifeFX.StartTint(pickupTintColor);
			var otherTintHandler = col.gameObject.GetComponentInChildren<Life_FX>(true);
			otherTintHandler.StartTint(pickupTintColor);
			var position = transform.position;
			Services.objectMaker.Make(Services.assetManager.FX.pickupEffectPrefab, position);
			CameraShaker.ShakeCamera(position,shakeIntensity);
			CameraStunner_FX.StartStun(stunLength);
			if(pickupItem.itemType == PickupItem.ItemType.cash)
				Services.risingText.CreateRisingText("+$" + pickupItem.itemAmount, position, Color.white);
			else
				Services.risingText.CreateRisingText("+" + pickupItem.itemAmount + " " +pickupItem.itemName, position, Color.white);
		}
	}
}
