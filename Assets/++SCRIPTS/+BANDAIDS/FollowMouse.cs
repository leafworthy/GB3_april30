using UnityEngine;

namespace __SCRIPTS
{
    public class FollowMouse : MonoBehaviour
    {
        private AimAbility aim;
  

        void Update()
        {
            if (PauseManager.IsPaused)
            {
                return;
            }
            transform.position = aim.GetAimPoint();
       
        }
    }
}