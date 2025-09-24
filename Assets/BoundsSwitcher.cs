using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class BoundsSwitcher : MonoBehaviour
{
	public CinemachineConfiner2D confiner;

	public List<Zone> zones  = new ();
	public int colliderIndex;
	public Zone currentZone;
	public Zone nextZone =>  GetNextZone();

	private Zone GetNextZone()
	{
		if (zones.Count == 0) return null;
		var nextIndex = colliderIndex + 1;
		if (nextIndex >= zones.Count) nextIndex = 0;
		return zones[nextIndex];
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created

    [Button]
    void Switch()
    {
	    if (zones.Count == 0) return;
	    colliderIndex++;
	    SetZone(colliderIndex);
    }

    [Button]
    void Set()
    {
	    if (zones.Count == 0) return;
	    SetZone(colliderIndex);
    }

    public void SetZone(int newIndex)
    {
	    if (zones.Count == 0) return;
	    colliderIndex = newIndex;
	    currentZone = zones[colliderIndex];
	    SetCamera(currentZone.zoneCamera);
    }

    private void SetCamera(CinemachineVirtualCameraBase newZoneCamera)
    {
	    foreach (var zone in zones)
	    {
		    zone.zoneCamera.Priority = zone.zoneCamera == newZoneCamera ? 10 : 0;
	    }
    }
}
