using System;
using UnityEngine;

public class EnemyController : MonoBehaviour, IMovementController, IRecyclable
{
	public event Action<Vector3> OnMovePress;
	public event Action OnMoveRelease;

	public event Action<Vector3> OnAttackButtonPress;
	public event Action OnAttackButtonRelease;
	private bool isOn = true;

	public void MoveInDirection(Vector3 newDirection)
	{
		if (!isOn) return;
		OnMovePress?.Invoke(newDirection);
	}


	public void StopMoving()
	{
		if (!isOn) return;
		OnMoveRelease?.Invoke();
	}

	public void StopAttacking()
	{
		if (!isOn) return;
		OnAttackButtonRelease?.Invoke();
	}

	public void AttackTarget(Vector3 target)
	{
		if (!isOn) return;
		OnAttackButtonPress?.Invoke(target);
	}

	public void Stop()
	{
		isOn = false;
	}

	public void Recycle()
	{
		isOn = true;
	}

	public void Breakdown()
	{
		Stop();
	}
}
