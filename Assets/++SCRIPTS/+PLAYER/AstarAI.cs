using System;
using _PLUGINS.AstarPathfindingProject.Core;
using _PLUGINS.AstarPathfindingProject.Core.AI;
using UnityEngine;

public class AstarAI : MonoBehaviour
{
	public event Action<Vector3> OnNewDirection;
	public event Action OnReachedEndOfPath;

	private Transform targetPosition;
	private Seeker seeker;
	private Path path;
	private Vector3 dir;

	private int rate = 30;
	private int counter;

	private float nextWaypointDistance = 3;
	private int currentWaypoint;
	private bool isIdle;

	private void OnEnable () {
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
			if (seeker == null)
			{

				seeker = GetComponent<Seeker>();
			}
			counter = 0;
			if (targetPosition == null) return;
			seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
			return;
		}
		counter++;

		if (PathIsInvalid()) return;
		UpdatePositionOnPath();



	}

	private void UpdatePositionOnPath()
	{

			var distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
			if (!(distanceToWaypoint < nextWaypointDistance))
			{
				isIdle = false;
				return;
			}
			if (currentWaypoint + 1 < path.vectorPath.Count)
			{

				currentWaypoint++;
				dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
				OnNewDirection?.Invoke(dir);
			}
			else
			{
				if (!isIdle)
				{
					isIdle = true;
					OnReachedEndOfPath?.Invoke();
				}
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
