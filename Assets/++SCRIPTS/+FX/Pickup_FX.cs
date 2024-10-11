using RisingText;
using UnityEngine;

public class Pickup_FX : MonoBehaviour
{
	[SerializeField] private CameraShaker.ShakeIntensityType shakeIntensity;
	[SerializeField] private CameraStunner.StunLength stunLength;
	private Pickup pickup;
	private Life_FX lifeFX;
	private Item item;

	private void Start()
	{
		item = GetComponent<Item>();
		lifeFX = GetComponent<Life_FX>();
		pickup = GetComponent<Pickup>();
		if (pickup == null) return;
		pickup.OnPickup += StartPickup;
	}

	private void StartPickup(Collider2D col, Color pickupTintColor)
	{
		lifeFX.StartTint(pickupTintColor);
		var otherTintHandler = col.gameObject.GetComponent<Life_FX>();
		otherTintHandler.StartTint(pickupTintColor);
		var position = transform.position+Vector3.up;
		CameraShaker.ShakeCamera(position,shakeIntensity);
		CameraStunner.StartStun(stunLength);
		if(item.itemType == Item.ItemType.cash)
			RisingTextCreator.CreateRisingText("+$" + item.itemAmount, position, Color.white);
		else
			RisingTextCreator.CreateRisingText("+" + item.itemAmount + " " +item.itemName, position, Color.white);
	}
}