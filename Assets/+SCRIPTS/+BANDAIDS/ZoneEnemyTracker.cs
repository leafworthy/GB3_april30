using System;
using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using UnityEngine;

[RequireComponent(typeof(Zone))]
public class ZoneEnemyTracker : MonoBehaviour
{
	public event Action<int> OnEnemyCountChanged;
	private Zone zone  => _zone ??= GetComponent<Zone>();
	private Zone _zone;
	private Collider2D _zoneCollider => zone?.zoneCollider;
	private readonly List<Collider2D> _overlapping = new();
	private ContactFilter2D _filter;


	private void OnEnable()
	{
		_filter = new()
		{
			layerMask = Services.assetManager.LevelAssets.EnemyLayer,
			useLayerMask = true,
			useTriggers = true
		};
		Services.enemyManager.OnEnemyDying += HandleEnemyDying;
	}

	private void OnDisable()
	{
		Services.enemyManager.OnEnemyDying -= HandleEnemyDying;
	}

	private void HandleEnemyDying(IGetAttacked _)
	{
		UpdateEnemyCount();
	}

	private int UpdateEnemyCount()
	{
		if (_zoneCollider == null)
			return 0;

		Physics2D.OverlapCollider(_zoneCollider, _filter, _overlapping);

		var activeEnemies = _overlapping.Select(c => c.GetComponent<SimpleEnemyAI>()).Where(ai => ai && ai.TryGetComponent(out Life life) && !life.IsDead())
		                                .Distinct().Count();

		var pendingPrefabs = GetComponentsInChildren<PrefabPlacer>(true).Length;
		var activeSpawners = GetComponentsInChildren<EnemySpawner>(true).Count(s => !s.IsFinished);

		var total = activeEnemies + pendingPrefabs + activeSpawners;
		OnEnemyCountChanged?.Invoke(total);
		return total;
	}

	public int GetEnemyCount() => UpdateEnemyCount();
}
