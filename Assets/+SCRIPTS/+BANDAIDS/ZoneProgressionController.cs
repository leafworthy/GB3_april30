using UnityEngine;

[RequireComponent(typeof(ZoneSwitcher))]
public class ZoneProgressionController : MonoBehaviour
{
	[Tooltip("If enemy count drops to or below this number, advance to the next zone."), SerializeField]
	private int advanceThreshold = 3;
	private ZoneSwitcher ZoneSwitcher  => zoneSwitcher ??= GetComponent<ZoneSwitcher>();
	private ZoneSwitcher zoneSwitcher;
	private ZoneEnemyTracker _currentTracker;

	private void OnEnable()
	{
		SetZone(0);
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void SetZone(int index)
	{
		ZoneSwitcher.SetZone(index);
		SubscribeToCurrentZone();
	}

	private void SubscribeToCurrentZone()
	{
		Unsubscribe();

		var currentZone = ZoneSwitcher.currentZone;
		if (currentZone == null) return;

		_currentTracker = currentZone.enemyTracker;
		_currentTracker.OnEnemyCountChanged += HandleEnemyCountChanged;
		HandleEnemyCountChanged(_currentTracker.GetEnemyCount());
	}

	private void Unsubscribe()
	{
		if (_currentTracker != null)
			_currentTracker.OnEnemyCountChanged -= HandleEnemyCountChanged;
	}

	private void HandleEnemyCountChanged(int count)
	{
		if (count <= advanceThreshold)
			TryAdvanceZone();
	}

	private void TryAdvanceZone()
	{
		var nextZone = ZoneSwitcher.nextZone;
		if (nextZone == null)
		{
			Debug.Log("No next zone to advance to");
			return;
		}
		var player = Services.playerManager?.mainPlayer?.SpawnedPlayerGO;

		var playerInNext = nextZone != null && nextZone.zoneCollider != null && Collider2DExtensions.IsObjectWithinCollider(nextZone.zoneCollider, player);

		if (playerInNext)
		{
			Debug.Log("Player entered next zone, advancing");
			ZoneSwitcher.AdvanceZone();
			SubscribeToCurrentZone();
		}
		else
			Debug.Log("All enemies cleared, waiting for player to enter next zone");
	}
}
