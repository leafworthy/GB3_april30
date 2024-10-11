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
	}

	private void Interactable_OnTimeComplete(Player player)
	{
		LevelDrops.DropLoot(dropPoint.transform.position, LootType.Gas);
		gasAmount--;
		if (gasAmount <= 0)
		{
			gasAmount = 0;
		}
	}

	private void Interactable_OnPlayerEnters(Player player)
	{
		if(gasAmount <= 0) return;
		player.Say("Siphon Gas", 0);
	}

	private void Interactable_OnPlayerExits(Player player)
	{
		player.StopSaying();
	}

	protected override bool canInteract(Player player) => gasAmount > 0;
}