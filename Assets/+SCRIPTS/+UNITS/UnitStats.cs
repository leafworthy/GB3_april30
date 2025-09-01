using System;
using Unity.VisualScripting;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class UnitStats
	{
		[SerializeField]private UnitStatsData _unitData;
		public UnitStatsData Data => GetData();

		private string unitName;

		public UnitStats(string _name)
		{
			unitName = _name;
			_unitData = UnitStatsManager.GetUnitStats(unitName);
			GetData();
		}

		private UnitStatsData GetData() => _unitData ??= UnitStatsManager.GetUnitStats(unitName);

		public float ExtraHealthFactor;
		public float ExtraDamageFactor;
		public float ExtraSpeedFactor;

		public float MaxHealth => Data.healthMax + ExtraHealthFactor * Data.healthMax;
		public float MoveSpeed => Data.moveSpeed + ExtraSpeedFactor * Data.moveSpeed;
		public float DashSpeed => Data.dashSpeed + ExtraSpeedFactor * Data.dashSpeed;
		public float JumpSpeed => Data.jumpSpeed;
		public float AggroRange => Data.aggroRange;
		public float AttackHeight => 5f;
		public bool IsObstacle => Data.category == UnitCategory.Obstacle;
		public bool IsPlayerAttackable => Data.isPlayerSwingHittable;
		public DebrisType DebrisType => Data.debrisType;

		public float GetAttackDamage(int attackIndex) => attackIndex switch
		                                                 {
			                                                 1 => Data.attack1Damage + ExtraDamageFactor * Data.attack1Damage,
			                                                 2 => Data.attack2Damage + ExtraDamageFactor * Data.attack2Damage,
			                                                 3 => Data.attack3Damage + ExtraDamageFactor * Data.attack3Damage,
			                                                 4 => Data.attack4Damage + ExtraDamageFactor * Data.attack4Damage,
			                                                 _ => 0f
		                                                 };

		public float GetAttackRange(int attackIndex) => attackIndex switch
		                                                {
			                                                1 => Data.attack1Range,
			                                                2 => Data.attack2Range,
			                                                3 => Data.attack3Range,
			                                                4 => Data.attack4Range,
			                                                _ => 0f
		                                                };

		public float GetAttackRate(int attackIndex) => attackIndex switch
		                                               {
			                                               1 => Data.attack1Rate,
			                                               2 => Data.attack2Rate,
			                                               3 => Data.attack3Rate,
			                                               4 => Data.attack4Rate,
			                                               _ => 0f
		                                               };
	}
}
