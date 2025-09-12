using UnityEngine;

namespace __SCRIPTS
{
	public class GameManager : Singleton<GameManager>
	{
		static bool hasInitialized;
		protected void Start()
		{
			if(hasInitialized)return;
			base.OnEnable();
			hasInitialized = true;
			Debug.Log("GAME MANAGER: GameManager started");
			DontDestroyOnLoad(gameObject);
		}
	}
}
