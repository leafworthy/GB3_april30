using System;
using __SCRIPTS;
using UnityEngine;

namespace GangstaBean.Core
{

	public interface INeedPlayer
	{
		void SetPlayer(Player newPlayer);
	}



	public interface IPoolable
	{
		void OnPoolSpawn();
		void OnPoolDespawn();
	}

	public interface ICanMove
	{
		void SetCanMove(bool canMove);
	}

	internal interface ISetBool
	{
		void SetBool(int hash, bool value);
	}

	public interface ICanMoveThings
	{
		event Action<Vector2> OnMoveInDirection;
		event Action OnStopMoving;
		Vector2 GetMoveAimDir();
		bool IsMoving();
	}

	public interface IHaveUnitStats
	{
		UnitStats Stats { get; }
	}

	public interface ICanAttack
	{
		Transform transform { get; }
		Player player { get; }
		LayerMask EnemyLayer { get; }
		IHaveUnitStats stats { get; }

		bool IsEnemyOf(Life targetLife);
		event Action<Life> OnAttack;
		event Action OnAttackStop;
		event Action OnAttackStart;
	}


}
