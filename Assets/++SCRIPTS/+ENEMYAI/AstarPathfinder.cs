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
			targetPosition = newTargetPosition;

			//Debug.Log($"[{gameObject.name}] SetTargetPosition called - Target: {newTargetPosition}, isPathing: {isPathing}");

			// Start pathfinding immediately instead of waiting for FixedUpdate counter
			if (seeker == null) seeker = GetComponent<Seeker>();
			if (seeker != null)
			{
				//Debug.Log($"[{gameObject.name}] Starting path from {transform.position} to {targetPosition}");
				seeker.StartPath(transform.position, targetPosition, OnPathComplete);
			}
			else
			{
				// TODO: Replace with structured logging
				// Debug.LogError($"[{gameObject.name}] Seeker component is null!");
			}
		}

		private void OnPathComplete(Path p)
		{
			//Debug.Log($"[{gameObject.name}] OnPathComplete called - isPathing: {isPathing}, path error: {p.error}");

			if(!isPathing) return;
			if (p.error)
			{
				//Debug.Log($"[{gameObject.name}] Path error: {p.errorLog}");
				//Debug.Log($"[{gameObject.name}] Not near pathfinding nodes - using direct movement toward target");

				// Use direct movement when not near pathfinding nodes
				WalkInDirectionOfTarget(targetPosition);
				return;
			}

			currentPath = p;
			currentWaypoint = 2;
			//Debug.Log($"[{gameObject.name}] Path complete - {p.vectorPath.Count} waypoints");
			UpdateDirection();
		}



		private void FixedUpdate()
		{
			if (!isPathing)
			{
				// We're in direct movement mode - check if we should switch back to pathfinding
				HandleDirectMovement();
				return;
			}

			// We're in pathfinding mode
			IfFrozen_StartNewPath();
			lastPosition = transform.position;

			// Check if we should try pathfinding periodically
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

		private void HandleDirectMovement()
		{
			// Continue moving toward target
			OnNewDirection?.Invoke((targetPosition - (Vector2)transform.position).normalized);

			// Periodically check if we can switch back to pathfinding
			counter++;
			if (counter >= rate * 2) // Check less frequently than pathfinding mode
			{
				counter = 0;
				if (CanUsePathfinding())
				{
					isPathing = true;
					// Try pathfinding immediately
					if (seeker != null)
					{
						seeker.StartPath(transform.position, targetPosition, OnPathComplete);
					}
				}
			}
		}

		private bool CanUsePathfinding()
		{
			if (AstarPath.active?.data?.gridGraph == null) return false;

			var graph = AstarPath.active.data.gridGraph;
			var nearestNode = graph.GetNearest(transform.position, null).node;

			return nearestNode != null && nearestNode.Walkable;
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
