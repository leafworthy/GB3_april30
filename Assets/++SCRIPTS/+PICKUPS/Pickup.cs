using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	[Serializable]
	public abstract class Pickup : MonoBehaviour
	{
		private List<PickupEffect> effects = new List<PickupEffect>();
		public Color pickupTintColor;
		private Animator animator;
		public event Action<Collider2D, Color> OnPickup;
		private bool hasBeenPickedUp;
		protected AudioClip Pickup_Sound;
		private static readonly int PickupTrigger = Animator.StringToHash("PickupTrigger");
		[FormerlySerializedAs("item")] public PickupItem pickupItem;
		

	

		public enum PickupType
		{
			Health,
			Damage,
			Cash,
			Nades,
			Jordans,
			Ammo
		}

		private void OnDisable()
		{
			hasBeenPickedUp = false;
		}

		public virtual List<PickupEffect> GetEffects()
		{
			return effects;
		}


		private void OnTriggerEnter2D(Collider2D other)
		{
			if (hasBeenPickedUp) return;
			animator ??= GetComponentInChildren<Animator>();

			var pickupHandler = other.GetComponent<PickupAbility>();
			if (pickupHandler is null) return;
			pickupHandler.PickUp(this);
			hasBeenPickedUp = true;
			OnPickup?.Invoke(other, pickupTintColor);
			animator.SetTrigger(PickupTrigger);
		}
	}
}