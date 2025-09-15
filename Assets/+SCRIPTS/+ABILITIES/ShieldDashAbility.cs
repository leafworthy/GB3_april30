using __SCRIPTS;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldDashAbility : DashAbility
{
	private float extraDashSpeedFactor = 1.5f;
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
			var _life = hit.GetComponent<Life>();
			if (_life == null || !_life.IsEnemyOf(life)) continue;
			var movement = hit.GetComponent<MoveAbility>();
			if (movement == null) return;
			movement.Push((hit.transform.position - transform.position).normalized, life.DashSpeed * extraDashSpeedFactor);
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
