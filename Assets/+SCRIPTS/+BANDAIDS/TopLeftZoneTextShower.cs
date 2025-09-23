using System.Collections.Generic;
using __SCRIPTS._ENEMYAI;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class TopLeftZoneTextShower : GUIDebugTextShower
{
	protected override TextAnchor alignment => TextAnchor.UpperLeft;
	public BoundsSwitcher boundsSwitcher;
	private int counter;
	private int rate = 20;

	[Button]
	private void DoIt()
	{


		FindOverlappingColliders(Services.assetManager.LevelAssets.EnemyLayer);
	}

	private void Update()
	{
		if (counter++ < rate) return;
		counter = 0;

		SetText(FindOverlappingColliders(Services.assetManager.LevelAssets.EnemyLayer).ToString());

	}

	private int FindOverlappingColliders(LayerMask targetLayerMask)
	{
		var bounds = boundsSwitcher.colliders[boundsSwitcher.colliderIndex];
		// Create an array to store the results
		var overlappingColliders = new List<Collider2D>();

		// Use OverlapCollider to get the number of overlaps and populate the array
		Physics2D.OverlapCollider(bounds, new ContactFilter2D {layerMask = targetLayerMask, useLayerMask = true, useTriggers = true},
			overlappingColliders);

		if (overlappingColliders.Count == 0)
		{
			Debug.Log("null");
			return 0;
		}

		var overlappingEnemies = new List<SimpleEnemyAI>();

		Debug.Log("total found: " + overlappingColliders.Count);

		foreach (var col in overlappingColliders)
		{
			var ai = col.GetComponent<SimpleEnemyAI>();
			if (ai == null) continue;
			overlappingEnemies.Add(ai);
		}
		return overlappingEnemies.Count;
	}
}
