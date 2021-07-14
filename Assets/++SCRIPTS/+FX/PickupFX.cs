using UnityEngine;


public class PickupFX : MonoBehaviour
{
	[SerializeField] private SHAKER.ShakeIntensityType shakeIntensity;
	[SerializeField] private HITSTUN.StunLength stunLength;
	private Pickup pickup;
	private TintHandler tintHandler;

	private void Start()
	{
		tintHandler = GetComponent<TintHandler>();
		pickup = GetComponent<Pickup>();
		pickup.OnPickup += StartPickup;
	}

	private void StartPickup(Collider2D col, Color pickupTintColor)
	{
		tintHandler.StartTint(pickupTintColor);
		var otherTintHandler = col.gameObject.GetComponent<TintHandler>();
		otherTintHandler.StartTint(pickupTintColor);
		AUDIO.PlaySound(pickup.GetPickupSound());
		var position = transform.position;
		SHAKER.ShakeCamera(position,shakeIntensity);
		HITSTUN.StartStun(stunLength);
		MAKER.Make(ASSETS.FX.pickupEffectPrefab, position);
	}
}
