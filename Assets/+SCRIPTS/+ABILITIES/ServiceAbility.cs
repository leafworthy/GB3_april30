using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public abstract class ServiceAbility : ServiceUser, IDoableActivity
{
	private UnitAnimations _anim;
	protected UnitAnimations anim => _anim ?? GetComponent<UnitAnimations>();
	private Body _body;
	protected Body body => _body ?? GetComponent<Body>();
	public virtual string VerbName => "Generic Ability";

	public virtual bool requiresArms => false;

	public virtual bool requiresLegs => false;

	public virtual bool canDo() => BodyCanDo(this);

	public virtual bool canStop() => false;

	public abstract void Do();

	public virtual void Stop(IDoableActivity activityToStop)
	{
		if (requiresArms) body.doableArms.Stop(activityToStop);
		if (requiresLegs) body.doableLegs.Stop(activityToStop);
		CancelInvoke(nameof(AnimationComplete));
	}

	protected bool BodyCanDo(IDoableActivity activityToDo)
	{
		if (requiresArms && !body.doableArms.CanDoActivity(activityToDo)) return false;
		if (requiresLegs && !body.doableLegs.CanDoActivity(activityToDo)) return false;
		return true;
	}

	protected virtual void AnimationComplete()
	{
	}

	protected void PlayAnimationClip(AnimationClip clip)
	{
		anim.Play(clip.name, 0, 0);
		Invoke(nameof(AnimationComplete), clip.length);
	}
}
