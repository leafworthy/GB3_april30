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
			Debug.Log($"[Doer] stopping {CurrentAbility.AbilityName}");
			_currentAbility = null;

			if (_bufferedAbility == null) return;
			var next = _bufferedAbility;
			_bufferedAbility = null;
			Debug.Log($"[Doer] buffered ability found: {next.AbilityName}");
			DoActivity(next);
		}

		public void DoActivity(IDoableAbility newAbility)
		{
			if (CanDoActivity(newAbility))
			{
				ActuallyDo(newAbility);
				return;
			}

			BufferAbility(newAbility);
		}

		private void ActuallyDo(IDoableAbility newAbility)
		{
			if (_currentAbility != null && _currentAbility.canStop(newAbility))
			{
				Debug.Log("[Doer] stopping activity " + _currentAbility.AbilityName);
				_currentAbility.Stop();
			}

			_currentAbility = newAbility;
			CurrentAbility.Do();
			Debug.Log("[Doer] activity started: " + newAbility.AbilityName);
		}

		private void BufferAbility(IDoableAbility newAbility)
		{
			if (ActivitiesAreTheSame(newAbility, CurrentAbility))
			{
				Debug.Log($"[Doer] Not buffering {newAbility.AbilityName} (same as current)");
				return;
			}

			if (ActivitiesAreTheSame(newAbility, _bufferedAbility))
			{
				Debug.Log($"[Doer] Not buffering {newAbility.AbilityName} (already buffered)");
				return;
			}

			_bufferedAbility = newAbility;
			Debug.Log($"[Doer] Buffered ability {newAbility.AbilityName} to start after {CurrentAbility.AbilityName}");
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

			if (!IsActive) return true;
			Debug.Log(newAbility.AbilityName + " can stop: " + CurrentAbility.AbilityName + " ? " + CurrentAbility.canStop(newAbility));
			return CurrentAbility.canStop(newAbility);
		}
	}
}
