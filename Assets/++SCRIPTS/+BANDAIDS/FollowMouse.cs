using UnityEngine;

namespace __SCRIPTS
{
    public class FollowMouse : MonoBehaviour
    {
        private AimAbility aim;
  

        void Update()
        {
            if (PauseManager.I.IsPaused)
            {
                return;
            }
            transform.position = aim.GetAimPoint();
       
        }
    }
}