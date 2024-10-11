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
		OnSelected += Interactable_OnPlayerEnters;
		OnDeselected += Interactable_OnPlayerExits;
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
		if (!canInteract(player)) return;
		player.Say("$"+ price+" for gas", 0);
	}
	private void Interactable_OnPlayerExits(Player player)
	{
		player.StopSaying();
	}

	protected override bool canInteract(Player player) => player.HasMoreMoneyThan(price) && gasAmount > 0;
}