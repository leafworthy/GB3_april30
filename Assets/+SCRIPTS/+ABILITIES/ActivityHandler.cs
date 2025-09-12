using System;
using JetBrains.Annotations;
using UnityEngine;
using GangstaBean.Core;

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
		[CanBeNull] public GangstaBean.Core.IActivity currentActivity;
		public bool isActive;
		public string ActivityStory = "Activity Story Start";

		[NonSerialized]
		private MonoBehaviour parentMonoBehaviour;

		public void SetParent(MonoBehaviour parent)
		{
			parentMonoBehaviour = parent;
		}

		public bool Do(GangstaBean.Core.IActivity activity)
		{
			// Check if character is dead
			if (parentMonoBehaviour != null)
			{
				var life = parentMonoBehaviour.GetComponent<Life>();
				if (life != null && life.IsDead())
				{
					ActivityStory += $"\nTried to do {activity.AbilityName} but character is dead";
					Debug.Log($"[ActivityHandler] BLOCKED: Character is dead, cannot {activity.AbilityName}");
					return false;
				}
			}

			if (isActive)
			{
				if (currentActivity?.AbilityName == activity.AbilityName)
				{
					ActivityStory += "\nTried to start doing " + activity.AbilityName + " but already doing it";
					Debug.Log($"[ActivityHandler] BLOCKED: Already doing {activity.AbilityName}");
					return false;
				}

				ActivityStory += "\n" +
				                 $"Tried to do {activity.AbilityName} but current activity is {currentActivity?.AbilityName}" +
				                 $" on {this} with {currentActivity?.AbilityName} and is active: {isActive}";
				Debug.Log($"[ActivityHandler] BLOCKED: Tried {activity.AbilityName} but busy with {currentActivity?.AbilityName}");
				return false;
			}

			ActivityStory += "\nStarted doing " + activity.AbilityName + " with last activity: " +
			                 currentActivity?.AbilityName;
			Debug.Log($"[ActivityHandler] SUCCESS: Started {activity.AbilityName}");
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

		public void Stop(GangstaBean.Core.IActivity activity)
		{
			if (currentActivity?.AbilityName != activity.AbilityName)
			{
				ActivityStory += "\n Tried to stop " + activity.AbilityName + " but current activity is " +
				                 currentActivity?.AbilityName;
				Debug.Log($"[ActivityHandler] StopSafely FAILED: Tried to stop {activity.AbilityName} but current is {currentActivity?.AbilityName}");
				return;
			}

			if (!isActive)
			{
				ActivityStory += "\n Tried to stop " + activity.AbilityName + " but not currently active";
				Debug.Log($"[ActivityHandler] StopSafely FAILED: Tried to stop {activity.AbilityName} but not active");
				return;
			}

			ActivityStory += "\n Stopped doing " + activity.AbilityName + " with last activity: " +
			                 currentActivity?.AbilityName;
			Debug.Log($"[ActivityHandler] StopSafely SUCCESS: Stopped {activity.AbilityName}");
			isActive = false;
			currentActivity = null;

		}



	}

}
