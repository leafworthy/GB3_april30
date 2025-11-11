using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class ZoneSwitcher : MonoBehaviour
{
	public List<Zone> zones = new();
	public int zoneIndex;
	public Zone currentZone { get; private set; }
	public Zone nextZone => GetNextZone();

	private void OnEnable()
	{
		zoneIndex = 0;
	}

	private Zone GetNextZone()
	{
		if (zones == null || zones.Count == 0) return null;
		var nextIndex = zoneIndex + 1;
		return nextIndex < zones.Count ? zones[nextIndex] : null;
	}

	[Button]
	public void SetZone(int newIndex)
	{
		if (zones == null || zones.Count == 0) return;
		if (newIndex < 0 || newIndex >= zones.Count) return;

		zoneIndex = newIndex;
		currentZone = zones[zoneIndex];
		SetCamera(currentZone.zoneCamera);
	}

	[Button]
	public void AdvanceZone()
	{
		var next = GetNextZone();
		if (next == null)
		{
			Debug.Log("No more zones to advance to.");
			return;
		}

		zoneIndex++;
		currentZone = next;
		SetCamera(currentZone.zoneCamera);
		Debug.Log($"Switched to zone {zoneIndex} ({currentZone.name})");
	}

	private void SetCamera(CinemachineVirtualCameraBase newZoneCamera)
	{
		foreach (var zone in zones)
		{
			zone.zoneCamera.Priority = zone.zoneCamera == newZoneCamera ? 10 : 0;
		}
	}
}
