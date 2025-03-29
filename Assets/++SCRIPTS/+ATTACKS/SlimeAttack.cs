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
		private Animations anim;
		private EnemyAI ai;
		private MoveAbility mover;
		private Life currentTarget;
		private Targetter targets;

		private void Start()
		{
			Activate();
		}

		private void Activate()
		{
			targets = GetComponent<Targetter>();
			mover = GetComponent<MoveAbility>();
			life = GetComponent<Life>();
			body = GetComponent<Body>();
			anim = GetComponent<Animations>();

			ai = GetComponent<EnemyAI>();
		
			ai.OnAttack += AI_Attack;
			anim.animEvents.OnAttackHit += CreateSlime;
		}

		private void CleanUp()
		{
			ai.OnAttack -= AI_Attack;
			anim.animEvents.OnAttackHit -= CreateSlime;
		}
	

		private void AI_Attack(Life _life)
		{
			if (PauseManager.IsPaused) return;
			if (life.IsDead()) return;
			AttackTarget(_life);
		}

		private void AttackTarget(Life targetLife)
		{
			if (!(Time.time >= currentCooldownTime)) return;
			currentTarget = targetLife;
			currentCooldownTime = Time.time + life.PrimaryAttackRate;
			anim.SetTrigger(Animations.Attack1Trigger);
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
			var newProjectile = ObjectMaker.I.Make(ProjectilePrefab, pos);
			var projectileScript = newProjectile.GetComponent<SlimePool>();
			var directionMult = body.BottomIsFacingRight ? 1 : -1;
			projectileScript.Fire(directionMult, life);
		}

	}
}