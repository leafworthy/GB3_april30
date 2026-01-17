using UnityEngine;

namespace __SCRIPTS
{
	public class Pickup_FX : MonoBehaviour
	{
		[SerializeField] CameraShaker.ShakeIntensityType shakeIntensity;
		[SerializeField] CameraStunner_FX.StunLength stunLength;
		Pickup pickup;
		Tinter tinter;
		PickupItem pickupItem;

		void Start()
		{
			pickupItem = GetComponent<PickupItem>();
			tinter = GetComponentInChildren<Tinter>();
			pickup = GetComponent<Pickup>();
			if (pickup == null) return;
			pickup.OnPickup += StartPickup;
		}

		void StartPickup(Collider2D col, Color pickupTintColor)
		{
			tinter.StartTint(pickupTintColor);
			var otherTintHandler = col.gameObject.GetComponentInChildren<Tinter>(true);
			otherTintHandler.StartTint(pickupTintColor);
			var position = transform.position;
			Services.objectMaker.Make(Services.assetManager.FX.pickupEffectPrefab, position);
			CameraShaker.ShakeCamera(position, shakeIntensity);
			CameraStunner_FX.StartStun(stunLength);
			if (pickupItem.itemType == PickupItem.ItemType.cash)
				Services.risingText.CreateRisingText("+$" + pickupItem.itemAmount, position, Color.white);
			else
				Services.risingText.CreateRisingText("+" + pickupItem.itemAmount + " " + pickupItem.itemName, position, Color.white);
		}
	}
}
