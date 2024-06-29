using UnityEngine;

public class PickupFX : MonoBehaviour
{
	[SerializeField] private CameraShaker.ShakeIntensityType shakeIntensity;
	[SerializeField] private CameraStunner.StunLength stunLength;
	private Pickup pickup;
	private TintHandler tintHandler;

	private void Start()
	{
		tintHandler = GetComponent<TintHandler>();
		pickup = GetComponent<Pickup>();
		if (pickup == null) return;
		pickup.OnPickup += StartPickup;
	}

	private void StartPickup(Collider2D col, Color pickupTintColor)
	{
		tintHandler.StartTint(pickupTintColor);
		var otherTintHandler = col.gameObject.GetComponent<TintHandler>();
		otherTintHandler.StartTint(pickupTintColor);
		var position = transform.position;
		Audio.PlaySound(pickup.GetPickupSound());
		CameraShaker.ShakeCamera(position,shakeIntensity);
		CameraStunner.StartStun(stunLength);
		Maker.Make(ASSETS.FX.pickupEffectPrefab, position);
	}
}
