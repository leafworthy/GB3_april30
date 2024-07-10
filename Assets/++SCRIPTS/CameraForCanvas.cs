using UnityEngine;

public class CameraForCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = CursorManager.GetCamera();
    }

}