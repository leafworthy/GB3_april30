using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[ExecuteAlways, System.Serializable]
public class SpriteGroup : SerializedMonoBehaviour
{
	public int order;
	private List<SpriteDepth> sprites => _sprites ??= GetComponentsInChildren<SpriteDepth>(true).ToList();
	[OdinSerialize] private List<SpriteDepth> _sprites;

	[Button]
	public void GetSprites()
	{
		_sprites = GetComponentsInChildren<SpriteDepth>(true).ToList();
		_sprites = _sprites.OrderBy(x => x.order).ToList();
	}

	[Button]
	public int SetSortingOrder(int newOrder)
	{
		int i = 0;
		GetSprites();
		foreach (var sprite in _sprites) // Use _sprites directly
		{
			Debug.Log("Setting sorting order of " + sprite.name + " to " + i, this);
			sprite.SetSortingOrder(i+ newOrder);
			i++;
		}

		return i;
	}
}
