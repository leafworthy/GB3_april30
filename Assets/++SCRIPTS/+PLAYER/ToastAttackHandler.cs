using System;
using System.Collections.Concurrent;
using UnityEngine;

[Serializable]
public class ToastAttackHandler : MonoBehaviour, IAttackHandler
{
	public event Action OnAttackStop;
	public event Action OnAttackStart;

	[SerializeField] private GameObject aimCenter;
	[SerializeField] private GameObject gunEndPoint;
	[SerializeField] private GameObject footPoint;

	private UnitStats stats;

	private float currentCooldownTime;
	private EnemyController controller;
	private bool isOn;
	private AnimationEvents animationEvents;
	private Vector3 currentAttackTarget;
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
			currentCooldownTime = Time.time + stats.GetStatValue(StatType.attackRate);
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
				stats.GetStatValue(StatType.attackDamage));
			target.TakeDamage(newAttack);
		}
	}

	private RaycastHit2D GetAttackHitObject(Vector3 targetPosition)
	{
		RaycastHit2D raycastHit = Physics2D.Raycast(footPoint.transform.position,
			(targetPosition - footPoint.transform.position).normalized,
			Vector3.Distance(footPoint.transform.position, targetPosition), ASSETS.LevelAssets.PlayerLayer);
		return raycastHit;
	}


	public Player GetPlayer()
	{
		return PLAYERS.GetEnemyPlayer();
	}

	public bool CanAttack(Vector3 target)
	{
		if (!isOn) return false;
		float targetDistance = Vector3.Distance(GetAimCenter(), target);
		if (targetDistance < stats.GetStatValue(StatType.attackRange))
		{
			return true;
		}

		return false;
	}

	public OnAttackEventArgs GetAttackEvent(DefenceHandler target)
	{
		return new OnAttackEventArgs(gunEndPoint.transform.position,
			target.GetPosition() + new Vector3(0, target.GetAimHeight(), 0), target);
	}


	public Vector3 GetAimCenter()
	{
		return aimCenter.transform.position;
	}

	public void Disable()
	{
		isOn = false;
	}

	public event Action OnKillEnemy;
	public event Action<AmmoHandler.AmmoType, int> OnUseAmmo;
	public event Action<Vector3> OnAim;
}
