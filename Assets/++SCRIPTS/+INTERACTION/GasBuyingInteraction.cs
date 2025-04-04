using UnityEngine;

namespace __SCRIPTS
{
	public class GasBuyingInteraction : TimedInteraction
	{
		public int price = 50;
		private bool occupied;
		public int gasAmount = 3;
		private HideRevealObjects hideRevealObjects;
		public GameObject dropPoint;

		protected override void OnEnable()
		{
			base.OnEnable();
			OnSelected += Interactable_OnPlayerEnters;
			OnDeselected += Interactable_OnPlayerExits;
			OnTimeComplete += Interactable_OnTimeComplete;
			hideRevealObjects = GetComponentInChildren<HideRevealObjects>();
		}

		protected override void OnDisable()
		{
			if(hideRevealObjects == null) return;
			base.OnDisable();
			 OnSelected -= Interactable_OnPlayerEnters;
			  OnDeselected -= Interactable_OnPlayerExits;
			   OnTimeComplete -= Interactable_OnTimeComplete;
			    
		}

		private void Interactable_OnTimeComplete(Player player)
		{
			LootTable.I.DropLoot(dropPoint.transform.position, LootType.Gas);
			player.SpendMoney(price);
			gasAmount--;
			if (gasAmount <= 0)
			{
				gasAmount = 0;
				hideRevealObjects.Set(1);
			}
		}

		private void Interactable_OnPlayerEnters(Player player)
		{
			if (!canInteract(player)) return;
			player.Say("$"+ price+" for gas", 0);
		}
		private void Interactable_OnPlayerExits(Player player)
		{
			player.StopSaying();
		}

		protected override bool canInteract(Player player)
		{
			if (!base.canInteract(player)) return false;
		
			return player.HasMoreMoneyThan(price) && gasAmount > 0;
		}
	}
}