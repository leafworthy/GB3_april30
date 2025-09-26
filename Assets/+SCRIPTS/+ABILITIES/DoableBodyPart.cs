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
		private IDoableAbility _bufferedAbility;

		public void Stop(IDoableAbility abilityToStop)
		{
			if (CurrentAbility == null) return;
			if (CurrentAbility != abilityToStop) return;
			_currentAbility = null;
		}

		public void DoActivity(IDoableAbility newAbility)
		{
			ActuallyDo(newAbility);
		}

		private void ActuallyDo(IDoableAbility newAbility)
		{
			if (_currentAbility != null && _currentAbility.canStop(newAbility)) _currentAbility.Stop();

			_currentAbility = newAbility;
			Debug.Log("current ability is now: " + CurrentAbility.AbilityName);
			CurrentAbility.Do();
		}

		private void BufferAbility(IDoableAbility newAbility)
		{
			if (ActivitiesAreTheSame(newAbility, CurrentAbility)) return;

			if (ActivitiesAreTheSame(newAbility, _bufferedAbility)) return;

			_bufferedAbility = newAbility;
		}

		private bool ActivitiesAreTheSame(IDoableAbility activity1, IDoableAbility activity2) =>
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

			if (!IsActive) return true;
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
