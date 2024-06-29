using UnityEngine;

public class LootOpener : PlayerInteractable
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

	protected override bool canInteract(Player player)
	{
		return howMuchLoot > 0;
	}

	protected override bool canEnter(Player player)
	{
		return howMuchLoot > 0;
	}

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
		LevelDrops.DropLoot(dropPosition+transform.position, lootType);
		howMuchLoot--;
		if (howMuchLoot <= 0)
		{ 
			
			player.RemoveInteractable(this);
		}
	}
}