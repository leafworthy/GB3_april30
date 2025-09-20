using System;
using GangstaBean.Core;
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

		[NonSerialized] private MonoBehaviour parentMonoBehaviour;

		public void SetParent(MonoBehaviour parent)
		{
			parentMonoBehaviour = parent;
		}

		public bool Do(IActivity activity)
		{
			if (parentMonoBehaviour != null)
			{
				var life = parentMonoBehaviour.GetComponent<Life>();
				if (life != null && life.IsDead()) return false;
			}

			if (isActive) return false;

			isActive = true;
			currentActivity = activity;

			return true;
		}

		public bool StopCurrentActivity()
		{
			if (!isActive) return false;

			isActive = false;
			currentActivity = null;

			return true;
		}

		public void Stop(IActivity activity)
		{
			isActive = false;
			currentActivity = null;
		}
	}
}
