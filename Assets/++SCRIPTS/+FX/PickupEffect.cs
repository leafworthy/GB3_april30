using System;
using _SCRIPTS;
using UnityEngine;

[Serializable]
public class PickupEffect
{
	[SerializeField] public float effectDuration;
	private float timeLeft;
	private bool isRunning;
	public event Action<PickupEffect> OnDone;

	protected PickupEffect(float _effectDuration)
	{
		effectDuration = _effectDuration;
	}
	public virtual void StartEffect(UnitStats stats)
	{
		timeLeft = effectDuration;
		isRunning = true;
	}

	public virtual bool CanUpdateEffect()
	{
		if (!isRunning) return false;
		if (timeLeft <= 0 && isRunning)
		{
			StopEffect();
			return false;
		}
		timeLeft -= Time.deltaTime;
		return true;
	}

	protected virtual void StopEffect()
	{
		OnDone?.Invoke(this);
		timeLeft = 0;
		isRunning = false;
	}
}
