using UnityEngine;

public class CameraForWorldspaceCanvas : MonoBehaviour
{
    void Start()
    {
        var canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = CursorManager.GetCamera();
    }

}