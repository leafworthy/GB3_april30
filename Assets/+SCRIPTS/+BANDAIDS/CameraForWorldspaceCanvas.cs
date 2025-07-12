using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
    public class CameraForWorldspaceCanvas : MonoBehaviour
    {
        void Start()
        {
            var canvas = GetComponentInChildren<Canvas>();
            canvas.worldCamera = CursorManager.GetCamera();
        }



    }
}
