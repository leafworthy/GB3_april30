using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpriteSortManager : MonoBehaviour
{
	public List<SpriteGroupSorter> sorters => _sorters ??= GetComponentsInChildren<SpriteGroupSorter>().ToList();
	[SerializeField]private List<SpriteGroupSorter> _sorters;
	public int offsetOrder = 0;

	[Button]
	public void GetSorters()
	{
		_sorters = GetComponentsInChildren<SpriteGroupSorter>().ToList();
		_sorters = sorters.OrderBy(x => x.order).ToList();
	}


	[Button]
	public void SortAll()
	{
		var i = 0;
		_sorters = sorters.OrderBy(x => x.order).ToList();
		foreach (var sorter in sorters)
		{
			Debug.Log("Sorting group " + sorter.name + " starting at order " + i);
			var amountOfSprites = sorter.SetSortingOrder(i+ offsetOrder);
			i += amountOfSprites;
		}
	}
}
