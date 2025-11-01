using System;
using GangstaBean.Core;
using UnityEngine;



namespace __SCRIPTS
{
	public interface ICanMoveThings
	{
		event Action<Vector2> OnMoveInDirection;
		event Action OnStopMoving;
		Vector2 GetMoveAimDir();
		bool IsMoving();

	}


	public interface IHaveAttackStats
	{
		float PrimaryAttackRate { get; }
		float PrimaryAttackDamageWithExtra { get; }
		float PrimaryAttackRange { get; }
		float UnlimitedAttackRate { get; }
		float UnlimitedAttackDamageWithExtra { get; }
		float UnlimitedAttackRange { get; }
		float ExtraDamageFactor { get; set; }
		float AggroRange { get; }
		float MoveSpeed { get; }
	}


	public interface ICanAttack
	{
		Transform transform { get; }
		Player player { get; }
		LayerMask EnemyLayer { get; }
		IHaveAttackStats Stats { get; }

		bool IsEnemyOf(IGetAttacked targetLife);
		event Action<IGetAttacked> OnAttack;
	}



	public interface IGetAttacked : INeedPlayer
	{
		public void TakeDamage(Attack attack);

		public event Action<Attack> OnDead;
		public Player player { get; }
		Transform transform { get; }
		DebrisType debrisType { get; }
		UnitCategory category { get; }
		bool CanTakeDamage();
		bool IsDead();
		event Action<Player, bool> OnDeathComplete;
		void DieNow();
		event Action<Attack> OnAttackHit;
	}

	[Serializable]
	public class Attack
	{
		private Attack()
		{
			OriginHeight = 5;
			DestinationHeight = 5;
			TintColor = Color.red;
		}

		public static Attack Create(ICanAttack originLife, IGetAttacked destinationLife)
		{
			var attack = new Attack();
			attack.OriginLife = originLife;
			if (originLife != null) attack.OriginFloorPoint = originLife.transform.position;
			attack.DestinationLife = destinationLife;
			if (destinationLife != null) attack.DestinationFloorPoint = destinationLife.transform.position;
			return attack;
		}

		public Attack WithDamage(float damage)
		{
			DamageAmount = damage;
			return this;
		}

		public Attack WithExtraPush(float push)
		{
			ExtraPush = push;
			return this;
		}

		public Attack WithOriginHeight(float height)
		{
			OriginHeight = height;
			return this;
		}

		public Attack WithOriginPoint(Vector3 transformPosition)
		{
			OriginFloorPoint = transformPosition;
			return this;
		}

		public void WithDestinationLife(IGetAttacked target)
		{
			DestinationLife = target;
			if (target != null)
				DestinationFloorPoint = target.transform.position;
		}

		public Attack WithDestinationPoint(Vector2 point)
		{
			DestinationFloorPoint = point;
			return this;
		}

		public Attack WithDestinationHeight(float height)
		{
			DestinationHeight = height;
			return this;
		}

		public Attack WithDebree(bool withDebree = true)
		{
			MakesDebree = withDebree;
			return this;
		}

		public Attack WithFlying(bool causesFlying = true)
		{
			CausesFlying = causesFlying;
			return this;
		}

		public Attack WithTint(Color color)
		{
			TintColor = color;
			return this;
		}


		// Properties
		public ICanAttack OriginLife;
		public IGetAttacked DestinationLife;
		public float OriginHeight;
		public float DestinationHeight;
		public float DamageAmount;
		public bool MakesDebree;
		public Vector2 DestinationFloorPoint;
		public Vector2 OriginFloorPoint;
		public float ExtraPush;
		public Color TintColor;
		public bool CausesFlying;
		public Vector2 Direction => DestinationFloorPoint - OriginFloorPoint;
		public Vector2 FlippedDirection => OriginFloorPoint - DestinationFloorPoint;
		public Vector2 DestinationWithHeight => DestinationFloorPoint + new Vector2(0, DestinationHeight);
		public Vector2 OriginWithHeight => OriginFloorPoint + new Vector2(0, OriginHeight);
	}
}
