using System;
using Pathfinding;
using UnityEngine;

public class AstarAI : MonoBehaviour
{
	public event Action<Vector3> OnNewDirection;
	public event Action OnReachedEndOfPath;

	private Transform targetPosition;
	private Seeker seeker;
	private Path path;

	private float nextWaypointDistance = 3;
	private int currentWaypoint;

	private void Start () {
		seeker = GetComponent<Seeker>();
		var aStar = FindObjectOfType<AstarPath>();
		if (aStar is null)
		{
			this.enabled = false;
		}
	}

	public void SetTargetPosition(Transform newTargetPosition)
	{
		targetPosition = newTargetPosition;
		seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
	}

	private void OnPathComplete (Path p)
	{
		if (p.error) return;
		path = p;
		currentWaypoint = 0;
	}

	private void Update ()
	{
		if (PathIsInvalid()) return;
		UpdatePositionOnPath();

		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		OnNewDirection?.Invoke(dir);
	}

	private void UpdatePositionOnPath()
	{
		while (true)
		{
			var distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
			if (distanceToWaypoint < nextWaypointDistance)
			{
				if (currentWaypoint + 1 < path.vectorPath.Count)
				{
					currentWaypoint++;
				}
				else
				{
					OnReachedEndOfPath?.Invoke();
					break;
				}
			}
			else
			{
				break;
			}
		}
	}

	private bool PathIsInvalid()
	{
		if (targetPosition is null)
		{
			return true;
		}

		seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
		return path is null;
	}
}
