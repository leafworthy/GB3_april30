using UnityEngine;

namespace __SCRIPTS
{
	public class GameManager : Singleton<GameManager>
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void ResetStatics()
		{
			hasInitialized = false;
		}
		static bool hasInitialized;
		protected void Start()
		{
			if(hasInitialized)return;
			base.OnEnable();
			hasInitialized = true;
			DontDestroyOnLoad(gameObject);
		}
	}
}
