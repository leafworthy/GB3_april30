using UnityEngine;

namespace GangstaBean.Utilities
{
    public class DeactivateOnAwake : MonoBehaviour
    {
        private void OnValidate()
        {
            gameObject.SetActive(false);
        }
    }
}
