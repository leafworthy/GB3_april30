using System;
using UnityEngine;

namespace _SCRIPTS
{
	public class PAUSE:Singleton<PAUSE>
	{

		public static bool isPaused;

		private void Start()
		{
			foreach (var playerController in LEVEL.I.playerControllersInLevel)
			{
				playerController.OnPauseButtonPress += Player_OnPausePressed;
			}
		}

		private void Player_OnPausePressed(PlayerIndex obj)
		{
			if (!isPaused)
			{
				Debug.Log("pause");
				isPaused = true;
				Time.timeScale = 0;
			}
			else
			{
				Debug.Log("unpause");
				isPaused = false;
				Time.timeScale = 1;
			}
		}
	}
}
