using UnityEngine;

public class Bean_SFXHandler : MonoBehaviour
{
	public AnimationEvents animEventsBottom;
	public AnimationEvents animEventsTop;
	private BeanAttackHandler attackHandler;
	private JumpHandler jumpHandler;

	private void Awake()
	{
		attackHandler = GetComponentInChildren<BeanAttackHandler>();
		jumpHandler = GetComponent<JumpHandler>();
		jumpHandler.OnJump += Jump;
		jumpHandler.OnLand += Land;

		attackHandler.OnShootStart += AttackOnShootStart;
		attackHandler.OnShootMiss += AttackOnShootMiss;
		animEventsBottom.OnStep += Anim_OnStep;
		animEventsBottom.OnDash += Anim_OnRoll;
		animEventsTop.OnAttackHit += Anim_OnKnifeHit;
		animEventsTop.OnThrow += Anim_OnNadeThrow;
		animEventsTop.OnReload += Anim_OnReload;

	}

	private void AttackOnShootMiss()
	{
		ASSETS.sounds.bean_gun_miss_sounds.PlayRandom();

	}

	private void Land()
	{
		ASSETS.sounds.land_sound.PlayRandom();
	}

	private void Jump()
	{
		ASSETS.sounds.jump_sound.PlayRandom();
	}

	private void Anim_OnReload()
	{
		ASSETS.sounds.bean_reload_sounds.PlayRandom();
	}

	private void Anim_OnNadeThrow()
	{
		ASSETS.sounds.bean_nade_throw_sounds.PlayRandom();
	}

	private void Anim_OnRoll()
	{
		ASSETS.sounds.bean_roll_sounds.PlayRandom();
	}


	private void Anim_OnKnifeHit(int i)
	{
		ASSETS.sounds.bean_knifehit_sounds.PlayRandom();
	}

	private void AttackOnShootStart(Attack attack)
	{

		ASSETS.sounds.ak47_shoot_sounds.PlayRandom();
	}

	private void Anim_OnStep()
	{
		ASSETS.sounds.player_walk_sounds_concrete.PlayRandom();
	}
}
