using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class Attack
	{
		Attack()
		{
			OriginHeight = 5;
			DestinationHeight = 5;
			TintColor = Color.red;
			MakesDebree = true;
		}

		public static Attack Create(ICanAttack originLife, Life destinationLife)
		{
			var attack = new Attack
			{
				OriginLife = originLife
			};
			if (originLife != null) attack.OriginFloorPoint = originLife.transform.position;

			if (destinationLife != null)
			{
				attack.DestinationLife = destinationLife;
				attack.DestinationFloorPoint = destinationLife.transform.position;
			}
			else
				attack.DestinationFloorPoint = attack.OriginFloorPoint;

			return attack;
		}

		public static Attack Create(ICanAttack originLife, Vector2 destinationPoint)
		{
			var attack = new Attack
			{
				OriginLife = originLife,
				OriginFloorPoint = originLife.transform.position,
				DestinationFloorPoint = destinationPoint
			};
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


		public Attack WithOriginPoint(Vector3 transformPosition)
		{
			OriginFloorPoint = transformPosition;
			return this;
		}

		public Attack WithDestinationLife(Life target)
		{
			DestinationLife = target;
			if (target != null)
				DestinationFloorPoint = target.transform.position;

			return this;
		}

		public Attack WithDestinationPoint(Vector2 point)
		{
			DestinationFloorPoint = point;
			return this;
		}

		public Attack WithDebree(bool withDebree = true)
		{
			MakesDebree = withDebree;
			return this;
		}

		public Attack WithFlying(bool causesFlying = true, float flyingHeight = 1.5f)
		{
			CausesFlying = causesFlying;
			FlyingHeight = flyingHeight;
			return this;
		}

		public Attack WithTint(Color color)
		{
			TintColor = color;
			return this;
		}

		// Properties
		public ICanAttack OriginLife;
		public Life DestinationLife;
		public float OriginHeight;
		public float DestinationHeight;
		public float DamageAmount;
		public bool MakesDebree;
		public Vector2 DestinationFloorPoint;
		public Vector2 OriginFloorPoint;
		public float ExtraPush;
		public Color TintColor;
		public bool CausesFlying;
		public float FlyingHeight;
		public bool CausesFire;
		public Vector2 Direction => DestinationFloorPoint - OriginFloorPoint;
		public Vector2 FlippedDirection => OriginFloorPoint - DestinationFloorPoint;
		public Vector2 DestinationWithHeight => DestinationFloorPoint + new Vector2(0, DestinationHeight);
		public Vector2 OriginWithHeight => OriginFloorPoint + new Vector2(0, OriginHeight);

		public Attack WithFire(bool causesFire)
		{
			CausesFire = causesFire;
			return this;
		}
	}
}
