using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class ChildWorldPositionLocker : MonoBehaviour
{
	[Header("Controls"), SerializeField]

	private bool lockChildren;

	private Dictionary<Transform, Vector3> lockedPositions = new();
	private bool wasLocked;

	private void Update()
	{
		// Check if lock state changed
		if (lockChildren != wasLocked)
		{
			if (lockChildren)
				LockChildrenPositions();
			else
				UnlockChildrenPositions();
			wasLocked = lockChildren;
		}

		// If locked, maintain positions
		if (lockChildren && lockedPositions.Count > 0) UpdateChildrenPositions();
	}

	/// <summary>
	/// Lock all direct children at their current world positions
	/// </summary>
	[Button]
	public void LockChildrenPositions()
	{
		lockedPositions.Clear();

		for (var i = 0; i < transform.childCount; i++)
		{
			var child = transform.GetChild(i);
			lockedPositions[child] = child.position;
		}

		lockChildren = true;
	}

	/// <summary>
	/// Unlock all children
	/// </summary>
	[Button]
	public void UnlockChildrenPositions()
	{
		lockedPositions.Clear();
		lockChildren = false;
	}

	private void UpdateChildrenPositions()
	{
		var childrenToRemove = new List<Transform>();

		foreach (var kvp in lockedPositions)
		{
			var child = kvp.Key;
			var lockedPosition = kvp.Value;

			// Remove null or non-child transforms
			if (child == null || child.parent != transform)
			{
				childrenToRemove.Add(child);
				continue;
			}

			// Keep child at locked world position
			child.position = lockedPosition;
		}

		// Clean up invalid entries
		foreach (var child in childrenToRemove)
		{
			lockedPositions.Remove(child);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!lockChildren) return;

		Gizmos.color = Color.red;
		foreach (var kvp in lockedPositions)
		{
			if (kvp.Key != null)
			{
				Gizmos.DrawWireSphere(kvp.Value, 0.2f);
				Gizmos.DrawLine(transform.position, kvp.Value);
			}
		}
	}
}
