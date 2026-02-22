using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{

	[Serializable]
	public class ExplosionAttack : MonoBehaviour, INeedPlayer
	{
		private Life currentTargetLife;
		private ICanAttack _attacker;
		private ICanAttack attacker => _attacker ??= GetComponent<ICanAttack>();
		private UnitAnimations anim;
		private float explosionRadius = 8;
		public AnimationClip attackAnimation;
		private bool isAttacking;
		protected Life life => _life ?? GetComponent<Life>();
		private Life _life;

		public void SetPlayer(Player newPlayer)
		{
			anim = GetComponent<UnitAnimations>();
			anim.animEvents.OnAttackHit += AttackHit;
			attacker.OnAttack += AttackerAttack;
		}

		private void OnDisable()
		{

				attacker.OnAttack -= AttackerAttack;

			if (anim == null) return;
			if (anim.animEvents == null) return;
			anim.animEvents.OnAttackHit -= AttackHit;
		}

		private void AttackHit(int attackType)
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			if (currentTargetLife == null) return;

			Debug.Log(attacker.EnemyLayer  + " Enemy Layer  ") ;
			MyAttackUtilities.Explode(transform.position, explosionRadius, attacker.stats.Stats.Damage(1), attacker);
			life.DieNow();
		}

		private void AttackerAttack(Life newTarget)
		{
			if (newTarget == null) return;
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			if (isAttacking) return;
			isAttacking = true;
			var move = GetComponent<MoveAbility>();
			currentTargetLife = newTarget;
			anim.Play(attackAnimation.name, 0, 0);

			move.Push(currentTargetLife.transform.position - transform.position, 100);
		}


	}
}
