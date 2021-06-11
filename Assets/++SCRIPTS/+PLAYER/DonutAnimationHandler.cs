using UnityEngine;

namespace _SCRIPTS
{
	public class DonutAnimationHandler:MonoBehaviour
	{
		private Animator animator;
		private DefenceHandler defenceHandler;
		private EnemyAI ai;

		private static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
		private static readonly int AggroTrigger = Animator.StringToHash("AggroTrigger");
		private static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");

		private void Start()
		{
			ai = GetComponent<EnemyAI>();
			ai.OnAggro += AI_OnAggro;
			animator = GetComponentInChildren<Animator>();

			defenceHandler = GetComponent<DefenceHandler>();
			defenceHandler.OnDying += Defence_OnDying;
			defenceHandler.OnDamaged += Defence_OnDamaged;
		}

		private void AI_OnAggro()
		{
			animator.SetTrigger(AggroTrigger);
		}

		private void Defence_OnDamaged(Vector3 vector3, float f, Vector3 arg3, bool isPoison)
		{
			animator.SetTrigger(HitTrigger);
		}

		private void Defence_OnDying()
		{
			animator.SetTrigger(DeathTrigger);
		}



	}
}
