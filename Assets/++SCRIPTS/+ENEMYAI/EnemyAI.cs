using System;
using UnityEngine;

public interface IEnemyState
{
    void OnEnterState(EnemyAI ai);
    void OnExitState();
    void UpdateState();
}

public class EnemyAI : MonoBehaviour
{
    public bool stopMovingOnAttack = true;
    private IEnemyState currentState;
    private Vector2 wanderPoint;

    public float idleCoolDownMax = 2f;
    public float WanderRadius = 50f;
    public bool BornOnAggro;

    // Cached component references to avoid per-frame GetComponent calls.
    private AstarPathfinder _pathmaker;
    private Life _life;
    private Targetter _targets;
    private EnemyThoughts _thoughts;
    private Animator _animator;

    // Exposed read-only properties using cached values.
    public AstarPathfinder Pathmaker => _pathmaker;
    public Life Life => _life;
    public Targetter Targets => _targets;
    public EnemyThoughts Thoughts => _thoughts;
    public Animator AnimatorComponent => _animator;

    public Vector2 WanderPoint
    {
        get => wanderPoint;
        private set => wanderPoint = value;
    }

    public event Action<Life> OnAttack;
    public event Action<Vector2> OnMoveInDirection;
    public event Action OnStopMoving;

    // Cache the delegate to avoid allocating a lambda each time.
    private Action<Vector2> onNewDirectionHandler;

    private void Awake()
    {
        // Cache component references once.
        _pathmaker = GetComponent<AstarPathfinder>();
        _life = GetComponent<Life>();
        _targets = GetComponent<Targetter>();
        _thoughts = GetComponent<EnemyThoughts>();
        _animator = GetComponentInChildren<Animator>();

        // Initialize wander point.
        wanderPoint = transform.position;
    }

    private void Start()
    {
        // Subscribe to the pathmaker's new direction event using a cached delegate.
        if (_pathmaker != null)
        {
            onNewDirectionHandler = HandleNewDirection;
            _pathmaker.OnNewDirection += onNewDirectionHandler;
        }

        // Start in an initial state (adjust as necessary).
        TransitionToState(new AggroState());
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks.
        if (_pathmaker != null && onNewDirectionHandler != null)
        {
            _pathmaker.OnNewDirection -= onNewDirectionHandler;
        }
    }

    private void FixedUpdate()
    {
        // Early-out if paused or dead.
        if (GlobalManager.IsPaused || Life.IsDead())
            return;

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
        if (!stopMovingOnAttack) return;
        OnStopMoving?.Invoke();
        Pathmaker.StopPathing();
    }

    public void Attack(Life targetsCurrentTarget)
    {
        Debug.DrawLine(transform.position, targetsCurrentTarget.transform.position, Color.red, 1f);
        StopMoving();
        OnAttack?.Invoke(targetsCurrentTarget);
    }

    public void OnAggro()
    {
        if (BornOnAggro)
        {
            // Use a constant (or cached) trigger name to avoid allocating strings.
            AnimatorComponent.SetTrigger(Animations.AggroTrigger);
        }
    }

    public void MoveWithoutPathing(Vector2 randomDirection)
    {
        OnMoveInDirection?.Invoke(randomDirection);
    }

    // Cached event handler for new direction events.
    private void HandleNewDirection(Vector2 direction)
    {
        OnMoveInDirection?.Invoke(direction);
    }
}
