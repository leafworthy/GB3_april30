using UnityEngine;

namespace __SCRIPTS
{
    public class FollowMouse : ServiceUser
    {
        private IAimAbility aim;


        void Update()
        {
            if (pauseManager.IsPaused)
            {
                return;
            }
            transform.position = aim.GetAimPoint();

        }
    }
}
