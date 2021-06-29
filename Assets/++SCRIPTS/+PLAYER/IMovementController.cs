using System;
using UnityEngine;

public interface IMovementController
{
	event Action OnDashButtonPress;
	event Action OnDashButtonRelease;
	event Action OnMoveRelease;
	event Action<Vector3> OnMovePress;
}
