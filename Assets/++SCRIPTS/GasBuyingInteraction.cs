using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasBuyingInteraction : TimedInteraction
{
public int price = 50;
	private bool occupied;
	public int gasAmount = 3;
	private HideRevealObjects hideRevealObjects;
	public GameObject dropPoint;

	protected override void Start()
	{
		base.Start();
		OnPlayerEnters += Interactable_OnPlayerEnters;
		OnPlayerExits += Interactable_OnPlayerExits;
		OnTimeComplete += Interactable_OnTimeComplete;
		hideRevealObjects = GetComponentInChildren<HideRevealObjects>();
	}

	private void Interactable_OnTimeComplete(Player player)
	{
		LevelDrops.DropLoot(dropPoint.transform.position, LootType.Gas);
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
		player.Say("$"+ price+" for gas", 0);
	}
	private void Interactable_OnPlayerExits(Player player)
	{
		player.StopSaying();
	}

	protected override bool canInteract(Player player)
	{
		return player.HasMoreMoneyThan(price) && gasAmount < 0;
	}
}
