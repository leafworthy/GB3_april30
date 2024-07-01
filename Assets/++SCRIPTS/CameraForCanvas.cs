
using __SCRIPTS._CAMERA;
using UnityEngine;

namespace __SCRIPTS
{
    public class CameraForCanvas : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var canvas = GetComponentInChildren<Canvas>();
            canvas.worldCamera = CursorManager.GetCamera();
        }

    }
}
