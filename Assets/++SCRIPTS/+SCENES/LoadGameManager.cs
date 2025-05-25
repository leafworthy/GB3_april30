using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS
{
    /// <summary>
    /// Ensures the GameManager scene is loaded additively when a game scene is loaded
    /// </summary>
    public static class LoadGameManager
    {
        private static string SceneName => "0_GameManagerScene";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadGameManagerAtStart()
        {
            if (GameManager.gameManagerLoaded)
            {
                Debug.Log("game manager already created");
                return;
            }
            CreateGameManager();
        }

        private static void CreateGameManager()
        {
            // Check if GameManager scene is already loaded
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                string sceneName = SceneManager.GetSceneAt(i).name;
                if (sceneName != "GameManagerScene" && sceneName != "GameManager") continue;
                Debug.Log("GameManager scene already loaded");
                return;
            }

            // Load GameManager scene additively
            SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
            Debug.Log($"Loading GameManager scene");
        }
    }
}
