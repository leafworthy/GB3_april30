using System;
using System.Linq;
using __SCRIPTS;
using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

public class NPC_AI : MonoBehaviour, ICanMoveThings, INeedPlayer, ICanAttack
{
	enum state
	{
		cowering,
		avoiding,
		fleeing,
		rescued,
		hiding
	}

	public Player player => _player;
	public LayerMask EnemyLayer => Services.assetManager.LevelAssets.EnemyLayer;
	public IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
	public bool IsEnemyOf(IGetAttacked targetLife) => player.IsHuman() != targetLife.player.IsHuman();

	public event Action<IGetAttacked> OnAttack;
	IHaveUnitStats _stats;

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
	public AnimationClip HidingAnimationClip;
	public float speed = 20;
	Player _player;
	LayerMask enemyLayer;
	public event Action<NPC_AI> OnRescued;

	JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
	JumpAbility _jumpAbility;

	void Start()
	{
		SetState(state.cowering);
		life.OnDead += LifeOnDead;
		jumpAbility.OnGotBackUp += JumpAbility_OnGotBackUp;
	}

	void JumpAbility_OnGotBackUp()
	{
		if (life.IsDead()) return;
		SetState(state.cowering);
	}

	void LifeOnDead(Attack obj)
	{
		OnStopMoving?.Invoke();
		enabled = false;
	}

	void SetState(state newState)
	{
		if (!enabled) return;
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
			case state.rescued:
				StartRescued();
				break;
			case state.hiding:
				StartHiding();
				break;
		}
	}

	void StartRescued()
	{
		var pos = transform.position;
		var correctedPosition = FindPositionAtEdgeOfScreen(pos);

		Services.risingText.CreateRisingText("Rescued!", correctedPosition, Color.green, 5);
		OnRescued?.Invoke(this);
	}

	Vector2 FindPositionAtEdgeOfScreen(Vector3 pos)
	{
		var screenPoint = CursorManager.GetCamera().WorldToViewportPoint(pos);
		screenPoint.x = .2f;
		screenPoint.y += .05f;

		var worldPoint = CursorManager.GetCamera().ViewportToWorldPoint(screenPoint);
		return new Vector2(worldPoint.x, pos.y);
	}

	void Update()
	{
		if (!enabled) return;
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
			case state.rescued:
				UpdateRescued();
				break;
			case state.hiding:
				UpdateHiding();
				break;
		}
	}

	void UpdateRescued()
	{
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
		Debug.Log("play run");
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

		if (HasRunOffScreen())
		{
			Debug.Log("run off screen");
			SetState(state.rescued);
			return;
		}

		moveDir = Vector2.left * speed;
		var hitsAhead = Physics2D.RaycastAll(transform.position, Vector2.left, 1, Services.assetManager.LevelAssets.BuildingLayer);
		for (var i = 0; i < hitsAhead.Length; i++)
		{
			if (hitsAhead[i].collider != null)
			{
				SetState(state.hiding);
				return;
			}
		}

		OnMoveInDirection?.Invoke(moveDir);
	}

	void StartHiding()
	{
		Debug.Log("start hiding");
		anim.Play(HidingAnimationClip.name, 0, 0);
		body.BottomFaceDirection(true);
		OnStopMoving?.Invoke();
	}

	void UpdateHiding()
	{
		if (!HasRunOffScreen())
		{

			var hitsAhead = Physics2D.RaycastAll(transform.position, Vector2.left, 1, Services.assetManager.LevelAssets.BuildingLayer);
			if (hitsAhead.Any(t => t.collider != null)) return;
			moveDir = Vector2.left * speed;
			Debug.Log("start fleeing AGAIN");
			SetState(state.fleeing);
			return;
		}
		SetState(state.rescued);
	}

	bool HasRunOffScreen()
	{
		var screenPoint = CursorManager.GetCamera().WorldToViewportPoint(transform.position);
		return screenPoint.x < 0;
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
