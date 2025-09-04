using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class GameLevel : MonoBehaviour
	{
		public TravelPoint defaultTravelPoint => FindFirstObjectByType<TravelPoint>();
		public SceneDefinition scene;
		public event Action OnGameOver;

		private void Start()
		{
			Services.playerManager.OnAllJoinedPlayersDead += LoseLevel;
		}

		public void StopLevel()
		{
			Services.playerManager.OnAllJoinedPlayersDead -= LoseLevel;
		}

		private void LoseLevel()
		{
			OnGameOver?.Invoke();
		}
	}
}
