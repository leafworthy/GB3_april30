using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class DoableBodyPart
	{
		public Ability CurrentAbility { get; private set; }
		public void Stop(Ability abilityToStop)
		{
			if (CurrentAbility == null) return;
			if (CurrentAbility != abilityToStop) return;
			CurrentAbility = null;
		}

		public void DoAbility(Ability newAbility)
		{
			if (CurrentAbility != null && CurrentAbility.canStop(newAbility)) CurrentAbility.StopAbilityBody();

			CurrentAbility = newAbility;
			Debug.Log("current ability is now: " + CurrentAbility.AbilityName);
			CurrentAbility.TryToDoAbility();
		}



		bool ActivitiesAreTheSame(Ability activity1, Ability activity2) =>
			activity1?.AbilityName == activity2?.AbilityName;

		public bool CanDoActivity(Ability newAbility, bool forceIt = false)
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
