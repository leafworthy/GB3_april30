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

			if (_bufferedAbility == null) return;
			var next = _bufferedAbility;
			_bufferedAbility = null;
			DoActivity(next);
		}

		public void DoActivity(IDoableAbility newAbility)
		{
			if (CanDoActivity(newAbility))
			{
				ActuallyDo(newAbility);
				return;
			}

			//BufferAbility(newAbility);
		}

		private void ActuallyDo(IDoableAbility newAbility)
		{
			if (_currentAbility != null && _currentAbility.canStop(newAbility)) _currentAbility.Stop();

			_currentAbility = newAbility;
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

		public bool CanDoActivity(IDoableAbility newAbility)
		{
			if (newAbility == null)
			{
				return false;
			}

			if (ActivitiesAreTheSame(newAbility, CurrentAbility))
			{
				return false;
			}

			if (!IsActive) return true;
			return CurrentAbility.canStop(newAbility);
		}
	}
}
