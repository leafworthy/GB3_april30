using System;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrimsonAI : MonoBehaviour, ICanMoveThings
{
	private static readonly int IsRunning = Animator.StringToHash("IsRunning");

	public GameObject target;

	public float closeEnoughDistance = 5;
	public Vector2 currentDirection;
	private Targetter targets => _targets ??= GetComponent<Targetter>();
	private Targetter _targets;
	private Animator animator => _animator ??= GetComponentInChildren<Animator>();
	private Animator _animator;

	private IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
	private IGetAttacked _life;

	private bool isIdle;
	private float idleTimer;
	private Vector2 currentTargetPosition;

	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;
	public Vector2 GetMoveAimDir() => currentDirection;
	public bool IsMoving() => false;

	private IHaveAttackStats stats => _stats ??= GetComponent<IHaveAttackStats>();
	private IHaveAttackStats _stats;
	public float MoveSpeed => stats.MoveSpeed;

	protected void Start()
	{
		Services.enemyManager.ConfigureNewEnemy(gameObject, EnemySpawner.EnemyType.Crimson);
		StartIdle();
	}


	private void Update()
	{
		if (life.IsDead()) return;
		if (isIdle)
			IdleUpdate();
		else
			RunToPointUpdate();
	}

	private void StartIdle()
	{
		animator.SetBool(IsRunning, false);
		OnStopMoving?.Invoke();
		idleTimer = Random.Range(1f, 5f);
		isIdle = true;
	}

	private void IdleUpdate()
	{
		if (target == null) target = PickARandomTarget();
		idleTimer -= Time.deltaTime;
		if (idleTimer <= 0) StartRunToPoint();
	}

	private void StartRunToPoint()
	{
		target = PickARandomTarget();
		animator.SetBool(IsRunning, true);
		currentTargetPosition = target.transform.position;
		isIdle = false;
	}

	private void RunToPointUpdate()
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

	private GameObject PickARandomTarget()
	{
		var targett = targets.GetClosestPlayer();
		return targett != null ? targets.GetClosestPlayer().transform.gameObject : null;
	}
}
