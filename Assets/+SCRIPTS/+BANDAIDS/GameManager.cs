using UnityEngine;

namespace __SCRIPTS
{
	public class GameManager : MonoBehaviour
	{

		public static GameMode currentGameMode;

		[RuntimeInitializeOnLoadMethod]
		static void CleanUp()
		{
			currentGameMode = GameMode.Arcade;
		}

		protected void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public static void SetGameMode(GameMode mode)
		{
			currentGameMode = mode;
		}

		public enum GameMode
		{
			Survival,
			Arcade
		}
	}
}
