using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(ZoneSwitcher))]
public class ZoneUI : GUIDebugTextShower
{
	protected override TextAnchor alignment => TextAnchor.UpperLeft;
	private ZoneSwitcher zoneSwitcher;
	[SerializeField] private GameObject goIndicator;


	private void Start()
	{
		Services.enemyManager.OnEnemyDying += OnEnemyDying;
		zoneSwitcher = GetComponent<ZoneSwitcher>();
		zoneSwitcher.SetZone(0);
		UpdateZoneText();
	}

	private void OnDestroy()
	{
		if (Services.enemyManager == null) return;
		Services.enemyManager.OnEnemyDying -= OnEnemyDying;
	}

	private void FixedUpdate()
	{
		UpdateZoneText();
	}

	private void OnEnemyDying(IGetAttacked _) => UpdateZoneText();

	[Button]
	private void UpdateZoneText()
	{
		int remainingEnemies = CountEnemiesInCurrentZone();

		if (remainingEnemies <= 3)
			TryAdvanceZone();
		else
			goIndicator?.SetActive(false);

		SetText(remainingEnemies.ToString());
	}

	private void TryAdvanceZone()
	{
		var nextZone = zoneSwitcher.nextZone;
		var player = Services.playerManager?.mainPlayer?.SpawnedPlayerGO;

		bool playerInNextZone = nextZone != null &&
		                        nextZone.zoneCollider != null &&
		                        Collider2DExtensions.IsObjectWithinCollider(nextZone.zoneCollider, player);

		if (playerInNextZone)
		{
			goIndicator?.SetActive(false);
			zoneSwitcher.AdvanceZone();
		}
		else
		{
			goIndicator?.SetActive(true);
		}
	}

	private int CountEnemiesInCurrentZone()
	{
		var zone = zoneSwitcher.currentZone;
		if (zone?.zoneCollider == null)
			return 0;

		var colliders = new List<Collider2D>();
		var filter = new ContactFilter2D
		{
			layerMask = Services.assetManager.LevelAssets.EnemyLayer,
			useLayerMask = true,
			useTriggers = true
		};

		Physics2D.OverlapCollider(zone.zoneCollider, filter, colliders);

		int activeEnemies = colliders
			.Select(c => c.GetComponent<SimpleEnemyAI>())
			.Where(ai => ai && ai.TryGetComponent(out Life life) && !life.IsDead())
			.Distinct()
			.Count();

		int pendingSpawns = zone.GetComponentsInChildren<PrefabPlacer>(true).Length;
		int activeSpawners = zone.GetComponentsInChildren<EnemySpawner>(true)
			.Count(s => !s.IsFinished);

		return activeEnemies + pendingSpawns + activeSpawners;
	}
}
