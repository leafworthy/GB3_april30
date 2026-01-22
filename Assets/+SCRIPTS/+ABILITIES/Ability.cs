using __SCRIPTS;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Ability : SerializedMonoBehaviour, INeedPlayer
{
	protected Player player;
	protected UnitAnimations anim => _anim ??= GetComponent<UnitAnimations>();
	UnitAnimations _anim;
	protected Body body => _body ??= GetComponent<Body>();
	Body _body;
	protected Life life => _life ??= GetComponent<Life>();
	Life _life;
	protected ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>();
	ICanAttack _attacker;

	protected Ability lastLegAbility;
	protected Ability lastArmAbility;
	public abstract string AbilityName { get; }

	public abstract bool requiresArms();
	public abstract bool requiresLegs();

	public virtual bool canDo() => BodyCanDo(this);
	public virtual bool canStop(Ability abilityToStopFor) => false;

	public void TryToDoAbility()
	{
		if (!canDo()) return;
		lastLegAbility = body.doableLegs.CurrentAbility;
		lastArmAbility = body.doableArms.CurrentAbility;
		body.DoAbility(this);
		DoAbility();
	}

	protected abstract void DoAbility();

	public virtual void StopAbilityBody()
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
		TryToDoAbility();
	}

	bool BodyCanDo(Ability abilityToDo)
	{
		if (Services.pauseManager.IsPaused) return false;
		if (life.IsDead()) return false;
		if (body.CanDoAbility(abilityToDo)) return false;

		return true;
	}

	protected virtual void AnimationComplete()
	{
		StopAbilityBody();
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
