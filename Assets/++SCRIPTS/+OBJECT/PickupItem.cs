using UnityEngine;

namespace GangstaBean.Objects
{
	public class PickupItem : MonoBehaviour
	{
		public string itemName;
		public Sprite itemGraphic;
		public ItemType itemType;
		public int itemAmount;

		public enum ItemType
		{
			health,
			gas,
			cash,
			key,
			book,
			ammo,
			nades
		}

		public void Use()
		{
		
		}
	}
}