using UnityEngine;

public class DeactivateOnAwake : MonoBehaviour
{
    private void OnValidate()
    {
        gameObject.SetActive(false);
    }
}
