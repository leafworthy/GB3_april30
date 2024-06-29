using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;

	private void Awake()
	{
		_prefab.Populate(count: 50);

		// If destroy active is true then even active instances will be destroyed
		_prefab.Clear(destroyActive: true);
	}
}