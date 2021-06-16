using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace _SCRIPTS
{

	[Serializable]
	public class ConeAttackHandler : MonoBehaviour, IAttackHandler
	{
		public event Action OnAttackStop;
		public event Action OnAttackStart;

		[SerializeField] private GameObject aimCenter;
		[SerializeField] private GameObject attackPoint;
		[SerializeField] private GameObject footPoint;

		private UnitStats stats;

		private float currentCooldownTime;
		private EnemyController controller;
		private bool isOn;
		private AnimationEvents animationEvents;
		public Vector3 currentAttackTarget;
		protected virtual void Awake()
		{
			animationEvents = GetComponentInChildren<AnimationEvents>();
			animationEvents.OnAttackHit += AttackHit;
			stats = GetComponent<UnitStats>();
			controller = GetComponent<EnemyController>();
			controller.OnAttackButtonPress += Controller_AttackButtonPress;
			controller.OnAttackButtonRelease += Controller_StopAttack;
			isOn = true;
		}

		private void AttackHit(int obj)
		{
			if (!isOn) return;
			AttackTarget(currentAttackTarget);
		}

		public void Controller_AttackButtonPress(Vector3 target)
		{
			if (!isOn) return;
			if (Time.time >= currentCooldownTime)
			{
				currentCooldownTime = Time.time + stats.attackRate;
				currentAttackTarget = target;
				OnAttackStart?.Invoke();
			}
		}

		public void Controller_StopAttack()
		{
			if(!isOn) return;
			OnAttackStop?.Invoke();
		}

		private void AttackTarget(Vector3 newTargetPosition)
		{
			if (!isOn) return;
			var hitObject = GetAttackHitObject(newTargetPosition);
			if (hitObject.collider == null) return;

			newTargetPosition = hitObject.point;
			var target = hitObject.collider.gameObject.GetComponent<DefenceHandler>();
			if (target != null)
			{
				var newAttack = new Attack(transform.position, newTargetPosition,
					stats.attackDamage);
				target.TakeDamage(newAttack);
			}
		}

		private RaycastHit2D GetAttackHitObject(Vector3 targetPosition)
		{
			RaycastHit2D raycastHit = Physics2D.Raycast(footPoint.transform.position,
				(targetPosition - footPoint.transform.position).normalized,
				Vector3.Distance(footPoint.transform.position, targetPosition), ASSETS.layers.PlayerLayer);
			return raycastHit;
		}




		public bool CanAttack(Vector3 target)
		{
			if (!isOn) return false;
			float targetDistance = Vector3.Distance(GetAimCenter(), target);
			if (targetDistance < stats.attackRange)
			{
				return true;
			}

			return false;
		}

		public OnAttackEventArgs GetAttackEvent(DefenceHandler target)
		{
			return new OnAttackEventArgs(attackPoint.transform.position,
				target.GetPosition() + new Vector3(0, target.GetAimHeight(), 0), target);
		}



		private Vector3 GetAimCenter()
		{
			return aimCenter.transform.position;
		}

		public void Disable()
		{
			isOn = false;
		}

		public event Action OnKillEnemy;
		public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	}
}
