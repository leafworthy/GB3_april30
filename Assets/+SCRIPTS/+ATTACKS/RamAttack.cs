using System;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class RamAttack : MonoBehaviour, INeedPlayer
	{
		private ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>(); //wtf
		private ICanAttack _attacker;
		private IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
		private IGetAttacked _life;
		public bool causesFlying;
		private float currentCooldown;
		private float coolDown = .5f;
		private bool isCooledDown;
		private MoveAbility mover;
		[SerializeField] private float pushBackAmount = 3;
		[SerializeField] private float extraPushAmount = 3;
		public float flyHeight = 2;
		public event Action OnAttackHit;

		public void SetPlayer(Player newPlayer)
		{
			mover = GetComponent<MoveAbility>();
			isCooledDown = true;
		}

		private void OnDrawGizmos()
		{
			//MyDebugUtilities.DrawCircle(transform.position, attacker.stats.Stats.Range(1), Color.red);
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
			var enemies = Physics2D.OverlapCircleAll(transform.position, attacker.stats.Stats.Range(1), Services.assetManager.LevelAssets.PlayerLayer).ToList();
			enemies.AddRange(Physics2D.OverlapCircleAll(transform.position, attacker.stats.Stats.Range(1), Services.assetManager.LevelAssets.BuildingLayer).ToList());
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
			var otherAttack = Attack.Create(attacker, other).WithDamage(attacker.stats.Stats.Damage(1)).WithFlying(causesFlying, flyHeight).WithExtraPush(extraPushAmount);
			other.TakeDamage(otherAttack);


				var bouncebackAttack = Attack.Create(attacker, life).WithDamage(attacker.stats.Stats.Damage(1)).WithOriginPoint(other.transform.position).WithDestinationPoint(transform.position);
				life.TakeDamage(bouncebackAttack);
				mover.Push(bouncebackAttack.Direction, pushBackAmount);
				OnAttackHit?.Invoke();
		}
	}
}
