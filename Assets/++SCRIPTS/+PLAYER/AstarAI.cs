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

	private int rate = 30;
	private int counter;

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
	}

	private void OnPathComplete (Path p)
	{
		if (p.error) return;
		path = p;
		currentWaypoint = 0;
	}

	private void FixedUpdate ()
	{

		if (counter >= rate)
		{
			counter = 0;
			seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
			return;
		}
		counter++;

		if (PathIsInvalid()) return;
		UpdatePositionOnPath();

		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		OnNewDirection?.Invoke(dir);

	}

	private void UpdatePositionOnPath()
	{

			var distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
			if (!(distanceToWaypoint < nextWaypointDistance)) return;
			if (currentWaypoint + 1 < path.vectorPath.Count)
			{
				currentWaypoint++;
			}
			else
			{
				OnReachedEndOfPath?.Invoke();
			}

	}

	private bool PathIsInvalid()
	{
		if (targetPosition is null)
		{
			return true;
		}


		return path is null;
	}
}
