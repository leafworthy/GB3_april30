using UnityEngine;

namespace __SCRIPTS
{
    public class DeactivateOnAwake : MonoBehaviour
    {
        private void OnValidate()
        {
            gameObject.SetActive(false);
        }
    }
}
