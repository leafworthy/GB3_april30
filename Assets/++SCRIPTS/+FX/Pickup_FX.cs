using RisingText;
using UnityEngine;

public class Pickup_FX : MonoBehaviour
{
	[SerializeField] private CameraShaker_FX.ShakeIntensityType shakeIntensity;
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
		ObjectMaker.Make(ASSETS.FX.pickupEffectPrefab, position);
		CameraShaker_FX.ShakeCamera(position,shakeIntensity);
		CameraStunner_FX.StartStun(stunLength);
		if(pickupItem.itemType == PickupItem.ItemType.cash)
			RisingTextCreator.CreateRisingText("+$" + pickupItem.itemAmount, position, Color.white);
		else
			RisingTextCreator.CreateRisingText("+" + pickupItem.itemAmount + " " +pickupItem.itemName, position, Color.white);
	}
}