using System;
using System.Linq;
using FunkyCode;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways]
	[Serializable]
	public class RamAttack : Attacks
	{
		public bool hasBounceBack = true;
		public bool causesFlying;
		private float currentCooldown;
		private float coolDown = .5f;
		private bool isCooledDown;
		private MoveAbility mover;
		[SerializeField] private float pushBackAmount = 3;

		public override string AbilityName => "Ram-Attack";

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			mover = GetComponent<MoveAbility>();
			isCooledDown = true;
		}

		private void OnDrawGizmos()
		{
			MyDebugUtilities.DrawCircle(transform.position, life.PrimaryAttackRange, Color.red);
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
			var enemies = Physics2D.OverlapCircleAll(transform.position, life.PrimaryAttackRange, Services.assetManager.LevelAssets.PlayerLayer).ToList();
			enemies.AddRange(Physics2D.OverlapCircleAll(transform.position, life.PrimaryAttackRange, Services.assetManager.LevelAssets.DoorLayer).ToList());
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
				if (!otherJump.IsResting)
					return;
			if (otherDefence == null) return;
			if (otherDefence.IsObstacle)
			{
				var door = other.GetComponentInChildren<DoorInteraction>();
				if (door == null || door.isOpen || door.isBroken) return;
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
			 var otherAttack = Attack.Create( life,other).WithDamage(life.PrimaryAttackDamageWithExtra);

			otherAttack.CausesFlying = causesFlying;
			other.TakeDamage(otherAttack);

			//WEIRD
			if (!hasBounceBack)
			{
				var bouncebackAttack =  Attack.Create(other, life).WithDamage(life.PrimaryAttackDamageWithExtra);
				life.TakeDamage(bouncebackAttack);
				mover.Push(bouncebackAttack.Direction, pushBackAmount);
			}
		}
	}
}
