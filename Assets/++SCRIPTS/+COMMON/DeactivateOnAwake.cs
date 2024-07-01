using UnityEngine;

namespace __SCRIPTS._COMMON
{
    public class DeactivateOnAwake : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            gameObject.SetActive(false);
        }

    }
}
