using UnityEngine;

public class SlimePool : MonoBehaviour
{
	private Animator animator;
	private float deathTime = 5;
	private float poisonTime = 3;
	private float poisonDamage = 5;
	private AnimationEvents animationEvents;
	private bool isDead;
	private static readonly int IsDead = Animator.StringToHash("isDead");
	private static readonly int Birth = Animator.StringToHash("Birth");


	public void Fire(int directionMult)
	{

		var tempScale = transform.localScale;
		if (directionMult < 0)
		{
			tempScale = transform.localScale * new Vector2(-1, 1);
			transform.localScale = tempScale;
		}
		else
		{
			tempScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
			transform.localScale = tempScale;
		}

		animator = GetComponentInChildren<Animator>();
		animator?.SetTrigger(Birth);
		animator?.SetBool(IsDead, false);
		Invoke("Die", deathTime);
		isDead = false;
	}

	private void Die()
	{
		animationEvents = GetComponentInChildren<AnimationEvents>();
		animationEvents.OnDieStop += Dead;
		animator = GetComponentInChildren<Animator>();
		if (animator is null) return;
		animator?.SetBool(IsDead, true);
		isDead = true;
	}

	private void Dead()
	{
		animationEvents.OnDieStop -= Dead;
		MAKER.Unmake(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (isDead) return;
		Debug.Log("trigger enter");
		if (other.transform == transform) return;
		var defence = other.GetComponent<DefenceHandler>();
		if (defence is null) return;
		if (!defence.IsPlayer()) return;
		Debug.Log("has defence");
		var poison = defence.gameObject.GetComponent<PoisonDamageEffect>();
		if (poison is null)
		{

			poison = defence.gameObject.AddComponent<PoisonDamageEffect>();
			poison.StartPoisonEffect(poisonTime, poisonDamage, defence);
			Debug.Log("adding component");
		}
		else
		{

			poison.StartPoisonEffect(poisonTime, poisonDamage, defence);
			Debug.Log("adding to poison");
		}

	}

}
