using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Pickup : MonoBehaviour
{
	private List<PickupEffect> effects = new List<PickupEffect>();
	public Color pickupTintColor;
	private Animator animator;
	private static readonly int PickupTrigger = Animator.StringToHash("PickupTrigger");
	public event Action<Collider2D, Color> OnPickup;
	private bool hasBeenPickedUp;
	protected AudioClip Pickup_Sound;

	public virtual AudioClip GetPickupSound()
	{
		return ASSETS.sounds.pickup_pickup_sounds.GetRandom();
	}

	public enum PickupType
	{
		Health,
		Damage,
		Cash,
		Nades,
		Jordans,
		Ammo
	}

	public virtual List<PickupEffect> GetEffects()
	{
		return effects;
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (hasBeenPickedUp) return;
		animator ??= GetComponentInChildren<Animator>();

		var pickupHandler = other.GetComponent<PickupEffectHandler>();
		if (pickupHandler is null) return;
		pickupHandler.PickUp(this);
		hasBeenPickedUp = true;
		OnPickup?.Invoke(other, pickupTintColor);
		animator.SetTrigger(PickupTrigger);
	}
}
