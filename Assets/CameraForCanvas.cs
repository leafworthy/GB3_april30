using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = CursorManager.GetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
