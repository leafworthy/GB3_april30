using UnityEngine;

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

	private void Defence_OnDamaged(Attack attack)
	{
		animator.SetTrigger(HitTrigger);
	}

	private void Defence_OnDying()
	{
		animator.SetTrigger(DeathTrigger);
	}



}
