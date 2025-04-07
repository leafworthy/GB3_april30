using System;
using __SCRIPTS;
using UnityEngine;

public class Wall : MonoBehaviour
{
	public isoDirection direction = isoDirection.none;

	// Optional: Add more wall-specific properties here as needed
	[Header("Wall Properties")] public bool hasWindow = false;
	public bool hasDoor = false;

	// Visual indicators for direction (optional)
	private void OnDrawGizmosSelected()
	{
		if (direction == isoDirection.none)
			return;

		Vector3 dir = Vector3.zero;
		Color gizmoColor = Color.white;

		// Set direction and color based on wall orientation
		switch (direction)
		{
			case isoDirection.NE:
				dir = new Vector3(1, 0.5f, 0);
				gizmoColor = new Color(0.8f, 0.2f, 0.2f, 0.8f); // Red
				break;
			case isoDirection.SE:
				dir = new Vector3(1, -0.5f, 0);
				gizmoColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); // Green
				break;
			case isoDirection.SW:
				dir = new Vector3(-1, -0.5f, 0);
				gizmoColor = new Color(0.2f, 0.2f, 0.8f, 0.8f); // Blue
				break;
			case isoDirection.NW:
				dir = new Vector3(-1, 0.5f, 0);
				gizmoColor = new Color(0.8f, 0.8f, 0.2f, 0.8f); // Yellow
				break;
		}

		// Draw direction arrow
		Gizmos.color = gizmoColor;
		Vector3 startPos = transform.position;
		Vector3 endPos = startPos + dir.normalized * 2f;

		Gizmos.DrawLine(startPos, endPos);

		// Draw arrow head
		Vector3 right = Quaternion.Euler(0, 0, 30) * -dir.normalized;
		Vector3 left = Quaternion.Euler(0, 0, -30) * -dir.normalized;

		Gizmos.DrawLine(endPos, endPos + right * 0.5f);
		Gizmos.DrawLine(endPos, endPos + left * 0.5f);
	}
	
	
}