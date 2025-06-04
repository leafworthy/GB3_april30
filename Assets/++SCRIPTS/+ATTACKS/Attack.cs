using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GangstaBean.Attacks
{
	[Serializable]
	public class Attack
	{
	
		public Attack(Life attacker, Life defender, float damageAmount)
		{
			DestinationLife = defender;
			OriginLife = attacker;
			if(attacker != null)
			{
				OriginHeight = attacker.AttackHeight;
				OriginFloorPoint = attacker.transform.position;
				Owner = attacker.player;
				Owner = attacker.player;
			}

			if (defender != null)
			{
				DestinationFloorPoint = defender.transform.position;
				DestinationHeight = attacker.AttackHeight;
			}

			DamageAmount = damageAmount;
		}

		public Attack(Life attacker, Vector2 attackFloorPoint, Vector2 destinationFloorPoint, Life defender, float damageAmount)
		{
			OriginFloorPoint = attackFloorPoint;
			OriginLife = attacker;
			if(attacker != null)
			{
				OriginHeight = attacker.AttackHeight;
				Owner = attacker.player;
			}
		
			DestinationFloorPoint = destinationFloorPoint;
			if(defender != null)
			{
				DestinationLife = defender;
				DestinationHeight = defender.AttackHeight;
			}
		
			DamageAmount = damageAmount;
		}

	



		public Life OriginLife;
		public Life DestinationLife;
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
	
		[FormerlySerializedAs("DestinationFlootPoint"),FormerlySerializedAs("Destination")] public Vector2 DestinationFloorPoint;
		[FormerlySerializedAs("Origin")] public Vector2 OriginFloorPoint;
		public Player Owner;
		public Color color = Color.red;
		public bool IsWounding;
	}
}