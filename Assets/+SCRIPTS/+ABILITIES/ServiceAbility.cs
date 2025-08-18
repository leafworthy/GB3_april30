using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public abstract class ServiceAbility : MonoBehaviour, IDoableActivity
{
	protected UnitAnimations anim => _anim ?? GetComponent<UnitAnimations>();
	private UnitAnimations _anim;
	protected Body body => _body ?? GetComponent<Body>();
	private Body _body;
	protected Life life => _life ??= GetComponent<Life>();
	private Life _life;
	protected PauseManager pauseManager => _pauseManager ??= ServiceLocator.Get<PauseManager>();
	private PauseManager _pauseManager;
	protected AssetManager assetManager => _assetManager ??= ServiceLocator.Get<AssetManager>();
	private AssetManager _assetManager;

	protected DoableMoveController moveController => _moveController ??= GetComponent<DoableMoveController>();
	private DoableMoveController _moveController;
	protected MoveAbility move => _move ??= GetComponent<MoveAbility>();
	private MoveAbility _move;

	protected Player _player;
	public virtual string VerbName => "Generic Ability";

	protected abstract bool requiresArms();

	protected abstract bool requiresLegs();

	public virtual bool canDo() => BodyCanDo(this);

	public virtual bool canStop() => false;

	public void DoSafely()
	{
		if (requiresArms()) body.doableArms.DoActivity(this);
		if (requiresLegs()) body.doableLegs.DoActivity(this);
	}

	public abstract void StartActivity();

	public virtual void StopActivity()
	{
		if (requiresArms()) body.doableArms.StopActivity(this);
		if (requiresLegs()) body.doableLegs.StopActivity(this);
		CancelInvoke(nameof(AnimationComplete));
	}

	private bool BodyCanDo(IDoableActivity activityToDo)
	{
		if (pauseManager.IsPaused) return false;
		if (life.IsDead()) return false;
		if (requiresArms() && !body.doableArms.CanDoActivity(activityToDo)) return false;
		if (requiresLegs() && !body.doableLegs.CanDoActivity(activityToDo)) return false;
		return true;
	}

	protected virtual void AnimationComplete()
	{
		StopActivity();
	}

	protected void PlayAnimationClip(string clipName, int layer = 0)
	{
		var clips = anim.animator.runtimeAnimatorController.animationClips;
		AnimationClip desiredClip = null;
		foreach (var clip in clips)
		{
			if (clip.name != clipName) continue;
			desiredClip = clip;
			break;
		}

		PlayAnimationClip(desiredClip, layer);
	}

	protected void PlayAnimationClip(AnimationClip clip, int layer = 0)
	{
		if (clip == null) Debug.LogWarning("[ServiceAbility] Animation clip not found: " + clip.name);
		anim.Play(clip.name, layer, 0);
		Invoke(nameof(AnimationComplete), clip.length);
	}
}
