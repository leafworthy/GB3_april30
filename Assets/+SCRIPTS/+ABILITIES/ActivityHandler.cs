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
					ActivityStory += $"\nTried to do {activity.VerbName} but character is dead";
					Debug.Log($"[ActivityHandler] BLOCKED: Character is dead, cannot {activity.VerbName}");
					return false;
				}
			}

			if (isActive)
			{
				if (currentActivity?.VerbName == activity.VerbName)
				{
					ActivityStory += "\nTried to start doing " + activity.VerbName + " but already doing it";
					Debug.Log($"[ActivityHandler] BLOCKED: Already doing {activity.VerbName}");
					return false;
				}

				ActivityStory += "\n" +
				                 $"Tried to do {activity.VerbName} but current activity is {currentActivity?.VerbName}" +
				                 $" on {this} with {currentActivity?.VerbName} and is active: {isActive}";
				Debug.Log($"[ActivityHandler] BLOCKED: Tried {activity.VerbName} but busy with {currentActivity?.VerbName}");
				return false;
			}

			ActivityStory += "\nStarted doing " + activity.VerbName + " with last activity: " +
			                 currentActivity?.VerbName;
			Debug.Log($"[ActivityHandler] SUCCESS: Started {activity.VerbName}");
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
			if (currentActivity?.VerbName != activity.VerbName)
			{
				ActivityStory += "\n Tried to stop " + activity.VerbName + " but current activity is " +
				                 currentActivity?.VerbName;
				Debug.Log($"[ActivityHandler] StopSafely FAILED: Tried to stop {activity.VerbName} but current is {currentActivity?.VerbName}");
				return;
			}

			if (!isActive)
			{
				ActivityStory += "\n Tried to stop " + activity.VerbName + " but not currently active";
				Debug.Log($"[ActivityHandler] StopSafely FAILED: Tried to stop {activity.VerbName} but not active");
				return;
			}

			ActivityStory += "\n Stopped doing " + activity.VerbName + " with last activity: " +
			                 currentActivity?.VerbName;
			Debug.Log($"[ActivityHandler] StopSafely SUCCESS: Stopped {activity.VerbName}");
			isActive = false;
			currentActivity = null;

		}

		public bool CompleteCurrentActivity(CompletionReason reason, GangstaBean.Core.IActivity newActivity = null)
		{
			if (!isActive || currentActivity == null) return false;

			// Try graceful completion first
			bool handled = false;


			// Use default cleanup if no custom completion was handled
			ActivityStory += $"\nUsing default cleanup for {currentActivity.VerbName}";
			DefaultCleanup(currentActivity, parentMonoBehaviour);
			isActive = false;
			currentActivity = null;
			return true;
		}

		public bool DoWithCompletion(GangstaBean.Core.IActivity activity, CompletionReason reason = CompletionReason.NewActivity)
		{
			if (isActive && currentActivity != null)
			{
				if (currentActivity.VerbName == activity.VerbName)
				{
					ActivityStory += "\nTried to start doing " + activity.VerbName + " but already doing it";
					return false;
				}

				// Try completing current activity first
				if (!CompleteCurrentActivity(reason, activity))
				{
					ActivityStory += $"\nCould not complete {currentActivity.VerbName} for {activity.VerbName}";
					return false;
				}
			}

			// Continue with normal Do logic
			return Do(activity);
		}

		/// <summary>
		/// Default cleanup logic that handles common patterns across most abilities.
		/// This covers the standard cleanup that 80% of abilities need.
		/// </summary>
		/// <param name="activity">The activity being cleaned up</param>
		/// <param name="parent">The MonoBehaviour component hosting the activity</param>
		public static void DefaultCleanup(GangstaBean.Core.IActivity activity, MonoBehaviour parent)
		{
			if (parent == null) return;

			// Cancel any pending Invoke calls - common safety mechanism
			parent.CancelInvoke();

			// Get common components that abilities use
			var animations = parent.GetComponent<UnitAnimations>();
			var body = parent.GetComponent<Body>();

			// Reset common animation states
			if (animations != null)
			{
				animations.SetBool(UnitAnimations.IsAttacking, false);
				animations.SetBool(UnitAnimations.IsShooting, false);
				animations.SetBool(UnitAnimations.IsCharging, false);
				animations.SetBool(UnitAnimations.IsShielding, false);
				animations.SetBool(UnitAnimations.IsBobbing, true); // Default bobbing state
			}

			// Body part cleanup is handled by the body parts themselves via StopSafely()
			// So we don't need to duplicate that logic here

			Debug.Log($"[ActivityHandler] Default cleanup completed for {activity.VerbName}");
		}
	}

}

namespace __SCRIPTS
{
}
