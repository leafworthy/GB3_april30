using System;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonAnimations : MonoBehaviour
{
	private static readonly int IsRunning = Animator.StringToHash("IsRunning");
	private Animator animator => _animator ??= GetComponent<Animator>();
	private Animator _animator;
	public List<GameObject> patrolPoints = new();
	private GameObject target;
	private State state;
	public float currentSpeed;
	public float acceleration = 5;
	public float maxSpeed = 30;
	private float closeEnoughDistance = 5;
	private Vector2 currentDirection;
	private float minimumSpeed = .2f;
	public float deceleration = 10;

	private int currentIndex;

	private enum State
	{
		idle,
		runToPoint
	}

	private void OnEnable()
	{
		state = State.idle;
	}

	private void Update()
	{
		switch (state)
		{
			case State.idle:
				DoIdle();
				break;
			case State.runToPoint:
				DoRunToPoint();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void DoRunToPoint()
	{
		if (target == null) target = PickARandomTarget();
		if (Vector2.Distance(transform.position, target.transform.position) < closeEnoughDistance)
		{
			target = null;
			animator.SetBool(IsRunning, false);
			state = State.idle;
		}
		else
		{
			animator.SetBool(IsRunning, true);
			currentDirection = (target.transform.position - transform.position).normalized;
			transform.position = (Vector2) transform.position + currentDirection * CalculateSpeed();
			transform.localScale = target.transform.position.x < transform.position.x ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
		}
	}

	private float CalculateSpeed()
	{
		return currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);
	}

	private GameObject PickARandomTarget()
	{
		if (patrolPoints.Count == 0) return null;
		var possibleTargets = new List<GameObject>(patrolPoints);
		possibleTargets.Remove(target);
		currentIndex = UnityEngine.Random.Range(0, possibleTargets.Count);
		return possibleTargets[currentIndex];
	}

	private void DoIdle()
	{
		if (currentSpeed > minimumSpeed)
		{
			currentSpeed = Mathf.Clamp(currentSpeed - deceleration * Time.deltaTime, 0, maxSpeed);
		}
		else
		{
			currentSpeed = 0;
		}
		transform.position = (Vector2) transform.position + currentDirection * currentSpeed;

		animator.SetBool(IsRunning, false);
		var someTime = UnityEngine.Random.Range(1, 5f);
		Invoke(nameof(StartRunning), someTime);
	}

	private void StartRunning()
	{
		CancelInvoke(nameof(StartRunning));
		state = State.runToPoint;
	}
}
