using UnityEngine;

namespace __SCRIPTS
{
    public class FollowMouse : MonoBehaviour
    {
        private IAimAbility aim;


        void Update()
        {
            if (Services.pauseManager.IsPaused)
            {
                return;
            }
            transform.position = aim.GetAimPoint();

        }
    }
}
