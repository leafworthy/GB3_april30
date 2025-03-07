using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStarter : MonoBehaviour
{
	private static bool hasStarted;
	
	private void Awake()
	{
		if(hasStarted) return;
		hasStarted = true;
		CreateGameManager();
	}

	private void CreateGameManager()
	{
		// Check if GameManager already exists in any form
		if(SceneLoader.hasLoaded) return;
		
		// Check if GameManager scene is already loaded
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if (SceneManager.GetSceneAt(i).name == "GameManager")
			{
				Debug.Log("GameManager scene already loaded");
				return;
			}
		}
		
		// Load GameManager scene additively
		SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);
		Debug.Log("Create game manager");
	}
}