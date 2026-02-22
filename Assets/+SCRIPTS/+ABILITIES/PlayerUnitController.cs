using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent]
	public class PlayerUnitController : MonoBehaviour, INeedPlayer, ICanMoveThings, ICanAttack
	{
		public Player player => _player;
		Player _player;
		public LayerMask EnemyLayer => player.GetEnemyLayer();
		public IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
		IHaveUnitStats _stats;

		public bool IsEnemyOf(Life targetLife) => player?.IsHuman() != targetLife.player?.IsHuman();
		public event Action<Life> OnAttack;
		public event Action OnAttackStart;
		public event Action OnAttackStop;
		public event Action<Vector2> OnMoveInDirection;
		public event Action OnStopMoving;
		public Vector2 GetMoveAimDir() => player.Controller.MoveAxis.GetCurrentAngle();

		public bool IsMoving() => false;

		public void SetPlayer(Player newPlayer)
		{
			_player = newPlayer;
			if (player == null) return;
			player.Controller.MoveAxis.OnChange += Player_MoveInDirection;
			player.Controller.MoveAxis.OnInactive += Player_StopMoving;
			player.Controller.Attack1RightTrigger.OnPress += Player_Attack1Press;
			player.Controller.Attack1RightTrigger.OnRelease += Player_Attack1Release;
		}

		void Player_Attack1Release(NewControlButton obj)
		{
			if (Services.pauseManager.IsPaused) return;
			OnAttackStop?.Invoke();
		}

		void Player_Attack1Press(NewControlButton obj)
		{
			if (Services.pauseManager.IsPaused) return;
			OnAttack?.Invoke(null);
			OnAttackStart?.Invoke();
		}

		void Player_StopMoving(NewInputAxis obj)
		{
			if (Services.pauseManager.IsPaused) return;
			OnStopMoving?.Invoke();
		}

		void Player_MoveInDirection(NewInputAxis axis, Vector2 newDirection)
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
