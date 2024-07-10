using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLevelGameScene : GameScene
{
    private int counter = 0;
    private int maxCounter = 100;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("restarting soon");  
        StartCoroutine(WaitOneSecondThenGoToScene());
    }

    private IEnumerator WaitOneSecondThenGoToScene()
    {
        Debug.Log("waiting");
        yield return new WaitForSeconds(1);
        Debug.Log("going");
        GoToScene(Type.InLevel);
    }

   
}
