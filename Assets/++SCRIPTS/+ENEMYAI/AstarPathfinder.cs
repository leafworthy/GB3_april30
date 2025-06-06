using System;
using __SCRIPTS.Plugins.AstarPathfindingProject.Core;
using __SCRIPTS.Plugins.AstarPathfindingProject.Core.AI;
using UnityEngine;
using GangstaBean.Core;
using Random = UnityEngine.Random;

namespace __SCRIPTS._ENEMYAI
{
	public class AstarPathfinder : MonoBehaviour, IPoolable
	{
		public event Action<Vector2> OnNewDirection;
		private Vector2 targetPosition;
		private Seeker seeker;
		private Path currentPath;
		private Vector3 currentDirection;

		private int rate = 20;
		private int counter;

		private float nextWaypointDistance = 2;
		private int currentWaypoint;
		private Vector2 lastPosition;
		private int frozenCounter;
		private int freezeLimit = 400;
		private bool isPathing;
		public event Action OnFreeze;

		private void OnEnable()
		{
			InitializePathfinder();
		}

		private void InitializePathfinder()
		{
			seeker = GetComponent<Seeker>();
			currentPath = null;
			var aStar = FindFirstObjectByType<AstarPath>();
			if (aStar is null) enabled = false;
			isPathing = true;
			lastPosition = transform.position;
			counter = 0;
			frozenCounter = 0;
			currentWaypoint = 0;
		}

		public void SetTargetPosition(Vector2 newTargetPosition)
		{
			isPathing = true;
			//seeker.StartPath(transform.position, targetPosition, OnPathComplete);
			targetPosition = newTargetPosition;
		}

		private void OnPathComplete(Path p)
		{
			if(!isPathing) return;
			if (p.error) return;

			currentPath = p;
			currentWaypoint = 2;
			UpdateDirection();
		}



		private void FixedUpdate()
		{
			if(!isPathing) return;
			IfFrozen_StartNewPath();
			lastPosition = transform.position;
			if (counter >= rate)
			{
				if (seeker == null) seeker = GetComponent<Seeker>();
				if (seeker == null) return;
				counter = 0;
				seeker.StartPath(transform.position, targetPosition, OnPathComplete);
				return;
			}

			counter++;

			if (PathIsInvalid()) return;
			UpdatePositionOnPath();
		}

		private void IfFrozen_StartNewPath()
		{
			if (Vector2.Distance(lastPosition, transform.position) <= 0.1f)
			{
				frozenCounter++;
				if (frozenCounter <= freezeLimit) return;
				frozenCounter = 0;
				OnFreeze?.Invoke();
			}
			else
				frozenCounter = 0;
		}

		private Vector2 GetRandomPosition() => Random.insideUnitCircle * 5 + (Vector2) transform.position;

		private void UpdateDirection()
		{
			if (currentPath.vectorPath.Count <= currentWaypoint) return;
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

		private bool thereAreMorePointsOnPath() => currentWaypoint + 1 < currentPath.vectorPath.Count;

		private bool isCloseEnoughToNextWaypoint()
		{
			if(currentPath is null) return false;
			if(currentPath.vectorPath.Count == 0) return false;
			if (currentPath.vectorPath.Count <= currentWaypoint) return false;
			var distanceToWaypoint = Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]);
			return distanceToWaypoint < nextWaypointDistance;
		}

		private bool PathIsInvalid() => currentPath == null || currentPath.vectorPath.Count == 0 ||
		                                (currentPath.vectorPath.Count <= currentWaypoint);

		public void StopPathing()
		{
			isPathing = false;
			targetPosition = transform.position;
		}

		public void WalkInDirectionOfTarget(Vector3 target)
		{
			isPathing = false;
			OnNewDirection?.Invoke((target - transform.position).normalized);
		}

		public void OnPoolSpawn()
		{
			// Reinitialize pathfinder when spawned from pool
			InitializePathfinder();
		}

		public void OnPoolDespawn()
		{
			// Clean up when returning to pool
			StopPathing();
			currentPath = null;
		}
	}
}
