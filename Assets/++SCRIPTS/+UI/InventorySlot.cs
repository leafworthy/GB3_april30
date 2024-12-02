using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
	public Sprite fullGraphic;
	public Sprite emptyGraphic;
	public Image slotGraphic;
	public Image itemGraphic;
	public bool isFull;
	public Item currentItem;
	public TMP_Text itemAmountText;

	public void AddItemToSlot(Item item)
	{
		
		if (currentItem == null)
		{
			currentItem = item;
			
		}

		itemGraphic.sprite = item.itemGraphic;
		itemGraphic.enabled = true;
		slotGraphic.sprite = fullGraphic;
		if (item.itemType == Item.ItemType.key)
		{

			itemAmountText.text = "";
			return;
		}
		itemAmountText.text = currentItem.itemAmount.ToString();
	}

	public void RemoveItemFromSlot()
	{
		if (!isFull) return;
		itemGraphic.enabled = false;
		isFull = false;
		slotGraphic.sprite = emptyGraphic;
		itemAmountText.text = "";
	}
}