using UnityEngine;

namespace __SCRIPTS
{
    public class SceneAnimationEvents : ServiceUser
    {

        public void FadeInComplete()
        {

            sceneLoader.FadeInComplete();
        }
        public void FadeComplete()
        {

	        sceneLoader.FadeOutComplete();
        }

    }
}
