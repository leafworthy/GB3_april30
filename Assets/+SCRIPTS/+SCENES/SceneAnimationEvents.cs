using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class SceneAnimationEvents : MonoBehaviour
	{

		public void FadeInComplete()
		{

			Services.sceneLoader.FadeInComplete();
		}

		public void FadeComplete()
		{

			Services.sceneLoader.FadeOutComplete();
		}

	}
}
