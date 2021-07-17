using System;
using UnityEngine;

public interface IMovementController
{

	event Action OnMoveRelease;
	event Action<Vector3> OnMovePress;
}

public interface IDashController : IMovementController
{
	event Action OnDashButtonPress;
	event Action OnDashButtonRelease;
}
