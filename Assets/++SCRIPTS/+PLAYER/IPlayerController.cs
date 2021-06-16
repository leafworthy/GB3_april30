using System;
using UnityEngine;

namespace _SCRIPTS
{
	public interface IPlayerController
	{
		event Action<Player> OnPauseButtonPress;
		event Action<Player> OnPauseButtonRelease;
		event Action<Vector3> OnRightTriggerPress;
		event Action OnRightTriggerRelease;
		event Action<Vector3> OnLeftTriggerPress;
		event Action<Vector3> OnLeftTriggerRelease;
		event Action OnDashButtonPress;
		event Action OnDashButtonRelease;
		event Action OnJumpPress;
		event Action OnJumpRelease;
		event Action<Vector3> OnAttackPress;
		event Action OnReloadPress;
		event Action OnAttackRelease;
		event Action OnMoveRelease;
		event Action<Vector3> OnMovePress;
		event Action OnAimStickInactive;
		event Action<Vector3> OnAim;
		event Action OnChargePress;
		event Action OnChargeRelease;

		void SetPlayer(Player player);
		Color GetPlayerColor();
	}
}
