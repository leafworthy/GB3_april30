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
		private Life life;
		private MoveAbility mover;
		private Player owner;
		[SerializeField]private float pushBackAmount = 3;

		private void Start()
		{
			life = GetComponentInChildren<Life>();
			mover = GetComponent<MoveAbility>();
			owner = Players.EnemyPlayer;
			isCooledDown = true;
		}

		private void FixedUpdate()
		{
			if (PauseManager.IsPaused) return;
			if (life.IsDead()) return;
			CheckForEnemiesInRange();
			Cooldown();
		}

		private void CheckForEnemiesInRange()
		{
			var enemies = Physics2D.OverlapCircleAll(transform.position, life.PrimaryAttackRange, ASSETS.LevelAssets.PlayerLayer)
			                       .ToList();
			enemies.AddRange(Physics2D.OverlapCircleAll(transform.position, life.PrimaryAttackRange, ASSETS.LevelAssets.DoorLayer).ToList());
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
			var otherAttack = new Attack(life, other, life.PrimaryAttackDamageWithExtra);
			var bouncebackAttack = new Attack(other, life, life.PrimaryAttackDamageWithExtra);
			other.TakeDamage(otherAttack);
			life.TakeDamage(bouncebackAttack);
			mover.Push(bouncebackAttack.Direction, pushBackAmount);
			Debug.Log("pushing");
		}
	}
}