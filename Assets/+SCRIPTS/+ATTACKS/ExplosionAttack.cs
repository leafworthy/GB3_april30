using System;
using __SCRIPTS._ENEMYAI;

namespace __SCRIPTS
{
	[Serializable]
	public class ExplosionAttack : Attacks
	{
		private Life currentTargetLife;
		private EnemyAI ai;
		private EnemyAI AI => ai ??= GetComponent<EnemyAI>();
		private UnitAnimations anim;
		private float explosionRadius = 5;
		public override string AbilityName => "ExplosionAttack";

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			anim = GetComponent<UnitAnimations>();
			anim.animEvents.OnAttackHit += AttackHit;
			if (life.IsHuman)
				life.Player.Controller.Attack1RightTrigger.OnPress += Player_Attack;
			else
				AI.OnAttack += AIAttack;
		}

		private void OnDisable()
		{
			if (life.IsHuman)
				life.Player.Controller.Attack1RightTrigger.OnPress -= Player_Attack;
			else
				AI.OnAttack -= AIAttack;

			if (anim == null) return;
			if (anim.animEvents == null) return;
			anim.animEvents.OnAttackHit -= AttackHit;
		}

		private void AttackHit(int attackType)
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			if (currentTargetLife == null) return;

			AttackUtilities.Explode(transform.position, explosionRadius, life.PrimaryAttackDamageWithExtra, life.Player);
			life.DieNow();
		}

		private void AIAttack(Life newTarget)
		{
			if (newTarget == null) return;
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			var move = GetComponent<MoveAbility>();
			currentTargetLife = newTarget;
			anim.SetTrigger(UnitAnimations.Attack1Trigger);
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
			anim.SetTrigger(UnitAnimations.Attack1Trigger);
		}
	}
}
