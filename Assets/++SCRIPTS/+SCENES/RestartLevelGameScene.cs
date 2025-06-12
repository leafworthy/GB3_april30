using System.Collections;
using UnityEngine;

namespace __SCRIPTS
{
    public class RestartLevelGameScene : GameScene
    {
   
        // Start is called before the first frame update
        void Start()
        {

            StartCoroutine(WaitOneSecondThenGoToScene());
        }

        private IEnumerator WaitOneSecondThenGoToScene()
        {

            yield return new WaitForSeconds(1);

            LevelManager.I.GoBackFromRestart();
        }

   
    }
}
