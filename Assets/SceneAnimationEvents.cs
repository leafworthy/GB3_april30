using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAnimationEvents : MonoBehaviour
{

    public void FadeInComplete()
    {
        Debug.Log("fade in complete");
        SceneLoader.I.FadeInComplete();
    }
    public void FadeComplete()
    {
        Debug.Log ("fade out complete");
        SceneLoader.I.FadeOutComplete();
    }

}
