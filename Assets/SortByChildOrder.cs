using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class SortByChildOrder : MonoBehaviour
{
	[SerializeField]private List<SpriteRenderer> _sprites;
	public int offsetOrder = -20000;

	[Button]
	public void SortAll()
	{
		_sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
		var i = 0;
		foreach (var sorter in _sprites)
		{
			sorter.sortingOrder = i + offsetOrder;
			i++;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying) SortAll();
	}
}
