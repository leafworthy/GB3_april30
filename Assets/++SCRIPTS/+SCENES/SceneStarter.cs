using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStarter : MonoBehaviour
{
	public static bool hasStarted;
	private void Awake()
	{
		if(hasStarted) return;
		hasStarted = true;
		Debug.Log("awake");
		CreateGameManager();
	}

	private void CreateGameManager()
	{
		if(SceneLoader.hasLoaded) return;
		SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);
		Debug.Log("gamemanager created");
	}
}