using System;
using UnityEngine;

public class EnemyController : MonoBehaviour, IMovementController
{
	private event Action OnControllerDie;

	public event Action OnDashButtonPress;
	public event Action OnDashButtonRelease;

	public event Action<Vector3> OnMovePress;
	public event Action OnMoveRelease;

	public event Action<Vector3> OnAttackButtonPress;
	public event Action OnAttackButtonRelease;


	public void MoveInDirection(Vector3 newDirection)
	{
		OnMovePress?.Invoke(newDirection);
	}


	public void StopMoving()
	{
		OnMoveRelease?.Invoke();
	}

	public void StopAttacking()
	{
		OnAttackButtonRelease?.Invoke();
	}

	public void AttackTarget(Vector3 target)
	{
		OnAttackButtonPress?.Invoke(target);
	}

	public void Die()
	{
		OnControllerDie?.Invoke();
	}
}
