using GangstaBean.UI;
using UnityEngine;

namespace GangstaBean.Utilities.UI
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