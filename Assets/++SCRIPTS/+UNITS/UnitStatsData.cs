using UnityEngine;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/UnitStats")]
	public class UnitStatsData : ScriptableObject
	{
		public DebrisType debrisType;
	
		public bool isObstacle;
		public bool isPlayerAttackable = true;

		public float AttackHeight;
	
		public float AttackDamage;
		public float AttackRate;
		public float Attack2Damage;
		public float Attack2Rate;
		public float AttackRange;

		//BULLETS + KUNAI
		public float PrimaryAttackDamage;
		public float PrimaryAttackRate;
		public float PrimaryAttackRange;
		//NADES + CHARGE
		public float SecondaryAttackDamage;
		public float SecondaryAttackRate;
		public float SecondaryAttackRange;
		//KNIFE + BAT
		public float TertiaryAttackDamage;
		public float TertiaryAttackRate;
		public float TertiaryAttackRange;
	
		public float HealthMax;
		public float MoveSpeed;
		public float DashSpeed;
		public float JumpSpeed;

		public float AggroRange;

		public bool ShowLifeBar = false;
	}
}