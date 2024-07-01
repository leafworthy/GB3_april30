using UnityEngine;

namespace __SCRIPTS._COMMON
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
			_instance = FindObjectOfType<T>(true);
			if (_instance != null) return;
			if (go == null)
			{
				go = new GameObject();
				go.name = typeof(T).Name;
			}


			_instance = go.AddComponent<T>();
		}


		protected virtual void Awake()
		{
			if (!Application.isPlaying) return;
			if (_instance == null)
				_instance = this as T;
			//Debug.Log("SINGLETON ONLINE: " + typeof(T).Name, this);
		}
	}
}
