using UnityEngine;

public class SiphonGasInteraction : TimedInteraction
{
	public int timeToSiphon = 3;
	public int gasAmount = 3;
	public GameObject dropPoint;

	protected override void Start()
	{
		base.Start();
		totalTime = timeToSiphon;

		OnSelected += Interactable_OnPlayerEnters;
		OnDeselected += Interactable_OnPlayerExits;
		OnTimeComplete += Interactable_OnTimeComplete;
		OnActionPress += Interactable_OnActionPress;
	}

	private void Interactable_OnActionPress(Player player)
	{
		
		SFX.sounds.siphon_gas_sound.PlayRandomAt(transform.position);
	}

	protected override bool canEnter(Player player)
	{
		if (!base.canEnter(player)) return false;
		return gasAmount > 0;
	}

	private void Interactable_OnTimeComplete(Player player)
	{
		if (gasAmount <= 0)
		{
			player.Say("No more gas", 0);
			return;
		}
		LootTable.DropLoot(dropPoint.transform.position, LootType.Gas);
		gasAmount--;
		if (gasAmount <= 0)
		{
			gasAmount = 0;
			FinishInteraction(player);
		}
	}

	private void Interactable_OnPlayerEnters(Player player)
	{
		if(gasAmount <= 0)
		{
			return;
		}
		player.Say("Siphon Gas", 0);
	}

	private void Interactable_OnPlayerExits(Player player)
	{
		player.StopSaying();
	}

	protected override bool canInteract(Player player) {
		
		if (!base.canInteract(player)) return false;
		return gasAmount > 0;
	}
}