using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using Sirenix.OdinInspector;
using UnityEngine;

public class TopLeftZoneTextShower : GUIDebugTextShower
{
	protected override TextAnchor alignment => TextAnchor.UpperLeft;
	public BoundsSwitcher boundsSwitcher;
	private int counter;
	private int rate = 20;
	private List<Collider2D> overlappingColliders = new();
	private List<Life> overlappingPlayers = new();
	public int index;
	private bool tryingToChange;
	public GameObject gogogo;

	private void Start()
	{
		if (boundsSwitcher == null) boundsSwitcher = GetComponent<BoundsSwitcher>();
		Services.enemyManager.OnEnemyDying -= EnemyManager_OnEnemyDying;
		Services.enemyManager.OnEnemyDying += EnemyManager_OnEnemyDying;
		SetTextToEnemiesInThisZone();
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
		if (enemiesInThisZone == 0)
		{
			if (Collider2DExtensions.IsObjectWithinCollider(boundsSwitcher.nextZone.zoneCollider, Services.playerManager.mainPlayer.SpawnedPlayerGO))
			{
				gogogo.SetActive(false);
				boundsSwitcher.SetZone(index + 1);
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

		SetText("Enemies left: " + enemiesInThisZone);
	}

	[Button]
	private void DoIt()
	{
		GetPlayersInThisAndNextZone();
	}

	private bool GetPlayersInThisAndNextZone()
	{
		var playersInThisZone = FindOverlappingPlayerColliders(index, Services.assetManager.LevelAssets.PlayerLayer);
		if (playersInThisZone == null || playersInThisZone.Count == 0) return false;
		var playersInNextsZone = FindOverlappingPlayerColliders(index + 1, Services.assetManager.LevelAssets.PlayerLayer);
		if (playersInNextsZone == null) return false;
		if (playersInNextsZone.Count == 0) return false;
		Debug.Log(HasCommonItem(playersInThisZone, playersInNextsZone));
		return HasCommonItem(playersInThisZone, playersInNextsZone);
	}

	public bool HasCommonItem(List<Life> list1, List<Life> list2) => list1.Intersect(list2).Any();

	private int FindOverlappingEnemyColliders(LayerMask targetLayerMask)
	{
		overlappingColliders.Clear();
		overlappingPlayers.Clear();
		var bounds = boundsSwitcher.currentZone.zoneCollider;

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

		var spawnPoints = FindObjectsByType<EnemyPlacer>(FindObjectsSortMode.None).ToList();
		var collided = Collider2DExtensions.FindEnemyPlacersWithinCollider(boundsSwitcher.currentZone.zoneCollider, spawnPoints);

		return overlappingEnemies.Count + collided.Count;
	}

	private List<Life> FindOverlappingPlayerColliders(int _index, LayerMask targetLayerMask)
	{
		Debug.Log("here");
		overlappingColliders.Clear();
		overlappingPlayers.Clear();
		var bounds = boundsSwitcher.currentZone.zoneCollider;
		Debug.Log(bounds, bounds);

		Physics2D.OverlapCollider(bounds, new ContactFilter2D {layerMask = targetLayerMask, useLayerMask = true, useTriggers = true}, overlappingColliders);

		if (overlappingColliders.Count == 0) return null;

		foreach (var col in overlappingColliders)
		{
			var life = col.GetComponent<Life>();
			if (life == null) continue;
			var playerController = life.GetComponent<PlayerUnitController>();
			if (playerController != null) overlappingPlayers.Add(life);
		}

		return overlappingPlayers;
	}
}
