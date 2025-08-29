using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public abstract class ServiceAbility : ServiceUser, IDoableAbility, INeedPlayer
{
	protected Player player;
	private UnitAnimations _anim;
	protected UnitAnimations anim => _anim ?? GetComponent<UnitAnimations>();
	private Body _body;
	protected Body body => _body ?? GetComponent<Body>();
	private Life _life;
	protected Life life => _life ?? GetComponent<Life>();
	public virtual string VerbName => "";

	protected abstract bool requiresArms();

	protected abstract bool requiresLegs();

	public virtual bool canDo() => BodyCanDo(this);

	public virtual bool canStop() => false;

	public void Do()
	{
		if (!canDo()) return;
		if (requiresArms())
		{

			body.doableArms.DoActivity(this);
		}
		if (requiresLegs())
		{
			body.doableLegs.DoActivity(this);
		}
		DoAbility();
	}

	protected abstract void DoAbility();

	public virtual void Stop()
	{
		Debug.Log("trying to stop " + VerbName + " ability");
		if (requiresArms())
		{
			body.doableArms.Stop(this);
			Debug.Log("arms stopped for " + VerbName + " ability");
		}
		if (requiresLegs())
		{
			body.doableLegs.Stop(this);
			Debug.Log("legs stopped for " + VerbName + " ability");
		}

		Debug.Log("cancelling invoke for " + VerbName + " ability");
		CancelInvoke(nameof(AnimationComplete));
	}

	private bool BodyCanDo(IDoableAbility abilityToDo)
	{
		if (pauseManager.IsPaused) return false;
		if (life.IsDead())
		{
			Debug.Log( "Dead, cannot do " + VerbName + " ability");
			return false;
		}
		if (requiresArms() && !body.doableArms.CanDoActivity(abilityToDo))
		{
			Debug.Log( "Cannot do arms, cannot do " + VerbName + " ability");
			return false;
		}
		if (requiresLegs() && !body.doableLegs.CanDoActivity(abilityToDo))
		{
			Debug.Log( "Cannot do legs, cannot do " + VerbName + " ability");
			return false;
		}

		return true;
	}

	protected virtual void AnimationComplete()
	{
		Debug.Log("Animation complete for " + VerbName + " ability");
		Stop();
	}

	protected void PlayAnimationClip(AnimationClip clip, int layer = 0)
	{
		Debug.Log("animation starting" + clip.name + " for " + VerbName + " ability " + "length: " + clip.length);
		anim.Play(clip.name, layer, 0);
		Invoke(nameof(AnimationComplete), clip.length);
	}

	public virtual void SetPlayer(Player _player)
	{
		player = _player;
	}
}
