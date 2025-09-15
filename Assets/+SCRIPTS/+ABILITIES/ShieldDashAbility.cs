using __SCRIPTS;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldDashAbility : DashAbility
{
	private float extraDashPushFactor = 3;
	public override string AbilityName => "Shield-Dash";
	protected override void DoAbility()
	{
		base.DoAbility();
		ShieldDash();
	}

	private void ShieldDash()
	{
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, life.EnemyLayer);
		foreach (var hit in hits)
		{
			Debug.Log("[SHIELD] Hit " + hit.name);
			var _life = hit.GetComponentInParent<Life>();
			if (_life == null || !_life.IsEnemyOf(life))
			{
				Debug.Log("[SHIELD] Not an enemy");
				continue;
			}
			var movement = _life.GetComponent<MoveAbility>();
			if (movement == null)
			{
				Debug.Log("[SHIELD] No movement component");
				continue;
			}
			movement.Push((hit.transform.position - transform.position).normalized, life.DashSpeed * extraDashPushFactor);
			Debug.Log("[SHIELD] Pushed " + hit.name);
		}
	}

	public override void SetPlayer(Player _player)
	{
		base.SetPlayer(_player);
		_player.Controller.DashRightShoulder.OnPress += ControllerDashRightShoulderPress;
	}

	private void OnDestroy()
	{
		if (player == null) return;
		if (player.Controller == null) return;
		player.Controller.DashRightShoulder.OnPress -= ControllerDashRightShoulderPress;
	}

	private void ControllerDashRightShoulderPress(NewControlButton newControlButton)
	{
		Do();
	}


}
