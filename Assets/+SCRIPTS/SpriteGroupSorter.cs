using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpriteGroupSorter : MonoBehaviour
{
	public int order;
	private List<SpriteGroup> sorters => _sorters ??= GetComponentsInChildren<SpriteGroup>().ToList();
	[SerializeField]private List<SpriteGroup> _sorters;

	[Button]
	public void GetSorters()
	{
		_sorters = GetComponentsInChildren<SpriteGroup>().ToList();
		_sorters = sorters.OrderBy(x => x.order).ToList();
	}

	[Button]
	public void SetOrderToChildOrder()
	{
		_sorters = GetComponentsInChildren<SpriteGroup>().ToList();
		foreach (var sorter in sorters)
		{
			sorter.order = sorter.transform.GetSiblingIndex();
		}
	}
	[Button]
	public int SetSortingOrder(int newOrder)
	{
		int i = 0;
		SetOrderToChildOrder();
		_sorters = sorters.OrderBy(x => x.order).ToList();
		foreach (var sorter in sorters)
		{
			Debug.Log("Setting sorter " + sorter.name + " to order " + i);
			var amountOfSprites = sorter.SetSortingOrder(i+ newOrder);
			i += amountOfSprites;
		}
		return i;
	}
}
