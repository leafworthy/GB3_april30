using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class DoableBodyPart
	{
		public IDoableAbility CurrentAbility { get; private set; }

		public void Stop(IDoableAbility abilityToStop)
		{
			if (CurrentAbility == null) return;
			if (CurrentAbility != abilityToStop) return;
			CurrentAbility = null;
		}

		public void DoAbility(IDoableAbility newAbility)
		{
			if (CurrentAbility != null && CurrentAbility.canStop(newAbility)) CurrentAbility.StopAbility();

			CurrentAbility = newAbility;
			Debug.Log("current ability is now: " + CurrentAbility.AbilityName);
			CurrentAbility.TryToActivate();
		}

		bool ActivitiesAreTheSame(IDoableAbility activity1, IDoableAbility activity2) =>
			activity1?.AbilityName == activity2?.AbilityName;

		public bool CanDoActivity(IDoableAbility newAbility)
		{
			if (newAbility == null)
			{
				Debug.Log("New Ability is null");
				return false;
			}

			if (ActivitiesAreTheSame(newAbility, CurrentAbility)) return false;

			if (CurrentAbility == null) return true;

			return CurrentAbility.canStop(newAbility);
		}
	}
}
