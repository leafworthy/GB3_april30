using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class Player_UnitController : MonoBehaviour, INeedPlayer, IControlMove
	{
		private Player player;
		private PauseManager pauseManager  => _pauseManager ??= ServiceLocator.Get<PauseManager>();
		private PauseManager _pauseManager;
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public bool IsMoving() => false;

		public void SetPlayer(Player _player)
		{
			player = _player;
			player.Controller.OnMoveAxis_Change += Player_MoveInDirection;
			player.Controller.OnMoveAxis_Inactive += Player_StopMoving;
		}

		private void Player_StopMoving(NewInputAxis obj)
		{
			if (pauseManager.IsPaused) return;
			OnStopMoving?.Invoke();
		}

		private void Player_MoveInDirection(Vector2 newDirection)
		{
			if (pauseManager.IsPaused) return;

			if (newDirection.magnitude < .5f)
			{
				OnStopMoving?.Invoke();
				return;
			}

			OnMoveInDirection?.Invoke(newDirection);

		}


	}
}
