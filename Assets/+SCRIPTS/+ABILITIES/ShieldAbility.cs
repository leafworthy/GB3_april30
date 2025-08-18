using __SCRIPTS;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

public class ShieldAbilityController: AbilityController
{
	private ShieldAbility shieldAbility;
	protected override void StopListeningToPlayer()
	{
	}

	private void ControllerDashPress(NewControlButton newControlButton)
	{
		shieldAbility.SetShielding(true);

	}
	protected override void ListenToPlayer()
	{
		shieldAbility = player.SpawnedPlayerGO.GetComponent<ShieldAbility>();
		player.Controller.OnDash_Pressed += ControllerDashPress;
	}
}
public class ShieldAbility : ServiceUser, INeedPlayer, IActivity
{

	private AnimationEvents animEvents;
	private MoveController moveController;
	private Life life;
	private Player owner;
	private JumpAbility jumps;
	private JumpController jumpController;
	private Body body;
	private UnitAnimations anim;
	private bool isDashing;
	private bool isShielding;
	public AnimationClip shieldOutClip;
	public string VerbName => "Shield";



	public void SetPlayer(Player _player)
	{
		anim = GetComponent<UnitAnimations>();
		body = GetComponent<Body>();
		jumps = GetComponent<JumpAbility>();
		jumpController = GetComponent<JumpController>();
		moveController = GetComponent<MoveController>();

		life = GetComponent<Life>();
		owner = _player;

	}



	private void ShieldPush()
	{
		var hits = Physics2D.OverlapCircleAll(transform.position, 30, AssetManager.LevelAssets.EnemyLayer);
		foreach (var hit in hits)
		{
			var _life = hit.GetComponent<Life>();
			if (_life != null && _life.IsEnemyOf(life))
			{

				var movement = hit.GetComponent<MoveAbility>();
				if (movement == null) return;
				movement.Push((hit.transform.position - transform.position).normalized, life.DashSpeed*1.5f);
			}
		}
	}





	public void SetShielding(bool isOn)
	{
		isShielding = isOn;
		anim.SetBool(UnitAnimations.IsShielding, isOn);
		life.SetShielding(isOn);
	}



}
