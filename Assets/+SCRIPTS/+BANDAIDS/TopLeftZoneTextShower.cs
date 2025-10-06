using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using Sirenix.OdinInspector;
using UnityEngine;

public class TopLeftZoneTextShower : GUIDebugTextShower
{
	protected override TextAnchor alignment => TextAnchor.UpperLeft;
	public BoundsSwitcher boundsSwitcher => GetComponent<BoundsSwitcher>();
	private int counter;
	private int rate = 20;
	private List<Collider2D> overlappingColliders = new();
	private List<Life> overlappingPlayers = new();
	public int index;
	private bool tryingToChange;
	public GameObject gogogo;
	private float cooldownTimer;

	private void Start()
	{
		Services.enemyManager.OnEnemyDying -= EnemyManager_OnEnemyDying;
		Services.enemyManager.OnEnemyDying += EnemyManager_OnEnemyDying;
		index = 0;
		boundsSwitcher.SetZone(index);
		SetTextToEnemiesInThisZone();
	}

	private void OnDisable()
	{
		Services.enemyManager.OnEnemyDying -= EnemyManager_OnEnemyDying;
	}

	private void Update()
	{


		SetTextToEnemiesInThisZone();
	}

	private void EnemyManager_OnEnemyDying(Life enemyLife)
	{
		SetTextToEnemiesInThisZone();
	}

	[Button]
	private void SetTextToEnemiesInThisZone()
	{
		var enemiesInThisZone = FindOverlappingEnemyColliders(Services.assetManager.LevelAssets.EnemyLayer);
		if (enemiesInThisZone <=3 )
		{
			// Add null check for nextZone and its collider
			if (boundsSwitcher.nextZone != null && boundsSwitcher.nextZone.zoneCollider != null &&
			    Collider2DExtensions.IsObjectWithinCollider(boundsSwitcher.nextZone?.zoneCollider, Services.playerManager?.mainPlayer?.SpawnedPlayerGO))
			{
				gogogo.SetActive(false);
				index++;
				boundsSwitcher.SetZone(index);
				Debug.Log("DID IT");
			}
			else
			{
				gogogo.SetActive(true);
				Debug.Log("all enemies dead but player not in zone");
			}
		}
		else
			gogogo.SetActive(false);

		Debug.Log("enemiesInThisZone: " + enemiesInThisZone);
		SetText(enemiesInThisZone.ToString());
	}

	private int FindOverlappingEnemyColliders(LayerMask targetLayerMask)
	{
		overlappingColliders.Clear();
		overlappingPlayers.Clear();

		// Add null checks before using the collider
		if (boundsSwitcher == null || boundsSwitcher.currentZone == null || boundsSwitcher.currentZone.zoneCollider == null)
		{
			Debug.LogWarning("BoundsSwitcher or currentZone or zoneCollider is null");
			return 0;
		}

		var bounds = boundsSwitcher.currentZone.zoneCollider;
		if (bounds == null)
		{
			Debug.Log("bounds is null");
			return 0;
		}

		Physics2D.OverlapCollider(bounds, new ContactFilter2D {layerMask = targetLayerMask, useLayerMask = true, useTriggers = true}, overlappingColliders);

		if (overlappingColliders.Count == 0) return 0;

		var overlappingEnemies = new List<SimpleEnemyAI>();

		foreach (var col in overlappingColliders)
		{
			var ai = col.GetComponent<SimpleEnemyAI>();
			if (ai == null) continue;
			var enemyLife = ai.GetComponent<Life>();
			if (enemyLife == null) continue;
			if (enemyLife.IsDead()) continue;
			if (overlappingEnemies.Contains(ai)) continue;
			overlappingEnemies.Add(ai);
		}

		var spawnPoints = boundsSwitcher.currentZone.GetComponentsInChildren<EnemyPlacer>(true).ToList();
		var unfinishedEnemySpawners = boundsSwitcher.currentZone.GetComponentsInChildren<EnemySpawner>(true).Where(  s => !s.IsFinished).ToList();

		return overlappingEnemies.Count + spawnPoints.Count + unfinishedEnemySpawners.Count;
	}
}
