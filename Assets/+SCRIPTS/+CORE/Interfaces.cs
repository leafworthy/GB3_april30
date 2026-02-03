using System;
using __SCRIPTS;
using UnityEngine;

namespace GangstaBean.Core
{
	/// <summary>
	/// Interface for components that need a player reference
	/// </summary>
	public interface INeedPlayer
	{
		void SetPlayer(Player newPlayer);
	}

	/// <summary>
	/// Interface for components that can be activities
	/// </summary>
	public interface IDoableAbility
	{
		string AbilityName { get; }
		bool canDo();
		bool canStop(IDoableAbility abilityToStopFor);
		void TryToActivate();
		void StopAbility();
		void Resume();
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

		bool IsEnemyOf(IGetAttacked targetLife);
		event Action<IGetAttacked> OnAttack;
	}

	public interface IGetAttacked : INeedPlayer, IHaveUnitStats
	{
		public Player player { get; }
		Transform transform { get; }
		DebrisType DebrisType { get; }
		UnitCategory category { get; }
		float CurrentHealth { get; }
		float MaxHealth { get; }
		public void TakeDamage(Attack attack);
		bool CanTakeDamage();
		bool IsDead();
		void DieNow();
		float GetFraction();
		void AddHealth(float amount);
		public event Action<Attack> OnDead;
		event Action<Player, bool> OnDeathComplete;
		event Action<Attack> OnAttackHit;
		event Action<Attack> OnShielded;
		event Action<float> OnFractionChanged;
		event Action<Attack> OnFlying;
		void SetTemporarilyInvincible(bool i);
		void SetShielding(bool isOn);
		bool IsEnemyOf(ICanAttack life);
	}
}
