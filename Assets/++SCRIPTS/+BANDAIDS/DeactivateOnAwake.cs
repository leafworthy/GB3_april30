using UnityEngine;

public class DeactivateOnAwake : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false);
    }

}