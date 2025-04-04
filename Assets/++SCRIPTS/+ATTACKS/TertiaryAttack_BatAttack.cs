using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class TertiaryAttack_BatAttack : Attacks
	{
		private AnimationEvents animEvents;
		private JumpAbility jumps;
		private Animations anim;
		private Body body;
		private const string batAttackVerbName = "bat attacking";
		private bool isCharging;
		private Life life;

		private bool isFullyCharged;
		private bool isPressingAttack;
		private bool isAttacking;
		private Player owner;
		private float extraPush = 1.75f;
		private Vector2 moveDir;
		public event Action OnSwing;
		public event Action<Vector2> OnHit;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			body = GetComponent<Body>();
			anim = GetComponent<Animations>();
			life = GetComponent<Life>();
			owner = life.player;
			jumps = GetComponent<JumpAbility>();

			if (owner == null) return;
			owner.Controller.Attack3Circle.OnPress += Player_AttackPress;
			owner.Controller.Attack3Circle.OnRelease += Player_AttackRelease;
			owner.Controller.MoveAxis.OnChange += Player_MoveInDirection;
			animEvents = anim.animEvents;
			animEvents.OnAttackHit += Anim_AttackHit;
			animEvents.OnAttackStop += Anim_AttackStop;
		}

		private void OnDisable()
		{
			if (owner == null) return;
			owner.Controller.Attack3Circle.OnPress -= Player_AttackPress;
			owner.Controller.Attack3Circle.OnRelease -= Player_AttackRelease;
			owner.Controller.MoveAxis.OnChange -= Player_MoveInDirection;
			if (animEvents == null) return;
			animEvents.OnAttackHit -= Anim_AttackHit;
			animEvents.OnAttackStop -= Anim_AttackStop;
		}

		private void Player_MoveInDirection(NewInputAxis arg1, Vector2 dir)
		{
			moveDir = dir;
		}

		private void Player_AttackRelease(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		private void Anim_AttackStop(int attackType)
		{
			isAttacking = false;
			body.arms.StopSafely();
			body.legs.StopSafely();

			if (attackType == 0)
			{
				anim.ResetTrigger(Animations.ChargeAttackTrigger);
				anim.ResetTrigger(Animations.ChargeStartTrigger);
			}

			if (isPressingAttack) Player_AttackPress(null);
		}

		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 5)
				RegularAttackHit(attackType);
		}

		private void RegularAttackHit(int attackType)
		{
			var circleCast = Physics2D.OverlapCircleAll(transform.position, GetHitRange(attackType), ASSETS.LevelAssets.EnemyLayer);
			var closest2 = circleCast.OrderBy(item => Vector2.Distance(item.gameObject.transform.position, transform.position)).Take(2);
			foreach (var col in closest2)
			{
				if (col == null) return;
				var _life = col.gameObject.GetComponent<Life>();
				if (!_life.isEnemyOf(attacker) || _life.cantDie) return;
				if (attacker.IsPlayer && _life.IsObstacle) return;
				HitTarget(GetAttackDamage(attackType), _life, extraPush);
				OnHit?.Invoke(col.gameObject.transform.position);
			}
		}

		private float GetHitRange(int attackType)
		{
			if (attackType == 5)
				return life.TertiaryAttackRange * 3;
			return life.TertiaryAttackRange;
		}

		private float GetAttackDamage(int attackType)
		{
			var extraDamageFactor = owner.spawnedPlayerDefence.ExtraMaxDamageFactor;
			return attackType switch
			       {
				       1 => 50 + 50 * extraDamageFactor,
				       2 => 50 + 50 * extraDamageFactor,
				       3 => 100 + 100 * extraDamageFactor,
				       4 => 100 + 100 * extraDamageFactor,
				       _ => 0
			       };
		}

		private void Player_AttackPress(NewControlButton newControlButton)
		{
			isPressingAttack = true;
			if (isAttacking) return;
			if (jumps.IsJumping)
			{
				StartJumpAttack();
				return;
			}

			body.BottomFaceDirection(moveDir.x >= 0);
			StartRandomAttack();
		}

		private void StartRandomAttack()
		{
			if (!body.arms.Do(batAttackVerbName))
			{
				if (body.arms.currentActivity != JumpAbility.VerbName)
					return;
			}

			if (!body.legs.Do(batAttackVerbName)) return;
			if (isAttacking) return;
			isAttacking = true;

			var randomAttack = Random.Range(1, 3);
			switch (randomAttack)
			{
				case 1:
					anim.SetTrigger(Animations.Attack1Trigger);
					break;
				case 2:
					anim.SetTrigger(Animations.Attack2Trigger);
					break;
				default:
					anim.SetTrigger(Animations.Attack3Trigger);
					break;
			}

			OnSwing?.Invoke();
		}

		private void StartJumpAttack()
		{
			if (isAttacking) return;
			OnSwing?.Invoke();
			isAttacking = true;
			anim.SetTrigger(Animations.JumpAttackTrigger);
		}
	}
}