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
				if (_currentAbility != null && _currentAbility.canStop()) _currentAbility.Stop();
				Debug.Log("starting activity " + newAbility.AbilityName);
				_currentAbility = newAbility;
				CurrentAbility.Do();
				Debug.Log("[Doer] activity started: " + newAbility.AbilityName);
				return;
			}

			Debug.Log($"[Doer] BLOCKED: Cannot do {newAbility.AbilityName} because " +
			          $"{(IsActive ? "no current activity" : $"current activity is {CurrentAbility.AbilityName}")}");
		}

		private bool ActivitiesAreTheSame(IDoableAbility activity1, IDoableAbility activity2) =>
			activity1?.AbilityName == activity2?.AbilityName;

		public bool CanDoActivity(IDoableAbility newAbility)
		{
			if (newAbility == null)
			{
				Debug.Log("Cannot do activity because it's null");
				return false;
			}
			if (ActivitiesAreTheSame(newAbility, CurrentAbility))
			{
				Debug.Log("Cannot do activity " + newAbility.AbilityName + " because it's already the current activity");
				return false;
			}

			if (IsActive)
			{
				Debug.Log(newAbility.AbilityName +" can stop: " + CurrentAbility.AbilityName + " ? " + CurrentAbility.canStop());
				return CurrentAbility.canStop();
			}

			return true;
		}
	}
}
