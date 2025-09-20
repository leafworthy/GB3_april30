using System.Collections.Generic;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using UnityEngine;

public class CrimsonAnimations : StateMachine<CrimsonAnimations>
{
	private static readonly int IsRunning = Animator.StringToHash("IsRunning");

	public Animator animator => _animator ??= GetComponentInChildren<Animator>();
	private Animator _animator;
	public List<GameObject> patrolPoints = new();
	public GameObject target;
	public float currentSpeed;
	public float acceleration = 5;
	public float maxSpeed = 30;
	public float closeEnoughDistance = 5;
	public Vector2 currentDirection;
	public float minimumSpeed = .2f;
	public float deceleration = 10;
	private Targetter targets => _targets ??= GetComponent<Targetter>();
	private Targetter _targets;
	private IdleState idleState;
	private RunToPointState runToPointState;
	private static PauseManager _pauseManager;
	private static PauseManager pauseManager => _pauseManager ??= ServiceLocator.Get<PauseManager>();

	protected override void Start()
	{
		base.Start();

		var enemyManager = ServiceLocator.Get<EnemyManager>();
		enemyManager.ConfigureNewEnemy(gameObject);

		idleState = new IdleState();
		runToPointState = new RunToPointState();
		SwitchState(idleState);
	}

	private void OnEnable()
	{
		if (idleState != null)
			SwitchState(idleState);
	}

	public float CalculateSpeed()
	{
		return currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, maxSpeed);
	}

	public GameObject PickARandomTarget()
	{
		var targett = targets.GetClosestPlayer();
		if (targett != null) return targets.GetClosestPlayer().gameObject;
		if (patrolPoints.Count == 0) return null;

		var randomIndex = Random.Range(0, patrolPoints.Count);
		for (var i = 0; i < 6; i++)
		{
			if (patrolPoints[randomIndex] != null && patrolPoints[randomIndex] != target) return patrolPoints[randomIndex];

		}

		return null;
	}

	private class IdleState : State<CrimsonAnimations>
	{
		private float idleTimer;

		public override void Enter(CrimsonAnimations machine)
		{
			machine.animator.SetBool(IsRunning, false);
			idleTimer = Random.Range(1f, 5f);
		}

		public override void Update(CrimsonAnimations machine)
		{
			if (machine.currentSpeed > machine.minimumSpeed)
				machine.currentSpeed = Mathf.Clamp(machine.currentSpeed - machine.deceleration * Time.deltaTime, 0, machine.maxSpeed);
			else
				machine.currentSpeed = 0;
			machine.transform.position = (Vector2) machine.transform.position + machine.currentDirection * machine.currentSpeed;
			if (machine.target == null) machine.target = machine.PickARandomTarget();
			machine.transform.localScale = machine.target.transform.position.x < machine.transform.position.x ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

			idleTimer -= Time.deltaTime;
			if (idleTimer <= 0) SwitchState(machine, machine.runToPointState);
		}

		public override void Exit(CrimsonAnimations machine)
		{
		}
	}

	private class RunToPointState : State<CrimsonAnimations>
	{
		private Vector3 currentTargetPosition;

		public override void Enter(CrimsonAnimations machine)
		{
			machine.target = machine.PickARandomTarget();
			machine.animator.SetBool(IsRunning, true);
			currentTargetPosition = machine.target.transform.position;
		}

		public override void Update(CrimsonAnimations machine)
		{
			if (pauseManager.IsPaused) return;

			if (Vector2.Distance(machine.transform.position, currentTargetPosition) < machine.closeEnoughDistance)
				SwitchState(machine, machine.idleState);
			else
			{
				machine.currentDirection = (currentTargetPosition - machine.transform.position).normalized;
				machine.transform.position = (Vector2) machine.transform.position + machine.currentDirection * machine.CalculateSpeed();
				machine.transform.localScale = currentTargetPosition.x < machine.transform.position.x ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
			}
		}

		public override void Exit(CrimsonAnimations machine)
		{
			machine.animator.SetBool(IsRunning, false);
		}
	}
}
