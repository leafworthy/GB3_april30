using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class Singleton<T> : SerializedMonoBehaviour where T : Component
	{
		static T _instance;

		[RuntimeInitializeOnLoadMethod]
		static void ResetStatics()
		{
			_instance = null;
		}

		public static T I
		{
			get
			{
				AddToScene();
				return _instance;
			}
		}

		static void AddToScene(GameObject go = null)
		{
			if (_instance != null) return;
			_instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
			if (_instance == null)
			{
				if (go == null)
				{
					go = new GameObject(typeof(T).Name);
					go.AddComponent<T>();
				}

				_instance = go.GetComponent<T>();
			}
		}

		protected virtual void OnEnable()
		{
			if (!Application.isPlaying) return;
			if (_instance == null || _instance == this)
				_instance = this as T;
			else
				Destroy(this);
		}
	}
}
