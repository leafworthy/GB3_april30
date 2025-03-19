using System.Collections;
using UnityEngine;

public class RestartLevelGameScene : GameScene
{
   
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
        LevelManager.I.GoBackFromRestart();
    }

   
}
