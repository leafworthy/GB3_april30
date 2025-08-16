using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;


public abstract class AbilityController : MonoBehaviour, INeedPlayer
{
	protected UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
	private UnitAnimations _anim;
	protected MoveAbility move => _move ??= GetComponent<MoveAbility>();
	private MoveAbility _move;
	protected AnimationEvents animEvents => _animEvents??= _anim.animEvents;
	private AnimationEvents _animEvents;
	protected Body body => _body ??= GetComponent<Body>();
	private Body _body;
	protected Life life => _life ??= GetComponent<Life>();
	private Life _life;
	protected Player player;

	protected PauseManager pauseManager  => _pauseManager ??= ServiceLocator.Get<PauseManager>();

	private PauseManager _pauseManager;

	public virtual void SetPlayer(Player _player)
	{
		player = _player;
		StopListeningToPlayer();
		ListenToPlayer();
	}

	protected abstract void StopListeningToPlayer();
	protected abstract void ListenToPlayer();

	protected virtual void OnDisable()
	{
		StopListeningToPlayer();
	}

	protected virtual void OnDestroy()
	{
		StopListeningToPlayer();
	}
}
