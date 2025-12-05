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
		_sprites = GetComponentsInChildren<SpriteRenderer>(true).ToList();
		var i = 0;
		foreach (var sorter in _sprites)
		{
			sorter.sortingOrder = i + offsetOrder;
			Debug.Log(i + offsetOrder);
			Debug.Log(  "Setting sorting order of " + sorter.gameObject.name + " to " + sorter.sortingOrder + "offset is " + offsetOrder + "and i is " + i, this);
			i++;
		}
	}

	private void Update()
	{
		//if (!Application.isPlaying) SortAll();
	}
}
