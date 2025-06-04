using UnityEngine;

namespace GangstaBean.Objects.Projectiles.Projectiles
{
	public class SlimePool : MonoBehaviour
	{
		private Animator animator;
		private float deathTime = 8;
		private float poisonTime = 2;
		private float poisonDamage = 5;
		private AnimationEvents animationEvents;
		private bool isDead;
		private static readonly int IsDead = Animator.StringToHash("isDead");
		private static readonly int Birth = Animator.StringToHash("Birth");
		private Life owner;
		public void Fire(int directionMult, Life _owner)
		{
			owner = _owner;
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
			if (animator == null) return;
			animator.SetTrigger(Birth);
			animator.SetBool(IsDead, false);
			Invoke("Die", deathTime);
			isDead = false;
		}

		private void Die()
		{
			animationEvents = GetComponentInChildren<AnimationEvents>();
			animationEvents.OnDieStop += Dead;
			animator = GetComponentInChildren<Animator>();
			if (animator == null) return;
			animator.SetBool(IsDead, true);
			isDead = true;
		}

	

		private void Dead()
		{
			animationEvents.OnDieStop -= Dead;
			ObjectMaker.I.Unmake(gameObject);
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (isDead) return;
			if (other.transform == transform) return;
			var defence = other.GetComponentInChildren<Life>();
			if (defence is null) return;
			if (!defence.IsPlayer) return;
			var poison = defence.gameObject.GetComponent<PoisonDamageEffect>() ?? defence.gameObject.AddComponent<PoisonDamageEffect>();

			poison.StartPoisonEffect(poisonTime, poisonDamage, defence, owner);


		}

	}
}