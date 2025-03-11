using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures the GameManager scene is loaded additively when a game scene is loaded
/// </summary>
public class SceneStarter : MonoBehaviour
{
    [Tooltip("Reference to the GameManager scene")]
    [SerializeField] private SceneDefinition gameManagerScene;
    
    private static bool hasStarted;
    
    private void Awake()
    {
        if(hasStarted) return;
        hasStarted = true;
        CreateGameManager();
    }

    private void CreateGameManager()
    {
        // Check if SceneLoader is already initialized
        if(SceneLoader.hasLoaded) return;
        
        // Check if GameManager scene is already loaded
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            if (sceneName == "GameManagerScene" || 
                sceneName == "GameManager" || 
                (gameManagerScene != null && sceneName == gameManagerScene.SceneName))
            {
                Debug.Log("GameManager scene already loaded");
                return;
            }
        }
        
        // Determine which game manager scene to load
        string sceneToLoad;
        
        // First try the serialized reference
        if (gameManagerScene != null && gameManagerScene.IsValid())
        {
            sceneToLoad = gameManagerScene.SceneName;
        }
        // Then try the ASSETS reference
        else if (ASSETS.Scenes != null && ASSETS.Scenes.gameManager != null)
        {
            sceneToLoad = ASSETS.Scenes.gameManager.SceneName;
        }
        // Finally fall back to a default name
        else
        {
            sceneToLoad = "GameManagerScene";
            Debug.LogWarning("No GameManager scene reference found, using default 'GameManagerScene'");
        }
        
        // Load GameManager scene additively
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        Debug.Log($"Loading GameManager scene: {sceneToLoad}");
    }
}