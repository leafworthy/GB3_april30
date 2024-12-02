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
		
		foreach (Transform child in inventoryGrid.transform) inventorySlots.Add(child.GetComponent<InventorySlot>());
		//Add items to slots
		//foreach (var item in inventory) AddItemToNextSlot(item.Value);

		PickupAbility.OnPickup += PickupEffects_OnPickup;
	}

	private void PickupEffects_OnPickup(Pickup pickup, Player player)
	{
		Debug.Log("on pickup");
		if (pickup.item.itemType == Item.ItemType.key)
		{
			if(player.hasKey) return;
			AddItemToInventory(pickup.item);
			AddItemToNextSlot(pickup.item);
			return;
		}

		if (pickup.item.itemType == Item.ItemType.gas)
		{
			AddItemToInventory(pickup.item);
			AddItemToNextSlot(pickup.item);
			
		}
	}

	private void AddItemToInventory(Item item)
	{
		if (inventory.ContainsKey(item.itemType))
		{
			inventory[item.itemType].itemAmount += item.itemAmount;
			Debug.Log("adding more " + item.itemType + "total: " + inventory[item.itemType].itemAmount);
			return;
		}

		inventory.Add(item.itemType, item);
	}

	private void AddItemToNextSlot(Item item)
	{

		foreach (var inventorySlot in inventorySlots.Where(inventorySlot => inventorySlot.currentItem != null)
		                                            .Where(inventorySlot => inventorySlot.currentItem.itemType == item.itemType))
		{
			Debug.Log("found slot");
			inventorySlot.AddItemToSlot(item);
			return;
		}

		//var firstEmptySlot = inventorySlots.Find(x => !x.isFull);
		
			var firstEmptySlot = Maker.Make(ASSETS.ui.InventorySlotPrefab).GetComponent<InventorySlot>();
			firstEmptySlot.transform.SetParent(inventoryGrid.transform);
			firstEmptySlot.transform.localPosition = Vector3.zero;
			firstEmptySlot.transform.localScale = Vector3.one;
			               inventorySlots.Add(firstEmptySlot);
		
		firstEmptySlot.AddItemToSlot(item);
	}
}