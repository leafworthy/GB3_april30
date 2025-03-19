using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static bool gameManagerLoaded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        gameManagerLoaded = true;
    }

}
