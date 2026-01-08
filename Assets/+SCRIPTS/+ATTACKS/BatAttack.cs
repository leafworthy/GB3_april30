using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	public class BatAttack : Ability
	{
		public override string AbilityName => "Bat-Attack";
		public event Action OnSwing;
		public event Action<Vector2> OnHitTarget;

		public AnimationClip Attack1AnimationClip;
		public AnimationClip Attack2AnimationClip;
		public AnimationClip Attack3AnimationClip;

		bool isPressingAttack;
		float extraPush = 1.75f;
		JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
		JumpAbility _jumps;
		AimAbility aimAbility => _aimAbility ??= GetComponent<AimAbility>();
		AimAbility _aimAbility;

		int currentAttackIndex;
		MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		MoveAbility _moveAbility;

		const float Attack1Damage = 50;
		const float Attack2Damage = 60;
		const float Attack3Damage = 100;
		const float Attack4Damage = 200;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && !jumps.IsInAir;

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			player.Controller.Attack1RightTrigger.OnPress += Player_AttackPress;
			player.Controller.Attack3Circle.OnPress += Player_AttackPress;

			player.Controller.Attack1RightTrigger.OnRelease += Player_AttackRelease;
			player.Controller.Attack3Circle.OnRelease += Player_AttackRelease;
		}

		void OnDestroy()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= Player_AttackPress;
			player.Controller.Attack1RightTrigger.OnRelease -= Player_AttackRelease;
		}

		void Player_AttackPress(NewControlButton newControlButton)
		{
			isPressingAttack = true;
			Try();
		}

		protected override void DoAbility()
		{
			StartRandomAttack();
		}

		void Player_AttackRelease(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		void RegularAttackHit(int attackType)
		{
			var hits = MyAttackUtilities.CircleCastForXClosestTargets(offence, offence.stats.PrimaryAttackRange);
			if (hits == null) return;
			foreach (var hit in hits)
			{
				MyAttackUtilities.HitTarget(offence, hit, GetAttackDamage(attackType), extraPush);
				OnHitTarget?.Invoke(hit.transform.position);
			}
		}

		float GetAttackDamage(int attackType)
		{
			var extraDamageFactor = player.spawnedPlayerStats.ExtraDamageFactor;
			return attackType switch
			       {
				       1 => Attack1Damage + Attack1Damage * extraDamageFactor,
				       2 => Attack2Damage + Attack2Damage * extraDamageFactor,
				       3 => Attack3Damage + Attack3Damage * extraDamageFactor,
				       _ => Attack4Damage
			       };
		}

		void StartRandomAttack()
		{
			OnSwing?.Invoke();
			moveAbility.SetCanMove(false);
			body.BottomFaceDirection(aimAbility.AimDir.x >= 0);
			currentAttackIndex = Random.Range(1, 4);
			var attackAnimation = currentAttackIndex switch
			                      {
				                      1 => Attack1AnimationClip,
				                      2 => Attack2AnimationClip,
				                      _ => Attack3AnimationClip
			                      };
			PlayAnimationClip(attackAnimation);
			Invoke(nameof(CurrentAttackHit), attackAnimation.length / 4);
		}

		public override void Stop()
		{
			moveAbility.SetCanMove(true);
			base.Stop();
		}

		protected override void AnimationComplete()
		{
			Stop();
			if (isPressingAttack) Player_AttackPress(null);
		}

		void CurrentAttackHit()
		{
			RegularAttackHit(currentAttackIndex);
		}
	}
}
