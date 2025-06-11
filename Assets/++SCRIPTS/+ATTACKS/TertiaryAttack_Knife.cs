using System;
using System.Linq;
using __SCRIPTS;
using UnityEngine;

public class TertiaryAttack_Knife : Attacks
	{
		private Player player;
		private Body body;
		private Animations anim;
		public override string VerbName => "Knife-Attack";
		private string AnimationClipName = "Top-Bean_Knife_Attack";

		private bool isAttacking;
		private bool isPressing;
		public GameObject attackPoint;
		public event Action OnMiss;
		public event Action<Vector2> OnHit;

		public override void SetPlayer(Player _player)
		{
			 base.SetPlayer(_player);
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			player = _player;
			player.Controller.Attack3Circle.OnPress += PlayerKnifePress;
			player.Controller.Attack3Circle.OnRelease += PlayerKnifeRelease;
			anim.animEvents.OnAttackHit += Anim_AttackHit;
			anim.animEvents.OnAttackStop += Anim_AttackStop;
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (anim == null) return;
			player.Controller.Attack3Circle.OnPress -= PlayerKnifePress;
			player.Controller.Attack3Circle.OnRelease -= PlayerKnifeRelease;
			anim.animEvents.OnAttackHit -= Anim_AttackHit;
			anim.animEvents.OnAttackStop -= Anim_AttackStop;
		}
		private void Anim_AttackStop(int obj)
		{
			Debug.Log("knife stop");
			anim.SetBool(Animations.IsBobbing, true);
			body.arms.StopSafely(this);
			isAttacking = false;
			if (!isPressing) return;
			Debug.Log("knife start again");
			PlayerKnifePress(null);
		}



		private void PlayerKnifeRelease(NewControlButton newControlButton)
		{
			isPressing = false;
		}

		private void PlayerKnifePress(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			isPressing = true;
			StartAttack();
		}

		private void StartAttack()
		{
			if(isAttacking)
			{
				Debug.Log("can't knife, still knifing");
				return;
			}
			if (!body.arms.Do(this))
			{
				Debug.Log("can't knife, still busy " + body.arms.currentActivity);
				return;
			}
			isAttacking = true;
			Debug.Log("knife start");
			anim.Play(AnimationClipName, 1, 0);
			anim.SetTrigger(Animations.KnifeTrigger);
			anim.SetBool(Animations.IsBobbing, false);
		}

		private GameObject FindClosestHit()
		{

			var circleCast = Physics2D.OverlapCircleAll(attackPoint.transform.position, attacker.TertiaryAttackRange, ASSETS.LevelAssets.EnemyLayer)
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
