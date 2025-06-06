using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class LootOpenInteraction : PlayerInteractable
	{
		public int howMuchLoot;
		public LootType lootType;
		public string MessageToDisplay;
		private HideRevealObjects hideRevealObjects;
		public Vector3 dropPosition;
		public LootContainerType lootContainerType;

		public enum LootContainerType
		{
			chest,
			drawer,
			trash,
			fridge
		}
		protected void Start()
		{
			hideRevealObjects = GetComponentInChildren<HideRevealObjects>();
			hideRevealObjects.Set(0);
			howMuchLoot =  Random.Range(1, howMuchLoot); // Randomly add 0-2 loot items to the container
			OnActionPress += interactable_OnInteract;
			OnPlayerEnters += interactable_PlayerEnters;
			OnPlayerExits += interactable_PlayerExits;
		}


		protected override bool canInteract(Player player)
		{
			if (!base.canInteract(player)) return false;

			return howMuchLoot > 0;
		}

		protected override bool canEnter(Player player) {
			if(!base.canEnter(player)) return false;
			return howMuchLoot > 0;
		}

		private void interactable_PlayerExits(Player player)
		{
			//player.StopSaying();
		}

		private void interactable_PlayerEnters(Player player)
		{
			//player.Say(MessageToDisplay, 2);
		}

		private void interactable_OnInteract(Player player)
		{
			PlayLootOpenSound();
			hideRevealObjects.Set(1);
			LootTable.I.DropLoot(dropPosition + transform.position, lootType);
			howMuchLoot--;
			if (howMuchLoot >= 0) return;
			hideRevealObjects.Set(2);
			FinishInteraction(player);
			player.RemoveInteractable(this);
		}

		private void PlayLootOpenSound()
		{
			switch (lootContainerType)
			{
				case LootContainerType.chest:
					SFX.I.sounds.chest_open_sound.PlayRandomAt(transform.position);
					break;
				case LootContainerType.drawer:
					SFX.I.sounds.drawer_open_sound.PlayRandomAt(transform.position);
					break;
				case LootContainerType.trash:
					SFX.I.sounds.trash_open_sound.PlayRandomAt(transform.position);
					break;
				case LootContainerType.fridge:
					SFX.I.sounds.fridge_open_sound.PlayRandomAt(transform.position);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
