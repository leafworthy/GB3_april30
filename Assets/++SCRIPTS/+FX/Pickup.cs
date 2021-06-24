using System;
using System.Collections.Generic;
using _SCRIPTS;
using UnityEngine;

[Serializable]
public class Pickup : MonoBehaviour
{
	private List<PickupEffect> effects = new List<PickupEffect>();

	private Animator animator;
	private static readonly int PickupTrigger = Animator.StringToHash("PickupTrigger");
	public event Action OnPickup;
	private bool hasBeenPickedUp;

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
		var pickupHandler = other.GetComponent<PickupHandler>();
		if (pickupHandler is null) return;
		pickupHandler.PickUp(this);
		hasBeenPickedUp = true;
		OnPickup?.Invoke();
		animator.SetTrigger(PickupTrigger);
		var fx = MAKER.Make(ASSETS.FX.pickupEffectPrefab, transform.position);
		//MAKER.Unmake(fx,3);
	}
}
