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

		public bool CanDoActivity(IDoableAbility newAbility, bool forceIt = false)
		{
			if (newAbility == null)
			{
				Debug.Log("New Ability is null");
				return false;
			}

			if (ActivitiesAreTheSame(newAbility, CurrentAbility))
			{
				Debug.Log("New Ability: " + newAbility.AbilityName + " is the same as current ability: " + CurrentAbility.AbilityName);
				return false;
			}

			if (CurrentAbility != null) return true;
			if (forceIt)
			{
				Debug.Log("forced it in body part");
				return true;
			}

			Debug.Log("Can Stop: " + CurrentAbility.AbilityName + " " + CurrentAbility.canStop(newAbility));
			return CurrentAbility.canStop(newAbility);
		}
	}
}
