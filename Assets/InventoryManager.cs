using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
	private Dictionary<Item.ItemType, Item> inventory = new();
	public GridLayoutGroup inventoryGrid;
	private List<InventorySlot> inventorySlots = new();

	private void Start()
	{
		//Add slots to grid
		foreach (Transform child in inventoryGrid.transform) inventorySlots.Add(child.GetComponent<InventorySlot>());
		//Add items to slots
		foreach (var item in inventory) AddItemToNextSlot(item.Value);

		PickupEffects.OnPickup += PickupEffects_OnPickup;
	}

	private void PickupEffects_OnPickup(Pickup pickup, Player player)
	{
		AddItemToInventory(pickup.item);
		AddItemToNextSlot(pickup.item);
	}

	private void AddItemToInventory(Item item)
	{
		if (inventory.ContainsKey(item.itemType))
		{
			inventory[item.itemType].itemAmount += item.itemAmount;
			return;
		}

		inventory.Add(item.itemType, item);
	}

	private void AddItemToNextSlot(Item item)
	{

		foreach (var inventorySlot in inventorySlots.Where(inventorySlot => inventorySlot.currentItem != null)
		                                            .Where(inventorySlot => inventorySlot.currentItem.itemType == item.itemType))
		{
			inventorySlot.AddItemToSlot(item);
			return;
		}

		var firstEmptySlot = inventorySlots.Find(x => !x.isFull);
		if (firstEmptySlot == null)
		{
			firstEmptySlot = Maker.Make(ASSETS.ui.InventorySlotPrefab).GetComponent<InventorySlot>();
			firstEmptySlot.transform.SetParent(inventoryGrid.transform);
			firstEmptySlot.transform.localPosition = Vector3.zero;
			inventorySlots.Add(firstEmptySlot);
		}
		firstEmptySlot.AddItemToSlot(item);
	}
}