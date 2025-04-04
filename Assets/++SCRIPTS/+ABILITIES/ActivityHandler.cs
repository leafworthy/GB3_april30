using System;
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
		public string currentActivity;
		public bool isActive;


		public bool Do(string Verb)
		{
			if (isActive)
			{
				Debug.Log( $"Tried to do {Verb} but current activity is {currentActivity}" +
				          $" on {this} with {currentActivity} and is active: {isActive}");
				return false;
			}
			if (currentActivity == Verb)
			{
				Debug.Log( $"Tried to do {Verb} but current activity is {currentActivity}" +
				          $" on {this} with {currentActivity} and is active: {isActive}");
				
				return false;
			}
			isActive = true;
			currentActivity = Verb;
			return true;
		}


		public bool Stop(string Verb = "")
		{
			if (!isActive)
			{
				return false;
			}
			isActive = false;
			currentActivity = null;
			return true;
		}

		public bool StopSafely(string Verb = "")
		{
			if (currentActivity != Verb)
			{
				Debug.Log( $"Tried to stop {Verb} but current activity is {currentActivity}" +
				          $" on {this} with {currentActivity} and is active: {isActive}");
				return false;
			}
			if (!isActive)
			{
				Debug.Log( $"Tried to stop {Verb} but current activity is {currentActivity}" +
				          $" on {this} with {currentActivity} and is active: {isActive}");
				return false;
			}

			Debug.Log("Stopped activity safely: " + Verb + " on " + this + " with " + currentActivity +
			          " and is active: " + isActive);
			isActive = false;
			currentActivity = null;
			return true;
		}
	}
}