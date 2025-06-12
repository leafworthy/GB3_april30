using UnityEngine;

namespace __SCRIPTS
{
    public class SceneAnimationEvents : MonoBehaviour
    {

        public void FadeInComplete()
        {

            SceneLoader.I.FadeInComplete();
        }
        public void FadeComplete()
        {

            SceneLoader.I.FadeOutComplete();
        }

    }
}