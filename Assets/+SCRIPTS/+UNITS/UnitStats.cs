using System;
using Sirenix.Serialization;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class UnitStats
	{
		[SerializeField] UnitStatsData _unitData;
		public UnitStatsData Data => _unitData ??= UnitStatsManager.GetUnitStats(unitName);
		string unitName;

		public UnitStats(string _name)
		{
			unitName = _name;
			_unitData = UnitStatsManager.GetUnitStats(unitName);
		}


		[OdinSerialize]public int EnemyTier { get; set; }
		const float ShieldDamageFactor = .1f;
		public event Action<Attack> OnDead;


		#region Health

		public float CurrentHealth { get; set; }
		public float RealMaxHealth => Data.healthMax;
		public float FractionWithExtraHealth => CurrentHealth / RealMaxHealth + GetExtraHealth();
		public bool IsTemporarilyInvincible { get; set; }
		public bool IsDead { get; set; }

		public bool IsShielded { get; set; }

		#endregion

		public void TakeDamage(Attack attack)
		{
			if (IsDead || IsTemporarilyInvincible) return;
			if (IsShielded) attack.DamageAmount *= ShieldDamageFactor;
			CurrentHealth = Mathf.Max(0, CurrentHealth - attack.DamageAmount);
			if (CurrentHealth <= 0) StartDeath(attack);
		}

		void StartDeath(Attack killingAttack)
		{
			Debug.Log("death starts");
			IsDead = true;
			OnDead?.Invoke(killingAttack);
		}

		public void AddHealth(float amount)
		{
			if (IsDead) return;
			CurrentHealth = Mathf.Min(RealMaxHealth + GetExtraHealth(), CurrentHealth + amount);
		}

		public bool CanTakeDamage() => !IsDead && !IsTemporarilyInvincible && !Data.isInvincible;
		public void FillHealth()
		{
			CurrentHealth = RealMaxHealth + GetExtraHealth();
		}



		public float ExtraHealthFactor;
		public float ExtraDamageFactor;
		public float ExtraSpeedFactor;

		public Color DebrisTint => Data.debrisTint;

		public float GetExtraHealth() => ExtraHealthFactor * Data.healthMax + Data.healthMax * EnemyTier;
		public float MoveSpeed => Data.moveSpeed + ExtraSpeedFactor * Data.moveSpeed + Data.moveSpeed * (EnemyTier * 0.3f);
		public float DashSpeed => Data.dashSpeed + ExtraSpeedFactor * Data.dashSpeed;
		public float JumpSpeed => Data.jumpSpeed;
		public float AggroRange => Data.aggroRange;
		public float AttackHeight => 5f;
		public bool IsObstacle => Data.category == UnitCategory.Obstacle;
		public bool IsPlayerAttackable => Data.isPlayerSwingHittable;
		public UnitCategory Category => Data.category;
		public DebrisType DebrisType => Data.debrisType;

		public float ExperienceGiven => Data.experienceGiven;
		public bool IsInvincible => Data.isInvincible;

		public float Damage(int attackIndex) => attackIndex switch
		                                        {
			                                        1 => Data.attack1Damage + ExtraDamageFactor * Data.attack1Damage + Data.attack1Damage * EnemyTier,
			                                        2 => Data.attack2Damage + ExtraDamageFactor * Data.attack2Damage + Data.attack2Damage * EnemyTier,
			                                        3 => Data.attack3Damage + ExtraDamageFactor * Data.attack3Damage + Data.attack3Damage * EnemyTier,
			                                        4 => Data.attack4Damage + ExtraDamageFactor * Data.attack4Damage + Data.attack4Damage * EnemyTier,
			                                        _ => 0f
		                                        };

		public float Range(int attackIndex) => attackIndex switch
		                                                {
			                                                1 => Data.attack1Range,
			                                                2 => Data.attack2Range,
			                                                3 => Data.attack3Range,
			                                                4 => Data.attack4Range,
			                                                _ => 0f
		                                                };

		public float Rate(int attackIndex) => attackIndex switch
		                                               {
			                                               1 => Data.attack1Rate,
			                                               2 => Data.attack2Rate,
			                                               3 => Data.attack3Rate,
			                                               4 => Data.attack4Rate,
			                                               _ => 0f
		                                               };


	}
}
