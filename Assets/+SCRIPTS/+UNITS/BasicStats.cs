using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class BasicStats : MonoBehaviour, INeedPlayer
	{
		//Handles UnitStatsData
		public string UnitName;
		public UnitStatsData Data => _data ??= UnitStatsManager.GetUnitStats(UnitName);
		[SerializeField]private UnitStatsData _data;

		public bool isInvincible  => Data.isInvincible;
		public DebrisType DebrisType => Data.debrisType;

		[Sirenix.OdinInspector.Button]
		public void GetStats()
		{
			_data = UnitStatsManager.GetUnitStats(UnitName);
		}
		#region UnitStats Wrappers

		public UnitCategory category => Data.category;
		public float PrimaryAttackDamageWithExtra => GetAttackDamage(1);
		public float PrimaryAttackRange => GetAttackRange(1);
		public float PrimaryAttackRate => GetAttackRate(1);
		public float SecondaryAttackDamageWithExtra => GetAttackDamage(2);
		public float SecondaryAttackRange => GetAttackRange(2);
		public float SecondaryAttackRate => GetAttackRate(2);
		public float TertiaryAttackDamageWithExtra => GetAttackDamage(3);
		public float TertiaryAttackRange => GetAttackRange(3);
		public float TertiaryAttackRate => GetAttackRate(3);
		public float UnlimitedAttackDamageWithExtra => GetAttackDamage(4);
		public float UnlimitedAttackRange => GetAttackRange(4);
		public float UnlimitedAttackRate => GetAttackRate(4);
		public float MoveSpeed => Data.moveSpeed + ExtraSpeedFactor * Data.moveSpeed + Data.moveSpeed * (EnemyTier ? EnemyTier.GetEnemyTier()* 0.3f: 0);
		public float DashSpeed => Data.dashSpeed + ExtraSpeedFactor * Data.dashSpeed;
		public float JumpSpeed => Data.jumpSpeed;
		public float AggroRange => Data.aggroRange;
		public float AttackHeight => 5f;
		public bool IsObstacle => Data.category == UnitCategory.Obstacle;
		public bool IsPlayerAttackable => Data.isPlayerSwingHittable;
		public bool showLifeBar => Data.showLifeBar;
		public float MaxHealth => Data.healthMax + GetExtraHealth();
		public bool IsNotInvincible => !Data.isInvincible;

		public float ExtraHealthFactor;
		public float ExtraDamageFactor;
		public float ExtraSpeedFactor;
		private int extraMaxDamageFactor;
		[SerializeField] private EnemyTier EnemyTier  => _enemyTier ??= GetComponent<EnemyTier>();
		private EnemyTier _enemyTier;
		private Player player;
		public float GetExtraHealth() => ExtraHealthFactor * Data.healthMax + Data.healthMax * (EnemyTier ? EnemyTier.GetEnemyTier() : 0);

		#endregion

		public bool IsHuman => player != null && player.IsHuman();
		public LayerMask EnemyLayer => IsHuman ? Services.assetManager.LevelAssets.EnemyLayer : Services.assetManager.LevelAssets.PlayerLayer;

		public float GetAttackDamage(int attackIndex) => attackIndex switch
		                                                 {
			                                                 1 => Data.attack1Damage + ExtraDamageFactor * Data.attack1Damage + Data.attack1Damage *(EnemyTier ? EnemyTier.GetEnemyTier(): 0),
			                                                 2 => Data.attack2Damage + ExtraDamageFactor * Data.attack2Damage + Data.attack2Damage *(EnemyTier ? EnemyTier.GetEnemyTier(): 0),
			                                                 3 => Data.attack3Damage + ExtraDamageFactor * Data.attack3Damage + Data.attack3Damage *(EnemyTier ? EnemyTier.GetEnemyTier(): 0),
			                                                 4 => Data.attack4Damage + ExtraDamageFactor * Data.attack4Damage + Data.attack4Damage *(EnemyTier ? EnemyTier.GetEnemyTier(): 0),
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

		public void SetPlayer(Player _player)
		{
			player = _player;
		}

		public int GetEnemyTier() => EnemyTier.GetEnemyTier();

		public void SetEnemyTier(int tier)
		{
			EnemyTier.SetEnemyTier(tier);
		}
	}
}
