using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class BoundsSwitcher : MonoBehaviour
{
	public CinemachineConfiner2D confiner;

	public List<Collider2D> colliders  = new List<Collider2D>();
	public int colliderIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Button]
    void Switch()
    {
	    if (colliders.Count == 0) return;
	    colliderIndex++;
	    SetCollider();
    }


    public void SetBounds(int newIndex)
    {
	    if (colliders.Count == 0) return;
	    colliderIndex = newIndex;
	    SetCollider();
    }

    private void SetCollider()
    {
	    if (colliderIndex >= colliders.Count) colliderIndex = 0;
	    confiner.BoundingShape2D = colliders[colliderIndex];
	    confiner.InvalidateBoundingShapeCache();
    }
}
