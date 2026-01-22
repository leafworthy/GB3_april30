using System;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using GangstaBean.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrimsonAI : MonoBehaviour, ICanMoveThings
{
	static readonly int IsRunning = Animator.StringToHash("IsRunning");

	public GameObject target;

	public float closeEnoughDistance = 5;
	public Vector2 currentDirection;
	Targetter targets => _targets ??= GetComponent<Targetter>();
	Targetter _targets;
	Animator animator => _animator ??= GetComponentInChildren<Animator>();
	Animator _animator;

	Life life => _life ??= GetComponent<Life>();
	Life _life;

	bool isIdle;
	float idleTimer;
	Vector2 currentTargetPosition;

	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;
	public Vector2 GetMoveAimDir() => currentDirection;
	public bool IsMoving() => false;

	IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
	IHaveUnitStats _stats;
	public float MoveSpeed => stats.Stats.MoveSpeed;

	protected void Start()
	{
		Services.enemyManager.ConfigureNewEnemy(gameObject, EnemySpawner.EnemyType.Crimson);
		StartIdle();
	}

	void Update()
	{
		if (life.IsDead()) return;
		if (isIdle)
			IdleUpdate();
		else
			RunToPointUpdate();
	}

	void StartIdle()
	{
		animator.SetBool(IsRunning, false);
		OnStopMoving?.Invoke();
		idleTimer = Random.Range(1f, 5f);
		isIdle = true;
	}

	void IdleUpdate()
	{
		if (target == null) target = PickARandomTarget();
		idleTimer -= Time.deltaTime;
		if (idleTimer <= 0) StartRunToPoint();
	}

	void StartRunToPoint()
	{
		target = PickARandomTarget();
		animator.SetBool(IsRunning, true);
		currentTargetPosition = target.transform.position;
		isIdle = false;
	}

	void RunToPointUpdate()
	{
		if (Services.pauseManager.IsPaused) return;

		if (Vector2.Distance(transform.position, currentTargetPosition) < closeEnoughDistance)
		{
			animator.SetBool(IsRunning, false);
			StartIdle();
		}
		else
		{
			currentDirection = (currentTargetPosition - (Vector2) transform.position).normalized;
			OnMoveInDirection?.Invoke(currentDirection);
		}
	}

	GameObject PickARandomTarget()
	{
		var targett = targets.GetClosestPlayer();
		return targett != null ? targets.GetClosestPlayer().transform.gameObject : null;
	}
}
