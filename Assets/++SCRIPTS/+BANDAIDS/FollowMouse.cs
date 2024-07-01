using __SCRIPTS._CAMERA;
using __SCRIPTS._COMMON;
using UnityEngine;

namespace __SCRIPTS._BANDAIDS
{
    public class FollowMouse : MonoBehaviour
    {
        void Update()
        {
            if (GlobalManager.IsPaused)
            {
                Debug.Log("no folloiw");
                return;
            }
            transform.position = (Vector2)CursorManager.GetMousePosition();
       
        }
    }
}