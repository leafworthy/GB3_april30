using __SCRIPTS;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Ability : SerializedMonoBehaviour, IDoableAbility, INeedPlayer
{
	protected Player player;
	protected UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
	UnitAnimations _anim;
	protected Body body => _body ??= GetComponent<Body>();
	Body _body;
	protected IGetAttacked defence => _defence ??= GetComponent<IGetAttacked>();
	IGetAttacked _defence;
	protected ICanAttack offence => _attack ??= GetComponent<ICanAttack>();
	ICanAttack _attack;

	protected IDoableAbility lastLegAbility;
	protected IDoableAbility lastArmAbility;
	public abstract string AbilityName { get; }

	protected abstract bool requiresArms();
	protected abstract bool requiresLegs();

	public virtual bool canDo() => BodyCanDo(this);
	public virtual bool canStop(IDoableAbility abilityToStopFor) => false;

	public void TryToActivate()
	{
		if (!canDo())
		{
			Debug.Log("cant do " + AbilityName + " because body cant do it", this);
			return;
		}
		lastLegAbility = body.doableLegs.CurrentAbility;
		lastArmAbility = body.doableArms.CurrentAbility;
		if (requiresArms()) body.doableArms.DoAbility(this);
		if (requiresLegs()) body.doableLegs.DoAbility(this);

		DoAbility();
	}

	protected abstract void DoAbility();

	public virtual void StopAbility()
	{
		StopBody();
	}

	protected void StopBody()
	{
		if (requiresArms()) body.doableArms.Stop(this);
		if (requiresLegs()) body.doableLegs.Stop(this);

		CancelInvoke();
	}

	public virtual void Resume()
	{
		TryToActivate();
	}

	bool BodyCanDo(IDoableAbility abilityToDo)
	{
		if (Services.pauseManager.IsPaused) return false;
		if (defence.IsDead()) return false;

		if (requiresArms() && !body.doableArms.CanDoActivity(abilityToDo))
		{
			Debug.Log("cant do " + AbilityName + " because arms cant do it", this);
			return false;
		}
		if (requiresLegs() && !body.doableLegs.CanDoActivity(abilityToDo))
		{
			Debug.Log("cant do " + AbilityName + " because legs cant do it", this);
			return false;
		}

		return true;
	}

	protected virtual void AnimationComplete()
	{
		StopAbility();
	}

	protected void PlayAnimationClip(string clipName, float length, int layer = 0)
	{
		anim.Play(clipName, layer, 0);
		if (length != 0) Invoke(nameof(AnimationComplete), length);
	}

	protected void PlayAnimationClipWithoutEvent(string clipName, int layer = 0)
	{
		anim.Play(clipName, layer, 0);
	}

	protected void PlayAnimationClip(AnimationClip clip, int layer = 0)
	{
		PlayAnimationClip(clip.name, clip.length, layer);
	}

	public virtual void SetPlayer(Player newPlayer)
	{
		player = newPlayer;
	}
}
