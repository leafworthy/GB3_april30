using System;
using GangstaBean.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class DoableActivityDoer
	{
		public bool isActive =>  currentActivity != null;
		[CanBeNull] public IDoableActivity currentActivity;
		[CanBeNull] public IDoableActivity fallbackActivity;

		public void Stop(IDoableActivity activityToStop)
		{
			if (currentActivity != null)
			{
				if (currentActivity == activityToStop)
				{
					currentActivity.ForceStop();
					fallbackActivity = currentActivity;
					currentActivity = null;
				}
			}
		}
		public bool DoSafely(IDoableActivity newActivity)
		{
			if (CanDoerDo(newActivity))
			{
				fallbackActivity = currentActivity;
				currentActivity = newActivity;

				return currentActivity.Do();
			}
			else
			{
				Debug.Log($"[DoableActivityDoer] BLOCKED: Cannot do {newActivity.VerbName} because " +
				          $"{(currentActivity == null ? "no current activity" : $"current activity is {currentActivity.VerbName}")}");
			}
			return false;
		}

		public bool DoFallback() => DoSafely(fallbackActivity);

		private bool ActivitiesAreTheSame(IDoableActivity activity1, IDoableActivity activity2) =>
			activity1.VerbName == activity2.VerbName;

		public bool CanDoerDo(IDoableActivity newActivity)
		{
			if (newActivity == null) return false;
			if (!newActivity.canDo()) return false;
			if (currentActivity != null)
			{
				if (ActivitiesAreTheSame(newActivity, currentActivity))
				{
					Debug.Log("activities are the same");
					return false;
				}

				if (!currentActivity.canStop()) return false;
			}

			return true;
		}
	}
}
