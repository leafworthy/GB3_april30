using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class ChainsawAttack : Attacks
	{
		private Player player;
		private Body body;
		private UnitAnimations anim;

		public override string AbilityName => "Chainsaw-Attack";

		private bool isAttacking;
		private bool isChainsawing;
		private bool isPressingChainsawButton;
		public GameObject attackPoint;
		private Vector2 moveDir;

		public event Action<Vector2> OnStartChainsawing;
		public event Action<Vector2> OnStartAttacking;
		public event Action<Vector2> OnStopAttacking;
		public event Action<Vector2> OnStopChainsawing;
		private float counter;
		public AnimationClip chainsawClip;

		public override void SetPlayer(Player _player)
		{
			anim = GetComponent<UnitAnimations>();
			body = GetComponent<Body>();
			player = _player;
			player.Controller.Attack3Circle.OnPress += PlayerChainsawPress;
			player.Controller.Attack3Circle.OnRelease += PlayerChainsawRelease;
			player.Controller.Attack1RightTrigger.OnPress += PlayerPrimaryPress;
			player.Controller.MoveAxis.OnChange += Player_MoveInDirection;

			// Listen for actions that should cancel chainsawing
			player.Controller.Attack2LeftTrigger.OnPress += PlayerMinePress;
			player.Controller.Jump.OnPress += PlayerJumpPress;
			player.Controller.DashRightShoulder.OnPress += PlayerDashPress;

			anim.animEvents.OnAttackStop += Anim_OnAttackStop;
		}

		private void Anim_OnAttackStop(int attack)
		{
			body.arms.Stop(this);
		}

		private void PlayerPrimaryPress(NewControlButton obj)
		{
			if (isAttacking) return;
			if (!isChainsawing) return;
			StopChainsawing();
		}

		private void StartAttacking()
		{
			isAttacking = true;
			anim.SetBool(UnitAnimations.IsAttacking, isAttacking);

			OnStartAttacking(transform.position);
		}

		private void StopAttacking()
		{
			isAttacking = false;
			anim.SetBool(UnitAnimations.IsAttacking, isAttacking);
			OnStopAttacking(transform.position);
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (anim == null) return;
			isAttacking = false;
			isChainsawing = false;
			isPressingChainsawButton = false;
			player.Controller.Attack3Circle.OnPress -= PlayerChainsawPress;
			player.Controller.Attack3Circle.OnRelease -= PlayerChainsawRelease;
			player.Controller.MoveAxis.OnChange -= Player_MoveInDirection;

			// Unsubscribe from cancel actions
			player.Controller.Attack2LeftTrigger.OnPress -= PlayerMinePress;
			player.Controller.Jump.OnPress -= PlayerJumpPress;
			player.Controller.DashRightShoulder.OnPress -= PlayerDashPress;
		}

		private void Player_MoveInDirection(NewInputAxis arg1, Vector2 dir)
		{
			moveDir = dir;
		}

		private void PlayerMinePress(NewControlButton obj)
		{
			if (isChainsawing || isAttacking)
			{
				CancelChainsawing();
			}
		}

		private void PlayerJumpPress(NewControlButton obj)
		{
			if (isChainsawing || isAttacking)
			{
				CancelChainsawing();
			}
		}

		private void PlayerDashPress(NewControlButton obj)
		{
			if (isChainsawing || isAttacking)
			{
				CancelChainsawing();
			}
		}

		private void CancelChainsawing()
		{
			isPressingChainsawButton = false;
			StopAttacking();
			StopChainsawing();
		}

		private void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;

			// Handle facing direction while chainsawing
			if (isChainsawing && moveDir != Vector2.zero && body != null)
			{
				body.BottomFaceDirection(moveDir.x > 0);
			}

			// Auto-start attacking if chainsawing but not attacking and button still pressed
			if (isChainsawing && !isAttacking && isPressingChainsawButton)
			{
				StartAttacking();
			}

			if (!isAttacking || life == null) return;
			counter += Time.fixedDeltaTime;
			if (!(counter >= life.TertiaryAttackRate)) return;
			counter = 0;
			HitClosest();
		}

		private void HitClosest()
		{
			AttackUtilities.HitTargetsWithinRange( life, attackPoint.transform.position,life.TertiaryAttackRange , life.TertiaryAttackDamageWithExtra, life.EnemyLayer);
		}

		private void PlayerChainsawRelease(NewControlButton newControlButton)
		{
			if (Services.pauseManager.IsPaused) return;
			isPressingChainsawButton = false;
			StopAttacking();
			OnStopAttacking?.Invoke(transform.position);
		}

		private void StartChainsawing()
		{
			if (isChainsawing) return; // Already chainsawing

			if (!body.arms.Do(this)) return;

			if (body.arms.isActive && body.arms.currentActivity?.AbilityName != AbilityName)
			{
				StopAttacking();
				StopChainsawing();
			}
			else
			{
				isChainsawing = true;
				anim.Play(chainsawClip.name, 1, 0);
				anim.SetBool(UnitAnimations.IsChainsawing, isChainsawing);
				OnStartChainsawing?.Invoke(transform.position);
			}
		}

		private void StopChainsawing()
		{
			if (!isChainsawing) return;
			if (isAttacking) return;
			isChainsawing = false;
			anim.SetBool(UnitAnimations.IsChainsawing, isChainsawing);
			body.arms.Stop(this);
			OnStopChainsawing?.Invoke(transform.position);
		}

		private void PlayerChainsawPress(NewControlButton newControlButton)
		{
			if (Services.pauseManager.IsPaused) return;
			isPressingChainsawButton = true;
			StartChainsawing();
		}


	}
}
