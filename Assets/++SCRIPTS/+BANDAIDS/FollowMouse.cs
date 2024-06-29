using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        if (Game_GlobalVariables.IsPaused)
        {
            Debug.Log("no folloiw");
            return;
        }
        transform.position = (Vector2)CursorManager.GetMousePosition();
       
    }
}