using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(ZoneEnemyTracker))]
public class Zone : MonoBehaviour
{
	public Collider2D zoneCollider;
	public CinemachineVirtualCameraBase zoneCamera;
	public ZoneEnemyTracker enemyTracker => _enemyTracker ??= GetComponent<ZoneEnemyTracker>();
	private ZoneEnemyTracker _enemyTracker;
}
