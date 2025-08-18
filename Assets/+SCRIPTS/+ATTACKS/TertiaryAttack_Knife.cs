using System;
using System.Linq;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class TertiaryAttack_Knife : Attacks
	{
		private Player player;
		private Body body;
		private UnitAnimations anim;
		public override string VerbName => "Knife-Attack";
		private string AnimationClipName = "Top-Bean_Knife_Attack";

		private bool isAttacking;
		private bool isPressing;
		public GameObject attackPoint;
		public event Action OnMiss;
		public event Action<Vector2> OnHit;

		public bool TryCompleteGracefully(CompletionReason reason, IActivity newActivity = null)
		{
			switch (reason)
			{
				case CompletionReason.AnimationInterrupt:
				case CompletionReason.NewActivity:
					// Handle graceful completion
					isAttacking = false;
					isPressing = false;
					anim.SetBool(UnitAnimations.IsBobbing, true);
					return true;
			}
			return false;
		}

		public override void SetPlayer(Player _player)
		{
			 base.SetPlayer(_player);
			anim = GetComponent<UnitAnimations>();
			body = GetComponent<Body>();
			this.player = _player;
			Debug.Log("[Knife] SetPlayer called, connecting input events");
			this.player.Controller.OnAttack3_Pressed += PlayerKnifePress;
			this.player.Controller.OnAttack3_Released += PlayerKnifeRelease;
			anim.animEvents.OnAttackHit += Anim_AttackHit;
			anim.animEvents.OnAttackStop += Anim_AttackStop;
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (anim == null) return;
			player.Controller.OnAttack3_Pressed -= PlayerKnifePress;
			player.Controller.OnAttack3_Released -= PlayerKnifeRelease;
			anim.animEvents.OnAttackHit -= Anim_AttackHit;
			anim.animEvents.OnAttackStop -= Anim_AttackStop;
		}
		private void Anim_AttackStop(int obj)
		{

			anim.SetBool(UnitAnimations.IsBobbing, true);
			body.doableArms.Stop(this);
			isAttacking = false;
			if (!isPressing) return;

			PlayerKnifePress(null);
		}



		private void PlayerKnifeRelease(NewControlButton newControlButton)
		{
			isPressing = false;
		}

		private void PlayerKnifePress(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused)
			{
				Debug.Log("[Knife] BLOCKED: Game is paused");
				return;
			}
			Debug.Log("[Knife] Player pressed knife button");
			isPressing = true;
			StartAttack();
		}

		private void StartAttack()
		{
			Debug.Log($"[Knife] StartAttack called, isAttacking: {isAttacking}");
			if(isAttacking)
			{
				Debug.Log("[Knife] BLOCKED: Already attacking");
				return;
			}

			Debug.Log("[Knife] SUCCESS: Starting knife attack");
			isAttacking = true;

			anim.Play(AnimationClipName, 1, 0);
			anim.SetTrigger(UnitAnimations.KnifeTrigger);
			anim.SetBool(UnitAnimations.IsBobbing, false);
		}

		private GameObject FindClosestHit()
		{

			var circleCast = Physics2D.OverlapCircleAll(attackPoint.transform.position, attacker.TertiaryAttackRange, assetManager.LevelAssets.EnemyLayer)
			                          .ToList();
			if (circleCast.Count <= 0) return null;

			var closest = circleCast[0];
			foreach (var col in circleCast)
			{
				var colStats = col.GetComponentInChildren<Life>();
				if (colStats.IsObstacle || !colStats.IsPlayerAttackable) continue;
				if (colStats.cantDie) continue;
				if (Vector2.Distance(col.gameObject.transform.position, transform.position) <
				    Vector2.Distance(closest.transform.position, transform.position))
					closest = col;
			}

			return closest.gameObject;
		}

		private void Anim_AttackHit(int attackType)
		{
			if (attackType != 3) return;

			var enemyHit = FindClosestHit();
			if (enemyHit == null)
			{
				OnMiss?.Invoke();
				return;
			}
			var enemy = enemyHit.transform.gameObject.GetComponent<Life>();
			if (enemy == null)
			{
				enemy = enemyHit.transform.gameObject.GetComponentInParent<Life>();
			}

			if (enemy == null || enemy.IsPlayer || enemy.cantDie || enemy.IsObstacle)
			{
				OnMiss?.Invoke();
				return;
			}
			OnHit?.Invoke(enemyHit.transform.position);

			HitTarget(attacker.TertiaryAttackDamageWithExtra, enemy, 2);

		}

	}
