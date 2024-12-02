using System;
using UnityEngine;

public interface IEnemyState
{
	void OnEnterState(EnemyAI _ai);
	void OnExitState();
	void UpdateState();
}

public class EnemyAI : MonoBehaviour
{
	private IEnemyState currentState;
	private Vector2 wanderPoint;

	public float idleCoolDownMax = 2;
	public float WanderRadius = 20;

	public AstarPathfinder Pathmaker => GetComponent<AstarPathfinder>();
	public Life Life => GetComponent<Life>();
	public Targetter Targets => GetComponent<Targetter>();
	public EnemyThoughts Thoughts => GetComponent<EnemyThoughts>();
	public Vector2 WanderPoint
	{
		get => wanderPoint;
		private set => wanderPoint = value;
	}
	

	public event Action<Life> OnAttack;
	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;

	private void Start()
	{
		WanderPoint = transform.position;
		Pathmaker.OnNewDirection += (direction) => OnMoveInDirection?.Invoke(direction);
		Pathmaker.OnFreeze += Pathmaker_Frozen;
		TransitionToState(new IdleState()); // Start in Idle State
	}

	private void Pathmaker_Frozen()
	{
		Debug.Log("Pathmaker frozen");
		TransitionToState(new IdleState());
	}

	private void FixedUpdate()
	{
		if (GlobalManager.IsPaused || Life.IsDead()) return;

		currentState?.UpdateState();
	}

	public void TransitionToState(IEnemyState newState)
	{
		currentState?.OnExitState();
		currentState = newState;
		currentState.OnEnterState(this);
	}


	public bool FoundTargetInAggroRange()
	{
		var target = Targets.GetClosestPlayerInAggroRange();
		return target != null;
	}


	public void StopMoving()
	{
		OnStopMoving?.Invoke();
		Pathmaker.StopPathing();
	}

	public void Attack(Life targetsCurrentTarget)
	{
		Debug.Log("attacking" + targetsCurrentTarget, targetsCurrentTarget);
		Debug.DrawLine( transform.position, targetsCurrentTarget.transform.position, Color.red, 1);
		StopMoving();
		OnAttack?.Invoke(targetsCurrentTarget);
	}
}