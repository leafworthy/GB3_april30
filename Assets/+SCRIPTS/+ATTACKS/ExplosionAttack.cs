using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{

	[Serializable]
	public class ExplosionAttack : MonoBehaviour, INeedPlayer
	{
		private Life currentTargetLife;
		private IAttack _attacker;
		private IAttack attacker => _attacker ??= GetComponent<IAttack>();
		private UnitAnimations anim;
		private float explosionRadius = 5;
		public AnimationClip attackAnimation;
		private bool isAttacking;
		protected Life life => _life ?? GetComponent<Life>();
		private Life _life;

		public void SetPlayer(Player _player)
		{
			anim = GetComponent<UnitAnimations>();
			anim.animEvents.OnAttackHit += AttackHit;
			attacker.OnAttack += AttackerAttack;
		}

		private void OnDisable()
		{
			if (life.IsHuman)
				life.Player.Controller.Attack1RightTrigger.OnPress -= Player_Attack;
			else
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

			AttackUtilities.Explode(transform.position, explosionRadius, life.PrimaryAttackDamageWithExtra, life);
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
			move.StopMoving();
			move.Push(currentTargetLife.transform.position - transform.position, 4);
		}

		private void Player_Attack(NewControlButton newControlButton)
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;

			var hitObject = AttackUtilities.RaycastToObject(currentTargetLife,
				life.IsHuman ? Services.assetManager.LevelAssets.EnemyLayer : Services.assetManager.LevelAssets.PlayerLayer);
			if (hitObject.collider == null) return;

			currentTargetLife = hitObject.collider.gameObject.GetComponent<Life>();
			if (currentTargetLife == null) return;
			anim.Play(attackAnimation.name, 0, 0);
			//anim.SetTrigger(UnitAnimations.Attack1Trigger);
		}
	}
}
