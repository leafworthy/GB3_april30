using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class DoableBodyPart
	{
		public bool IsActive => CurrentAbility != null;
		public IDoableAbility CurrentAbility => _currentAbility;
		private IDoableAbility _currentAbility;

		public void Stop(IDoableAbility abilityToStop)
		{
			if (CurrentAbility == null) return;
			if (CurrentAbility != abilityToStop) return;
			_currentAbility = null;
		}

		public void DoActivity(IDoableAbility newAbility)
		{
			if (CanDoActivity(newAbility))
			{
				Debug.Log("starting activity " + newAbility.VerbName);
				_currentAbility = newAbility;
				CurrentAbility.Do();
				Debug.Log("[Doer] activity started: " + newAbility.VerbName);
				return;
			}

			Debug.Log($"[Doer] BLOCKED: Cannot do {newAbility.VerbName} because " +
			          $"{(IsActive ? "no current activity" : $"current activity is {CurrentAbility.VerbName}")}");
		}

		private bool ActivitiesAreTheSame(IDoableAbility activity1, IDoableAbility activity2) =>
			activity1?.VerbName == activity2?.VerbName;

		public bool CanDoActivity(IDoableAbility newAbility)
		{
			if (newAbility == null) return false;
			if (ActivitiesAreTheSame(newAbility, CurrentAbility))
			{
				//Debug.Log("Cannot do activity " + newAbility.VerbName + " because it's already the current activity");
				return false;
			}
			if (IsActive)
			{
				Debug.Log( "Cannot do activity " + newAbility.VerbName + " because current activity is " + CurrentAbility.VerbName);
				return CurrentAbility.canStop();
			}
			return true;
		}
	}
}
