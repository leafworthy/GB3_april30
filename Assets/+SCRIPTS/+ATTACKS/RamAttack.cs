using System;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class RamAttack : MonoBehaviour, INeedPlayer
	{
		ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>(); //wtf
		ICanAttack _attacker;
		Life life => _life ??= GetComponent<Life>();
		Life _life;
		public bool causesFlying;
		float currentCooldown;
		float coolDown = .5f;
		MoveAbility mover;
		[SerializeField] float pushBackAmount = 3;
		[SerializeField] float extraPushAmount = 3;
		public float flyHeight = 2.5f;
		public event Action OnAttackHit;

		public void SetPlayer(Player newPlayer)
		{
			mover = GetComponent<MoveAbility>();
		}

		void OnDrawGizmos()
		{
			//MyDebugUtilities.DrawCircle(transform.position, attacker.stats.Stats.Range(1), Color.red);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			CheckForHit(other.gameObject);
		}

		void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			CheckForEnemiesInRange();
			Cooldown();
		}

		void CheckForEnemiesInRange()
		{
			var enemies = Physics2D.OverlapCircleAll(transform.position, attacker.stats.Stats.Range(1), Services.assetManager.LevelAssets.PlayerLayer).ToList();
			enemies.AddRange(Physics2D.OverlapCircleAll(transform.position, attacker.stats.Stats.Range(1), Services.assetManager.LevelAssets.BuildingLayer)
			                          .ToList());
			if (enemies.Count <= 0) return;
			foreach (var enemy in enemies)
			{
				CheckForHit(enemy.gameObject);
			}
		}

		void Cooldown()
		{
			if (!(currentCooldown > 0)) return;
			currentCooldown -= Time.fixedDeltaTime;
			if (!(currentCooldown <= 0)) return;
			currentCooldown = 0;
		}

		void CheckForHit(GameObject other)
		{
			if (life.IsDead()) return;
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

		void AttackHit(Life other)
		{
			currentCooldown = coolDown;
			var otherAttack = Attack.Create(attacker, other).WithDamage(attacker.stats.Stats.Damage(1)).WithFlying(causesFlying, flyHeight)
			                        .WithExtraPush(extraPushAmount);
			other.TakeDamage(otherAttack);
			if(other.IsDead()) return;

			var bouncebackAttack = Attack.Create(attacker, life).WithDamage(attacker.stats.Stats.Damage(1)).WithOriginPoint(other.transform.position)
			                             .WithDestinationPoint(transform.position);
			life.TakeDamage(bouncebackAttack);
			mover.Push(bouncebackAttack.Direction, pushBackAmount);
			OnAttackHit?.Invoke();
		}
	}
}
