using UnityEngine;

namespace _SCRIPTS
{
    public class ClickToQuit : MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Application.Quit();
            }
        }
    }
}
