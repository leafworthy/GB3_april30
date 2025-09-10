using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
    public abstract class Singleton<T> : SerializedMonoBehaviour where T : Component
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

        private static void AddToScene(GameObject go = null)
        {
            if (_instance != null) return;
            _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if(_instance == null)
            {
                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                    go.AddComponent<T>();
                }
                _instance = go.GetComponent<T>();
                Debug.Log("Singleton " + typeof(T).Name + "actually created", go);
            }
        }


        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return;
            if (_instance == null || _instance == this)
            {
	            _instance = this as T;
            Debug.Log("SINGLETON ONLINE: " + typeof(T).Name, this);
            }
            else
            {
	            Debug.Log("SINGLETON DUPLICATE DESTROYED: " + typeof(T).Name, this);
	            Destroy(this);
            }
        }

        private void OnDisable()
        {
            Debug.Log("SINGLETON OFFLINE: " + typeof(T).Name, this);
        }
    }
}
