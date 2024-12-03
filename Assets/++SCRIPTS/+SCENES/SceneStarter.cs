using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStarter : MonoBehaviour
{
	public static bool hasStarted;
	public GameObject display;
	private void Awake()
	{
		if(hasStarted) return;
		hasStarted = true;
		CreateGameManager();
	}

	private void CreateGameManager()
	{
		if(SceneLoader.hasLoaded) return;
	SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);
	}
}