using UnityEngine;

public class LootOpenInteraction : PlayerInteractable
{
	public int howMuchLoot;
	public LootType lootType;
	public string MessageToDisplay;
	private HideRevealObjects hideRevealObjects;
	public Vector3 dropPosition;

	protected void Start()
	{
		hideRevealObjects = GetComponentInChildren<HideRevealObjects>();
		hideRevealObjects.Set(0);
		OnActionPress += interactable_OnInteract;
		OnPlayerEnters += interactable_PlayerEnters;
		OnPlayerExits += interactable_PlayerExits;
	}

	protected override bool canInteract(Player player) => howMuchLoot > 0;

	protected override bool canEnter(Player player) => howMuchLoot > 0;

	private void interactable_PlayerExits(Player player)
	{
		player.StopSaying();
	}

	private void interactable_PlayerEnters(Player player)
	{
		player.Say(MessageToDisplay, 2);
	}

	private void interactable_OnInteract(Player player)
	{
		hideRevealObjects.Set(1);
		LevelDrops.DropLoot(dropPosition + transform.position, lootType);
		howMuchLoot--;
		if (howMuchLoot >= 0) return;
		hideRevealObjects.Set(2);
		player.RemoveInteractable(this);
	}
}