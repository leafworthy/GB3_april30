using System;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, Serializable]
	public class RamAttack : MonoBehaviour, INeedPlayer
	{
		private ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>();
		private ICanAttack _attacker;
		private IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
		private IGetAttacked _life;
		public bool hasBounceBack = true;
		public bool causesFlying;
		private float currentCooldown;
		private float coolDown = .5f;
		private bool isCooledDown;
		private MoveAbility mover;
		[SerializeField] private float pushBackAmount = 3;

		public void SetPlayer(Player newPlayer)
		{
			mover = GetComponent<MoveAbility>();
			isCooledDown = true;
		}

		private void OnDrawGizmos()
		{
			MyDebugUtilities.DrawCircle(transform.position, attacker.stats.PrimaryAttackRange, Color.red);
		}

		private void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			CheckForEnemiesInRange();
			Cooldown();
		}

		private void CheckForEnemiesInRange()
		{
			var enemies = Physics2D.OverlapCircleAll(transform.position, attacker.stats.PrimaryAttackRange, Services.assetManager.LevelAssets.PlayerLayer).ToList();
			enemies.AddRange(Physics2D.OverlapCircleAll(transform.position, attacker.stats.PrimaryAttackRange, Services.assetManager.LevelAssets.DoorLayer).ToList());
			if (enemies.Count <= 0) return;
			foreach (var enemy in enemies)
			{
				CheckForHit(enemy.gameObject);
			}
		}

		private void Cooldown()
		{
			if (!(currentCooldown > 0)) return;
			isCooledDown = false;
			currentCooldown -= Time.fixedDeltaTime;
			if (!(currentCooldown <= 0)) return;
			currentCooldown = 0;
			isCooledDown = true;
		}

		private void CheckForHit(GameObject other)
		{
			if (life.IsDead()) return;
			if (!isCooledDown) return;
			if (other == null) return;
			if (other.transform == transform) return;
			var otherDefence = other.GetComponentInChildren<Life>();
			if (otherDefence == null) return;
			if (otherDefence.IsDead()) return;
			var otherJump = other.GetComponentInChildren<JumpAbility>();
			if (otherJump != null)
			{
				if (!otherJump.IsResting)
					return;
			}


			AttackHit(otherDefence);
		}

		private void AttackHit(IGetAttacked other)
		{
			currentCooldown = coolDown;
			var otherAttack = Attack.Create(attacker, other).WithDamage(attacker.stats.PrimaryAttackDamageWithExtra).WithFlying(causesFlying);
			other.TakeDamage(otherAttack);

			//WEIRD
			if (!hasBounceBack)
			{
				var bouncebackAttack = Attack.Create(attacker, life).WithDamage(attacker.stats.PrimaryAttackDamageWithExtra);
				life.TakeDamage(bouncebackAttack);
				mover.Push(bouncebackAttack.Direction, pushBackAmount);
			}
		}
	}
}
