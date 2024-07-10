using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        if (GlobalManager.IsPaused)
        {
            return;
        }
        transform.position = (Vector2)CursorManager.GetMousePosition();
       
    }
}