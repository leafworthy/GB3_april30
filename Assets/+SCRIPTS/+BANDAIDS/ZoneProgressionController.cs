using __SCRIPTS;
using UnityEngine;

[RequireComponent(typeof(ZoneSwitcher))]
public class ZoneProgressionController : MonoBehaviour
{
	[Tooltip("If enemy count drops to or below this number, advance to the next zone."), SerializeField]
	int advanceThreshold = 3;
	ZoneSwitcher ZoneSwitcher => zoneSwitcher ??= GetComponent<ZoneSwitcher>();
	ZoneSwitcher zoneSwitcher;
	ZoneEnemyTracker _currentTracker;

	void Start()
	{
		SetZone(0);
	}

	void OnDisable()
	{
		Unsubscribe();
	}

	void SetZone(int index)
	{
		ZoneSwitcher.SetZone(index);
		SubscribeToCurrentZone();
	}

	void SubscribeToCurrentZone()
	{
		Unsubscribe();

		var currentZone = ZoneSwitcher.currentZone;
		if (currentZone == null) return;

		_currentTracker = currentZone.enemyTracker;
		_currentTracker.OnEnemyCountChanged += HandleEnemyCountChanged;
		HandleEnemyCountChanged(_currentTracker.GetEnemyCount());
	}

	void Unsubscribe()
	{
		if (_currentTracker != null)
			_currentTracker.OnEnemyCountChanged -= HandleEnemyCountChanged;
	}

	void HandleEnemyCountChanged(int count)
	{
		if (count <= advanceThreshold)
			TryAdvanceZone();
	}

	void TryAdvanceZone()
	{
		var nextZone = ZoneSwitcher.nextZone;
		if (nextZone == null) return;
		var player = Services.playerManager?.mainPlayer?.SpawnedPlayerGO;

		var playerInNext = nextZone != null && nextZone.zoneCollider != null && MyExtensions.IsObjectWithinCollider(nextZone.zoneCollider, player);

		if (!playerInNext) return;
		ZoneSwitcher.AdvanceZone();
		SubscribeToCurrentZone();
	}
}
