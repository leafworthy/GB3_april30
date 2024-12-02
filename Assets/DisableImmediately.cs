using UnityEngine;

public class DisableImmediately : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameObject.SetActive(false);
        Debug.Log("disable here");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
