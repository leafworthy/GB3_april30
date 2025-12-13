using System;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using GangstaBean.Core;
using UnityEngine;

public class NPC_AI : MonoBehaviour, ICanMoveThings, ICanAttack, INeedPlayer
{
	enum state
	{
		cowering,
		avoiding,
		fleeing
	}

	public Player player => _player;
	public LayerMask EnemyLayer => Services.assetManager.LevelAssets.EnemyLayer;
	public IHaveAttackStats stats => _stats ??= GetComponent<IHaveAttackStats>();
	IHaveAttackStats _stats;
	public bool IsEnemyOf(IGetAttacked targetLife) => life.IsEnemyOf(this);

	public event Action<IGetAttacked> OnAttack;
	public event Action OnAttackStop;
	public event Action OnAttackStart;

	public void SetPlayer(Player newPlayer)
	{
		_player = newPlayer;
	}

	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;
	public Vector2 GetMoveAimDir() => moveDir;
	Vector2 moveDir;
	public bool IsMoving() => isMoving;
	bool isMoving;

	IGetAttacked _life;
	IGetAttacked life => _life ??= GetComponent<IGetAttacked>();

	IGetAttacked cowerTarget;
	IGetAttacked avoidTarget;
	Targetter targetter => _targetter ??= GetComponent<Targetter>();
	Targetter _targetter;

	float distanceToAvoidEnemy = 5;
	float distanceToCowerFromEnemy = 5;
	state currentState;

	Body body => _body ??= GetComponent<Body>();
	Body _body;

	UnitAnimations _anim;
	UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();

	public AnimationClip CoweringAnimationClip;
	public AnimationClip RunningAnimationClip;
	public AnimationClip StandingAnimationClip;
	public float speed = 20;
	Player _player;
	LayerMask enemyLayer;

	void Start()
	{
		SetState(state.cowering);
		life.OnDead += LifeOnDead;
	}

	void LifeOnDead(Attack obj)
	{
		OnStopMoving?.Invoke();
		enabled = false;
	}

	void SetState(state newState)
	{
		currentState = newState;

		switch (newState)
		{
			case state.cowering:
				StartCowering();
				break;
			case state.avoiding:
				StartAvoiding();
				break;
			case state.fleeing:
				StartFleeing();
				break;
		}
	}

	void Update()
	{
		switch (currentState)
		{
			case state.cowering:
				UpdateCowering();
				break;
			case state.avoiding:
				UpdateAvoiding();
				break;
			case state.fleeing:
				UpdateFleeing();
				break;
		}
	}

	void StartFleeing()
	{
		if (ShouldCower())
		{
			SetState(state.cowering);
			return;
		}

		if (ShouldAvoid())
		{
			SetState(state.avoiding);
			return;
		}

		moveDir = Vector2.left * speed;
		anim.Play(RunningAnimationClip.name, 0, 0);
		OnMoveInDirection?.Invoke(moveDir);
	}

	void UpdateFleeing()
	{
		if (ShouldCower())
		{
			SetState(state.cowering);
			return;
		}

		if (ShouldAvoid())
		{
			SetState(state.avoiding);
			return;
		}

		moveDir = Vector2.left * speed;
		OnMoveInDirection?.Invoke(moveDir);
	}

	bool ShouldAvoid()
	{
		avoidTarget = _targetter.GetClosestEnemyInRange(distanceToAvoidEnemy);
		return avoidTarget != null;
	}

	bool ShouldCower()
	{
		cowerTarget = targetter.GetClosestEnemyInRange(distanceToCowerFromEnemy);
		return cowerTarget != null;
	}

	void StartCowering()
	{
		if (!ShouldCower())
		{
			if (ShouldAvoid())
			{
				SetState(state.avoiding);
				return;
			}

			SetState(state.fleeing);
			return;
		}

		anim.Play(CoweringAnimationClip.name, 0, 0);
		body.BottomFaceDirection(cowerTarget.transform.position.x > transform.position.x);
		OnStopMoving?.Invoke();
	}

	void UpdateCowering()
	{
		if (!ShouldCower()) SetState(ShouldAvoid() ? state.avoiding : state.fleeing);
	}

	void StartAvoiding()
	{
		if (!ShouldAvoid())
		{
			if (ShouldCower())
			{
				SetState(state.cowering);
				return;
			}

			SetState(state.fleeing);
			return;
		}

		anim.Play(RunningAnimationClip.name, 0, 0);
		moveDir = (transform.position - avoidTarget.transform.position).normalized * speed;
		OnMoveInDirection?.Invoke(moveDir);
	}

	void UpdateAvoiding()
	{
		if (!ShouldAvoid())
		{
			if (ShouldCower())
			{
				SetState(state.cowering);
				return;
			}

			SetState(state.fleeing);
			return;
		}

		moveDir = (transform.position - avoidTarget.transform.position).normalized * speed;
		OnMoveInDirection?.Invoke(moveDir);
	}
}
