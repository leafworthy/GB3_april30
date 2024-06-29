using System;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class AstarPathfinder : MonoBehaviour
{
	public event Action<Vector2> OnNewDirection;

	private Vector2 targetPosition;
	private Seeker seeker;
	private Path currentPath;
	private Vector3 currentDirection;

	private int rate = 15;
	private int counter;

	private float nextWaypointDistance =2;
	private int currentWaypoint;
	private Vector2 lastPosition;
	private int frozenCounter;
	private int freezeLimit = 200;

	private void OnEnable () {
		seeker = GetComponent<Seeker>();
		currentPath = null;
		var aStar = FindObjectOfType<AstarPath>();
		if (aStar is null)
		{
			enabled = false;
		}

		lastPosition = transform.position;
	}


	public void SetTargetPosition(Vector2 newTargetPosition)
	{
		targetPosition = newTargetPosition;
	}

	private void OnPathComplete (Path p)
	{
		if (p.error) return;
		currentPath = p;
		currentWaypoint = 1;
	}

	private void FixedUpdate ()
	{
		if(Vector2.Distance(lastPosition, transform.position) <= 0.1f)
		{
			
			frozenCounter++;
			if(frozenCounter > freezeLimit)
			{
				frozenCounter = 0;
				seeker.StartPath(transform.position, GetRandomPosition(), OnPathComplete);
				
			}
		}
		else
		{
			frozenCounter = 0;
		}

		lastPosition = transform.position;
		if (counter >= rate)
		{
			if (seeker == null)
			{

				seeker = GetComponent<Seeker>();
			}
			counter = 0;
			seeker.StartPath(transform.position, targetPosition, OnPathComplete);
			return;
		}
		counter++;
		
		if (PathIsInvalid()) return;
		UpdatePositionOnPath();
	}

	private Vector2 GetRandomPosition()
	{
		return (Vector2)Random.insideUnitCircle*5+(Vector2)transform.position;
	}

	private void UpdateDirection()
	{
		currentDirection = (currentPath.vectorPath[currentWaypoint] - transform.position).normalized;
		OnNewDirection?.Invoke(currentDirection);
	}

	private void UpdatePositionOnPath()
	{

		
		if (!isCloseEnoughToNextWaypoint()) return;
		if (!thereAreMorePointsOnPath()) return;
		currentWaypoint++;
		UpdateDirection();


	}

	private bool thereAreMorePointsOnPath()
	{
		return currentWaypoint + 1 < currentPath.vectorPath.Count;
	}

	private bool isCloseEnoughToNextWaypoint()
	{
		var distanceToWaypoint = Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
		return (distanceToWaypoint < nextWaypointDistance);
	}

	private bool PathIsInvalid()
	{
		return currentPath == null || currentPath.vectorPath.Count == 0;
	}
}
