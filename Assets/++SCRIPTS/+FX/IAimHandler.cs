using System;
using UnityEngine;

internal interface IAimHandler
{
	event Action<Vector3> OnAim;
	event Action OnAimStop;
}
