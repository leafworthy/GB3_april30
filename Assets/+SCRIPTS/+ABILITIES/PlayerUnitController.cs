using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class PlayerUnitController : MonoBehaviour, INeedPlayer, IMove
	{
		private Player player;
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public Vector2 GetMoveAimDir() => player.Controller.MoveAxis.GetCurrentAngle();

		public bool IsMoving() => false;

		public void SetPlayer(Player _player)
		{
			player = _player;
			player.Controller.MoveAxis.OnChange += Player_MoveInDirection;
			player.Controller.MoveAxis.OnInactive += Player_StopMoving;
		}

		private void Player_StopMoving(NewInputAxis obj)
		{
			if (Services.pauseManager.IsPaused) return;
			OnStopMoving?.Invoke();
		}

		private void Player_MoveInDirection(NewInputAxis axis, Vector2 newDirection)
		{
			if (Services.pauseManager.IsPaused) return;

			if (newDirection.magnitude < .5f)
			{
				OnStopMoving?.Invoke();
				return;
			}

			OnMoveInDirection?.Invoke(newDirection);

		}


	}
}
