using System;
using JetBrains.Annotations;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class Arms : ActivityHandler
	{
	}

	[Serializable]
	public class Legs : ActivityHandler
	{
	}

	[Serializable]
	public class ActivityHandler
	{
		[CanBeNull] public IActivity currentActivity;
		public bool isActive;
		public string ActivityStory = "Activity Story Start";


		public bool Do(IActivity activity)
		{
			if (isActive)
			{
				if (currentActivity?.VerbName == activity.VerbName)
				{
					ActivityStory += "\nTried to start doing " + activity.VerbName + " but already doing it";
					return false;
				}

				ActivityStory += "\n" +
				                 $"Tried to do {activity.VerbName} but current activity is {currentActivity?.VerbName}" +
				                 $" on {this} with {currentActivity?.VerbName} and is active: {isActive}";
				return false;
			}

			ActivityStory += "\nStarted doing " + activity.VerbName + " with last activity: " +
			                 currentActivity?.VerbName;
			isActive = true;
			currentActivity = activity;

			return true;
		}

		public bool StopCurrentActivity()
		{
			if (!isActive) return false;

			ActivityStory += "\nStopped doing " + currentActivity;
			isActive = false;
			currentActivity = null;

			return true;
		}

		public void StopSafely(IActivity activity)
		{
			if (currentActivity?.VerbName != activity.VerbName)
			{
				ActivityStory += "\n Tried to stop " + activity.VerbName + " but current activity is " +
				                 currentActivity?.VerbName;
				return;
			}

			if (!isActive)
			{
				ActivityStory += "\n Tried to stop " + activity.VerbName + " but not currently active";
				return;
			}

			ActivityStory += "\n Stopped doing " + activity.VerbName + " with last activity: " +
			                 currentActivity?.VerbName;
			isActive = false;
			currentActivity = null;

		}
	}

}
