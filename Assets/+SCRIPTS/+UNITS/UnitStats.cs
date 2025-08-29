using UnityEngine;
using VInspector;

namespace __SCRIPTS
{
	public class UnitStats : MonoBehaviour
	{
		[SerializeField] private UnitStatsData _unitData;
		public UnitStatsData Data => GetData();

		private UnitStatsData GetData() => _unitData ??= UnitStatsManager.GetUnitStats(gameObject.name);

		[Header("Extra Factors")] public float ExtraHealthFactor;
		public float ExtraDamageFactor;
		public float ExtraSpeedFactor;

		// Computed properties (base + extra * base)
		public float MaxHealth => Data.healthMax + ExtraHealthFactor * Data.healthMax;
		public float MoveSpeed => Data.moveSpeed + ExtraSpeedFactor * Data.moveSpeed;
		public float DashSpeed => Data.dashSpeed + ExtraSpeedFactor * Data.dashSpeed;
		public float JumpSpeed => Data.jumpSpeed;
		public float AggroRange => Data.aggroRange;
		public float AttackHeight => 5f; // Default value since not in CSV

		// Unit type properties
		public bool IsObstacle => Data.category == UnitCategory.Obstacle;
		public bool IsPlayerAttackable => Data.isPlayerSwingHittable;
		public DebrisType DebrisType => Data.debrisType;

		private void OnEnable()
		{
			GetData();
		}

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

		[Button]
		private void GetStats()
		{
			_unitData = UnitStatsManager.GetUnitStats(gameObject.name);
		}

		[Button]
		private void ClearStats()
		{
			_unitData = null;
		}
	}
}
