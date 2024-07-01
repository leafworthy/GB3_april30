using System;
using System.Linq;
using __SCRIPTS._ABILITIES;
using __SCRIPTS._COMMON;
using __SCRIPTS._MANAGERS;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._ATTACKS
{
	[Serializable]
	public class RamAttack : Attacks
	{
		private float currentCooldown;
		private float coolDown = 2;
		private bool isCooledDown;
		private UnitStats stats;
		private MoveAbility mover;
		private Life life;
		private Player owner;

		private void Start()
		{
			life = GetComponent<Life>();
			mover = GetComponent<MoveAbility>();
			stats = GetComponent<UnitStats>();
			owner = Players.EnemyPlayer;
			isCooledDown = true;
		}

		private void FixedUpdate()
		{
			if (GlobalManager.IsPaused) return;
			if (life.IsDead()) return;
			TryAttackingClosestEnemy();
			Cooldown();
		}

		private void TryAttackingClosestEnemy()
		{
			CheckForHit(FindClosestDoorHit());
		}

		private GameObject FindClosestDoorHit()
		{
			float hitRange = 5;
			var circleCast = Physics2D.OverlapCircleAll(transform.position, hitRange, ASSETS.LevelAssets.DoorLayer)
			                          .ToList();
			if (circleCast.Count <= 0) return null;

			var closest = circleCast[0];

			return closest.gameObject;
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

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (GlobalManager.IsPaused) return;
			CheckForHit(other.gameObject);
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (GlobalManager.IsPaused) return;
			CheckForHit(other.gameObject);
		}

		private void CheckForHit(GameObject other)
		{
			if (life == null) life = GetComponentInChildren<Life>();
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
			if (otherDefence.IsObstacle || otherDefence.IsPlayer)
			{
				AttackHit(otherDefence);
			}

		}

		private void AttackHit(Life other)
		{
			currentCooldown = coolDown;
			var otherAttack = new Attack(life, other, stats.AttackDamage);
			var bouncebackAttack = new Attack(other, life, stats.AttackDamage);
			other.TakeDamage(otherAttack);
			life.TakeDamage(bouncebackAttack);
			mover.Push(bouncebackAttack.Direction, bouncebackAttack.DamageAmount * 2);
		}
	}
}
