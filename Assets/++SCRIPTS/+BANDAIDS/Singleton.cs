using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
	private static T _instance;
    private static bool _shuttingDown = false;
    private static object _lock = new object();

	public static T I
	{
		get
		{
            if (_shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }
            
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) + 
                            " is needed in the scene, so '" + singleton.name + 
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using existing instance of " + typeof(T));
                    }
                }

                return _instance;
            }
		}
	}

	protected virtual void Awake()
	{
		if (!Application.isPlaying) return;
        
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[Singleton] " + typeof(T).Name + " marked as DontDestroyOnLoad.");
        }
        else if (_instance != this)
        {
            // If an instance already exists and it's not this one, destroy this one
            Debug.LogWarning("[Singleton] Another instance of " + typeof(T) + 
                " already exists! Destroying duplicate instance.");
            Destroy(gameObject);
        }
	}

    private void OnApplicationQuit()
    {
        _shuttingDown = true;
    }
    
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _shuttingDown = true;
        }
    }
}