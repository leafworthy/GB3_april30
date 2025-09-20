using __SCRIPTS;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Ability : SerializedMonoBehaviour, IDoableAbility, INeedPlayer
{
	protected Player player;
	private UnitAnimations _anim;
	protected UnitAnimations anim => _anim ?? GetComponent<UnitAnimations>();
	private Body _body;
	protected Body body => _body ?? GetComponent<Body>();
	private Life _life;
	protected Life life => _life ?? GetComponent<Life>();
	public abstract string AbilityName { get; }

	protected abstract bool requiresArms();

	protected abstract bool requiresLegs();

	public virtual bool canDo() => BodyCanDo(this);

	public virtual bool canStop(IDoableAbility abilityToStopFor) => false;

	public void Do()
	{
		if (!canDo()) return;
		if (requiresArms()) body.doableArms.DoActivity(this);

		if (requiresLegs()) body.doableLegs.DoActivity(this);

		DoAbility();
	}

	protected abstract void DoAbility();

	public virtual void Stop()
	{
		if (requiresArms()) body.doableArms.Stop(this);

		if (requiresLegs()) body.doableLegs.Stop(this);

		CancelInvoke();
	}

	private bool BodyCanDo(IDoableAbility abilityToDo)
	{
		if (Services.pauseManager.IsPaused) return false;
		if (life.IsDead()) return false;
		if (requiresArms() && !body.doableArms.CanDoActivity(abilityToDo)) return false;
		if (requiresLegs() && !body.doableLegs.CanDoActivity(abilityToDo)) return false;

		return true;
	}

	protected virtual void AnimationComplete()
	{
		Stop();
	}

	protected void PlayAnimationClip(string clipName, float length, int layer = 0)
	{
		anim.Play(clipName, layer, 0);
		if (length != 0) Invoke(nameof(AnimationComplete), length);
	}

	protected void PlayAnimationClip(AnimationClip clip, int layer = 0)
	{
		anim.Play(clip.name, layer, 0);
		Invoke(nameof(AnimationComplete), clip.length);
	}

	public virtual void SetPlayer(Player _player)
	{
		player = _player;
	}
}
