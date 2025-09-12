using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class Attack
	{
		private void MakeNewAttack(Life originLife, Vector2 originFloorPoint, Vector2 destinationFloorPoint, Life destinationLife, float damageAmount)
		{
			OriginFloorPoint = originFloorPoint;
			OriginLife = originLife;
			OriginHeight = 5;

			DestinationFloorPoint = destinationFloorPoint;
			DestinationLife = destinationLife;
			DestinationHeight = 5;

			DamageAmount = damageAmount;
		}

		public Attack(Life attacker, Life defender, float damageAmount)
		{
			Debug.Log("attack style 1");
			MakeNewAttack(attacker, attacker.transform.position, defender.transform.position, defender, damageAmount);
		}

		public Attack(Life attacker, Vector2 attackFloorPoint, Vector2 destinationFloorPoint, Life defender, float damageAmount)
		{
			Debug.Log("attack style 2");
			MakeNewAttack(attacker, attackFloorPoint, destinationFloorPoint, defender, damageAmount);
		}



		public Attack(Life attacker, Vector2 attackPointWithHeight, Life defender, float damageAmount)
		{
			Debug.Log("attack style 3");
			MakeNewAttack(attacker, attackPointWithHeight - new Vector2(0, attacker.AttackHeight), defender.transform.position, defender, damageAmount);
		}

		public Attack(Life attacker, Life defender, Vector2 attackPointWithHeight, Vector2 destinationPointWithHeight,  float damageAmount)
		{
			Debug.Log("attack style 3");
			MakeNewAttack(attacker, attackPointWithHeight - new Vector2(0, 5), destinationPointWithHeight - new Vector2(0, 5), defender, damageAmount);
		}

		public Attack(Life attacker, Vector2 attackPointWithHeight, Vector2 destinationFloorPoint, float damageAmount)
		{
			Debug.Log("attack style 4");
			MakeNewAttack(attacker, attackPointWithHeight - new Vector2(0, attacker.AttackHeight), destinationFloorPoint, null, damageAmount);
		}

		public Life OriginLife;
		public Life DestinationLife;
		public Vector2 Direction => DestinationFloorPoint - OriginFloorPoint;
		public Vector2 FlippedDirection => OriginFloorPoint - DestinationFloorPoint;

		public Attack GetFlippedAttack() => new(DestinationLife, OriginLife, DamageAmount);
		public Vector2 DestinationWithHeight => DestinationFloorPoint + new Vector2(0, DestinationHeight);
		public Vector2 OriginWithHeight => OriginFloorPoint + new Vector2(0, OriginHeight);

		public float OriginHeight;
		public float DestinationHeight;
		public float DamageAmount;
		public bool IsPoison;

		public Vector2 DestinationFloorPoint;
		public Vector2 OriginFloorPoint;
		public Color color = Color.red;
	}
}
