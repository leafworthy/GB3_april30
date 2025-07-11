using System;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class RamAttack : Attacks
	{
		private float currentCooldown;
		private float coolDown = .5f;
		private bool isCooledDown;
		private MoveAbility mover;
		[SerializeField]private float pushBackAmount = 3;

		public override string VerbName => "Ram-Attack";
		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			mover = GetComponent<MoveAbility>();
			isCooledDown = true;
		}


		private void FixedUpdate()
		{
			if (pauseManager.IsPaused) return;
			if (attacker.IsDead()) return;
			CheckForEnemiesInRange();
			Cooldown();
		}

		private void CheckForEnemiesInRange()
		{
			var enemies = Physics2D.OverlapCircleAll(transform.position, attacker.PrimaryAttackRange, assets.LevelAssets.PlayerLayer)
			                       .ToList();
			enemies.AddRange(Physics2D.OverlapCircleAll(transform.position, attacker.PrimaryAttackRange, assets.LevelAssets.DoorLayer).ToList());
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
			if (attacker.IsDead()) return;
			if (!isCooledDown) return;
			if (other == null) return;
			if (other.transform == transform) return;
			var otherDefence = other.GetComponentInChildren<Life>();
			var otherJump = other.GetComponentInChildren<JumpAbility>();
			if (otherJump != null)
			{
				if(!otherJump.isResting) return;
			}
			if (otherDefence == null) return;
			if (otherDefence.IsObstacle)
			{
				var door = other.GetComponentInChildren<DoorInteraction>();
				if(door == null || door.isOpen || door.isBroken) return;

			}
			else
			{
				if (otherDefence.IsDead()) return;
			}

			AttackHit(otherDefence);

		}

		private void AttackHit(Life other)
		{
			currentCooldown = coolDown;
			var otherAttack = new Attack(attacker, other, attacker.PrimaryAttackDamageWithExtra);
			var bouncebackAttack = new Attack(other, attacker, attacker.PrimaryAttackDamageWithExtra);
			other.TakeDamage(otherAttack);
			attacker.TakeDamage(bouncebackAttack);
			mover.Push(bouncebackAttack.Direction, pushBackAmount);

		}
	}
}
