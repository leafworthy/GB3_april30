using System;
using UnityEngine;

namespace _SCRIPTS
{
	public interface IMovementController
	{
		event Action OnDashButtonPress;
		event Action OnDashButtonRelease;
		event Action OnMoveRelease;
		event Action<Vector3> OnMovePress;
	}
}
