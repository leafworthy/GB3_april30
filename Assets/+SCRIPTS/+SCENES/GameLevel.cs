using System;

namespace __SCRIPTS
{
	public class GameLevel : ServiceUser
	{
		public TravelPoint defaultTravelPoint => FindFirstObjectByType<TravelPoint>();
		public SceneDefinition scene;
		public event Action OnGameOver;

		private void Start()
		{
			playerManager.OnAllJoinedPlayersDead += LoseLevel;
		}

		public void StopLevel()
		{
			playerManager.OnAllJoinedPlayersDead -= LoseLevel;
		}

		private void LoseLevel()
		{
			OnGameOver?.Invoke();
		}
	}
}
