using System;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using UnityEngine;

public class NPC_AI : MonoBehaviour, IMove
{
	private enum state
	{
		cowering,
		avoiding,
		fleeing
	}

	public event Action<Vector2> OnMoveInDirection;
	public event Action OnStopMoving;
	public Vector2 GetMoveAimDir() => moveDir;
	private Vector2 moveDir;
	public bool IsMoving() => isMoving;
	private bool isMoving;

	private Life _life;
	private Life life => _life ??= GetComponent<Life>();

	private Life cowerTarget;
	private Life avoidTarget;
	private Targetter targetter => _targetter ??= GetComponent<Targetter>();
	private Targetter _targetter;

	private float distanceToAvoidEnemy = 5;
	private float distanceToCowerFromEnemy = 5;
	private state currentState;

	private Body body => _body ??= GetComponent<Body>();
	private Body _body;

	private UnitAnimations _anim;
	private UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();

	public AnimationClip CoweringAnimationClip;
	public AnimationClip RunningAnimationClip;
	public AnimationClip StandingAnimationClip;
	public float speed = 20;

	private void Start()
	{
		SetState(state.cowering);
		life.OnDying += Life_OnDying;


	}

	private void Life_OnDying(Attack obj)
	{
		OnStopMoving?.Invoke();
		this.enabled = false;
	}

	private void SetState(state newState)
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

	private void Update()
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

	private void StartFleeing()
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

	private void UpdateFleeing()
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

	private bool ShouldAvoid()
	{
		avoidTarget = _targetter.GetClosestEnemyInRange(distanceToAvoidEnemy);
		return avoidTarget != null;
	}

	private bool ShouldCower()
	{
		cowerTarget = targetter.GetClosestEnemyInRange(distanceToCowerFromEnemy);
		return cowerTarget != null;
	}

	private void StartCowering()
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

	private void UpdateCowering()
	{
		if (!ShouldCower())
		{
			SetState(ShouldAvoid() ? state.avoiding : state.fleeing);
		}
	}

	private void StartAvoiding()
	{
		if(!ShouldAvoid())
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

	private void UpdateAvoiding()
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
