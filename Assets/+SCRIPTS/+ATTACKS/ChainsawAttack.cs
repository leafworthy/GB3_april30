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

		public override string VerbName => "Chainsaw-Attack";

		private bool isAttacking;
		private bool isChainsawing;
		private bool isPressingChainsawButton;
		public GameObject attackPoint;
		public event Action OnMiss;
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

			if (isAttacking && attacker != null)
			{
				counter += Time.fixedDeltaTime;
				if (counter >= attacker.TertiaryAttackRate)
				{
					counter = 0;
					HitClosest();
				}
			}
		}

		private void HitClosest()
		{
			var enemyHit = FindClosestHits();
			if (enemyHit == null || enemyHit.Count == 0)
			{
				OnMiss?.Invoke();
				return;
			}

			foreach (var enemy in enemyHit)
			{
				if (enemy == null) continue;
				if (enemy.IsHuman || enemy.CanTakeDamage || enemy.IsObstacle) continue;
				if (enemy.IsDead()) continue;
				HitTarget(attacker.TertiaryAttackDamageWithExtra, enemy, 1);
			}
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
			// Don't occupy legs - allow movement while chainsawing

			if (body.arms.isActive && body.arms.currentActivity?.VerbName != VerbName)
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

		private List<Life> FindClosestHits()
		{
			// Null checks
			if (attackPoint == null || attacker == null || Services.assetManager.LevelAssets == null)
			{
				return new List<Life>();
			}

			var circleCast = Physics2D.OverlapCircleAll(attackPoint.transform.position, attacker.TertiaryAttackRange, Services.assetManager.LevelAssets.EnemyLayer).ToList();
			if (circleCast.Count <= 0) return new List<Life>();

			var enemies = new List<Life>();
			foreach (var enemyCollider in circleCast)
			{
				if (enemyCollider == null || enemyCollider.gameObject == null) continue;

				var enemy = enemyCollider.gameObject.GetComponent<Life>();
				if (enemy == null) enemy = enemyCollider.gameObject.GetComponentInParent<Life>();
				if (enemy == null) continue;
				if (enemy.IsHuman || enemy.CanBeAttacked || enemy.IsObstacle) continue;
				enemies.Add(enemy);
			}

			return enemies;
		}
	}
}
