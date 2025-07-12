using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class DoableBodyPart
	{
		public bool IsActive => currentActivity != null;
		public IDoableActivity currentActivity;

		public void Stop(IDoableActivity activityToStop)
		{
			if (currentActivity == null) return;
			if (currentActivity != activityToStop) return;
			currentActivity.Stop(activityToStop);
			currentActivity = null;
		}

		public bool UseBodyPart(IDoableActivity newActivity)
		{
			if (currentActivity != null)
			{
				if (!currentActivity.canStop())
					return false;
			}

			if (CanDoActivity(newActivity))
			{
				currentActivity = newActivity;
				currentActivity.Do();
				return true;
			}

			Debug.Log($"[DoableActivityDoer] BLOCKED: Cannot do {newActivity.VerbName} because " +
			          $"{(currentActivity == null ? "no current activity" : $"current activity is {currentActivity.VerbName}")}");

			return false;
		}

		private bool ActivitiesAreTheSame(IDoableActivity activity1, IDoableActivity activity2) =>
			activity1?.VerbName == activity2?.VerbName;

		public bool CanDoActivity(IDoableActivity newActivity)
		{
			if (newActivity == null) return false;
			if (IsActive) return false;
			return !ActivitiesAreTheSame(newActivity, currentActivity);
		}
	}
}
