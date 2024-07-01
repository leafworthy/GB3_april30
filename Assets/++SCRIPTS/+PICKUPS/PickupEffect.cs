using System;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._PICKUPS
{
	[Serializable]
	public class PickupEffect
	{
		[SerializeField] public float effectDuration;
		private float timeLeft;
		protected bool isRunning;
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

		public virtual void UpdateEffect()
		{
			if (timeLeft <= 0 && isRunning)
			{
				StopEffect();
			}
			else
			{
				timeLeft -= Time.deltaTime;
			}
		}

		protected virtual void StopEffect()
		{
			OnDone?.Invoke(this);
			timeLeft = 0;
			isRunning = false;
		}
	}
}
