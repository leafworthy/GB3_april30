using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class SceneAnimationActions : MonoBehaviour
	{
		public event Action OnFadeInComplete;
		public event Action OnFadeOutComplete;

		public void FadeInComplete()
		{
			OnFadeInComplete?.Invoke();
		}

		public void FadeComplete()
		{
			OnFadeOutComplete?.Invoke();
		}
	}
}