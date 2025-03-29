using UnityEngine;

namespace __SCRIPTS
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;


        public static T I
        {
            get
            {
                AddToScene();
                return _instance;
            }
        }

        public static void AddToScene(GameObject go = null)
        {
            if (_instance != null) return;
            _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
 
        }


        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;
            if (_instance == null)
                _instance = this as T;
            Debug.Log("SINGLETON ONLINE: " + typeof(T).Name, this);
        }

        private void OnDestroy()
        {
            Debug.Log("this happens to " + typeof(T).Name, this);
            Debug.Log("SINGLETON OFFLINE: " + typeof(T).Name, this);
        }
    }
}