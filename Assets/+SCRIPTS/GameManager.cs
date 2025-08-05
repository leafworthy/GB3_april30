using UnityEngine;

namespace __SCRIPTS
{
	public class GameManager : Singleton<GameManager>
	{
		static bool hasInitialized;
		protected override void OnEnable()
		{
			if(hasInitialized)return;
			base.OnEnable();
			hasInitialized = true;
			Debug.Log("GAME MANAGER: GameManager started");
			DontDestroyOnLoad(gameObject);
		}
	}
}
