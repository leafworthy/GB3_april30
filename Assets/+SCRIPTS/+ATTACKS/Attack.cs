using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	public interface ICanAttack
	{
		float AttackHeight { get; }
		Transform transform { get; }
		Player Player { get; }
		DebrisType DebrisType { get;  }
	}

	[Serializable]
	public class Attack
	{

		public Attack(ICanAttack attacker, ICanAttack defender, float damageAmount)
		{
			DestinationLife = defender;
			OriginLife = attacker;
			if(attacker != null)
			{
				OriginHeight = attacker.AttackHeight;
				OriginFloorPoint = attacker.transform.position;
				Owner = attacker.Player;

				//
				Owner = attacker.Player;
			}

			if (defender != null)
			{
				DestinationFloorPoint = defender.transform.position;
				DestinationHeight = attacker.AttackHeight;
			}

			DamageAmount = damageAmount;
		}

		public Attack(ICanAttack attacker, Vector2 attackFloorPoint, Vector2 destinationFloorPoint, ICanAttack defender, float damageAmount)
		{
			OriginFloorPoint = attackFloorPoint;
			OriginLife = attacker;
			if(attacker != null)
			{
				OriginHeight = attacker.AttackHeight;
				Owner = attacker.Player;
			}

			DestinationFloorPoint = destinationFloorPoint;
			if(defender != null)
			{
				DestinationLife = defender;
				DestinationHeight = defender.AttackHeight;
			}

			DamageAmount = damageAmount;
		}





		public ICanAttack OriginLife;
		public ICanAttack DestinationLife;
		public Vector2 Direction => DestinationFloorPoint - OriginFloorPoint;

		public Vector2 FlippedDirection => OriginFloorPoint -DestinationFloorPoint;

		public Attack GetFlippedAttack()
		{
			if(DestinationLife == null)
				return new Attack(DestinationLife, OriginLife, DamageAmount);
			return new Attack(DestinationLife, OriginLife, DamageAmount);
		}
		public Vector2 DestinationWithHeight => DestinationFloorPoint + new Vector2(0,DestinationHeight);
		public Vector2 OriginWithHeight => OriginFloorPoint + new Vector2(0, OriginHeight);

		public float OriginHeight;
		public float DestinationHeight;
		public float DamageAmount;
		public bool IsPoison;

		public Vector2 DestinationFloorPoint;
		public Vector2 OriginFloorPoint;
		public Player Owner;
		public Color color = Color.red;
		public bool IsWounding;
	}
}
