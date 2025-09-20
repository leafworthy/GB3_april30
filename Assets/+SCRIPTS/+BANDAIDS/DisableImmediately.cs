using UnityEngine;

namespace __SCRIPTS
{
    public class DisableImmediately : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            gameObject.SetActive(false);
        }

    }
}
