using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Projectiles;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class SlimeAttack : MonoBehaviour
	{
		[SerializeField] private GameObject ProjectilePrefab;
		private float currentCooldownTime;

		private IGetAttacked life => _life ??= GetComponent<IGetAttacked>();
		private IGetAttacked _life;
		private Body body;
		private UnitAnimations anim;
		private ICanAttack attacker;
		private IGetAttacked currentTarget;

		private void Start()
		{
			body = GetComponent<Body>();
			anim = GetComponent<UnitAnimations>();

			attacker = GetComponent<ICanAttack>();

			attacker.OnAttack += AttackerAttack;
			anim.animEvents.OnAttackHit += CreateSlime;
		}

		private void OnDisable()
		{
			attacker.OnAttack -= AttackerAttack;
			anim.animEvents.OnAttackHit -= CreateSlime;
		}


		private void AttackerAttack(IGetAttacked target)
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			AttackTarget(target);
		}

		private void AttackTarget(IGetAttacked targetLife)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			currentTarget = targetLife;
			currentCooldownTime = Time.time + attacker.stats.Stats.Rate(1);

			// Face the target only when starting a new offence
			FaceTarget();

			anim.SetTrigger(UnitAnimations.Attack1Trigger);
		}

		private void FaceTarget()
		{
			if (currentTarget == null) return;

			// Calculate direction to target
			Vector2 directionToTarget = currentTarget.transform.position - transform.position;

			// Only update facing if the distance is significant to avoid flipping
			if (Mathf.Abs(directionToTarget.x) > 0.5f)
			{
				bool shouldFaceRight = directionToTarget.x > 0;
				body.BottomFaceDirection(shouldFaceRight);
			}
		}

		private void CreateSlime(int attackType)
		{
			if (currentTarget == null) return;

			var newAttack = Attack.Create(attacker,currentTarget).WithDamage(attacker.stats.Stats.Damage(1));

			if ((Vector2.Distance(transform.position, currentTarget.transform.position) <= attacker.stats.Stats.Range(1)))
			{
				currentTarget.TakeDamage(newAttack);
			}

			var targetPos = (currentTarget.transform.position -transform.position).normalized * attacker.stats.Stats.Range(1) + transform.position;
			CreateSlimePrefab(targetPos);
		}


		private void CreateSlimePrefab(Vector2 pos)
		{
			var newProjectile = Services.objectMaker.Make(ProjectilePrefab, pos);
			var projectileScript = newProjectile.GetComponent<SlimePool>();
			var directionMult = body.BottomIsFacingRight ? 1 : -1;
			projectileScript.Fire(directionMult, attacker);
		}

	}
}
