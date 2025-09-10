using __SCRIPTS._ENEMYAI;
using __SCRIPTS.Projectiles;
using UnityEngine;

namespace __SCRIPTS
{
	public class SlimeAttack : MonoBehaviour
	{
		[SerializeField] private GameObject ProjectilePrefab;
		private float currentCooldownTime;

		private Life life;
		private Body body;
		private UnitAnimations anim;
		private EnemyAI ai;
		private Life currentTarget;

		private void OnEnable()
		{
			life = GetComponent<Life>();
			body = GetComponent<Body>();
			anim = GetComponent<UnitAnimations>();

			ai = GetComponent<EnemyAI>();

			ai.OnAttack += AI_Attack;
			anim.animEvents.OnAttackHit += CreateSlime;
		}

		private void OnDisable()
		{
			ai.OnAttack -= AI_Attack;
			anim.animEvents.OnAttackHit -= CreateSlime;
		}


		private void AI_Attack(Life _life)
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			AttackTarget(_life);
		}

		private void AttackTarget(Life targetLife)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			currentTarget = targetLife;
			currentCooldownTime = Time.time + life.PrimaryAttackRate;

			// Face the target only when starting a new attack
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

			var newAttack = new Attack(life, currentTarget, life.PrimaryAttackDamageWithExtra);

			if ((Vector2.Distance(transform.position, currentTarget.transform.position) <= life.PrimaryAttackRange) ||  (currentTarget.IsObstacle))
			{
				currentTarget.TakeDamage(newAttack);
			}

			var targetPos = (currentTarget.transform.position -transform.position).normalized * life.PrimaryAttackRange + transform.position;
			CreateSlimePrefab(targetPos);
		}


		private void CreateSlimePrefab(Vector2 pos)
		{
			var newProjectile = Services.objectMaker.Make(ProjectilePrefab, pos);
			var projectileScript = newProjectile.GetComponent<SlimePool>();
			var directionMult = body.BottomIsFacingRight ? 1 : -1;
			projectileScript.Fire(directionMult, life);
		}

	}
}
