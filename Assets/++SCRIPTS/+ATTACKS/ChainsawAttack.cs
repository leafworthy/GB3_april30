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
		private Animations anim;

		public override string VerbName => "Chainsaw-Attack";

		private bool isAttacking;
		private bool isChainsawing;
		public GameObject attackPoint;
		public event Action OnMiss;

		public event Action<Vector2> OnStartChainsawing;
		public event Action<Vector2> OnStartAttacking;
		public event Action<Vector2> OnStopAttacking;
		public event Action<Vector2> OnStopChainsawing;
		private float counter;
		private float counterMax = 10;
		public AnimationClip chainsawClip;

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			player = _player;
			player.Controller.Attack3Circle.OnPress += PlayerChainsawPress;
			player.Controller.Attack3Circle.OnRelease += PlayerChainsawRelease;
			player.Controller.Attack1RightTrigger.OnPress += PlayerPrimaryPress;
			anim.animEvents.OnAttackStop += Anim_OnAttackStop;
		}

		private void Anim_OnAttackStop(int attack)
		{
			body.arms.StopSafely(this);
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
			anim.SetBool(Animations.IsAttacking, isAttacking);

			OnStartAttacking(transform.position);
		}

		private void StopAttacking()
		{
			isAttacking = false;
			anim.SetBool(Animations.IsAttacking, isAttacking);
			OnStopAttacking(transform.position);
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (anim == null) return;
			isAttacking = false;
			isChainsawing = false;
			player.Controller.Attack3Circle.OnPress -= PlayerChainsawPress;
			player.Controller.Attack3Circle.OnRelease -= PlayerChainsawRelease;
		}

		private void FixedUpdate()
		{
			if (PauseManager.I.IsPaused) return;
			if (isAttacking)
			{
				counter += Time.fixedDeltaTime;
				if (counter >= counterMax)
				{
					counter = 0;
					HitClosest();
				}
			}
		}

		private void HitClosest()
		{
			var enemyHit = FindClosestHits();
			if (enemyHit.Count == 0)
			{
				OnMiss?.Invoke();
				return;
			}
			else
			{
				foreach (var enemy in enemyHit)
				{
					if (enemy == null) continue;
					if (enemy.IsPlayer || enemy.cantDie || enemy.IsObstacle) continue;
					if (enemy.IsDead()) continue;
					HitTarget(attacker.TertiaryAttackDamageWithExtra, enemy, 1);
				}
			}
		}

		private void PlayerChainsawRelease(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			StopAttacking();
			OnStopAttacking?.Invoke(transform.position);
		}

		private void StartChainsawing()
		{
			if (isChainsawing)
			{
				StartAttacking();
				return;
			}
			if (!body.arms.Do(this)) return;
			if (!body.legs.Do(this))
			{
				body.arms.StopSafely(this);
				return;
			}

			if (body.arms.isActive && body.arms.currentActivity.VerbName != VerbName)
			{
				StopAttacking();
				StopChainsawing();
			}
			else
			{
				isChainsawing = true;
				anim.Play(chainsawClip.name, 1, 0);
				anim.SetBool(Animations.IsChainsawing, isChainsawing);
				OnStartChainsawing?.Invoke(transform.position);
			}
		}

		private void StopChainsawing()
		{
			if (!isChainsawing) return;
			if (isAttacking) return;
			isChainsawing = false;
			anim.SetBool(Animations.IsChainsawing, isChainsawing);
			body.arms.StopSafely(this);
			Debug.Log("chainsaw stop");
			OnStopChainsawing?.Invoke(transform.position);
		}



		private void PlayerChainsawPress(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			StartChainsawing();
		}

		private List<Life> FindClosestHits()
		{
			var circleCast = Physics2D.OverlapCircleAll(attackPoint.transform.position, attacker.TertiaryAttackRange,
				ASSETS.LevelAssets.EnemyLayer).ToList();
			if (circleCast.Count <= 0) return null;
			var enemies = new List<Life>();
			foreach (var enemyCollider in circleCast)
			{
				var enemy = enemyCollider.gameObject.GetComponent<Life>();
				if (enemy == null) enemy = enemyCollider.gameObject.GetComponentInParent<Life>();
				if (enemy == null) continue;
				if (enemy.IsPlayer || enemy.cantDie || enemy.IsObstacle) continue;
				enemies.Add(enemy);
			}

			return enemies;
		}
	}
}
