using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{

	public class JumpController : AbilityController
	{
		private JumpAbility _ability;
		private JumpAbility ability => _ability ??= GetComponent<JumpAbility>();


		protected override void ListenToPlayer()
		{
			life.OnWounded += Life_Wounded;
			life.player.Controller.OnJump_Pressed += Controller_Jump;
			life.OnDying += Life_OnDying;
			body.OnFallFromLandable += Body_OnFallFromLandable;
			player.Controller.OnJump_Pressed += Controller_Jump;
			animEvents.OnRoar += AnimEvents_OnRoar;

			ability.FallFromLandable();
		}

		protected override void StopListeningToPlayer()
		{
			life.OnWounded -= Life_Wounded;
			life.player.Controller.OnJump_Pressed -= Controller_Jump;
			life.OnDying -= Life_OnDying;
			body.OnFallFromLandable -= Body_OnFallFromLandable;
			player.Controller.OnJump_Pressed -= Controller_Jump;
			animEvents.OnRoar -= AnimEvents_OnRoar;
		}

		private void Body_OnFallFromLandable(float obj)
		{

			ability.FallFromLandable();
		}

		private void Life_OnDying(Player arg1, Life arg2)
		{
			ability.DoSafely();
		}

		private void AnimEvents_OnRoar()
		{
			ability.SetCanJump(true);
		}

		private void Controller_Jump(NewControlButton newControlButton)
		{
			ability.DoSafely();
		}

		private void Life_Wounded(Attack attack)
		{
			anim.ResetTrigger(UnitAnimations.LandTrigger);
			anim.SetTrigger(UnitAnimations.FlyingTrigger);
			ability.DoSafely();
		}

		public void Jump(float attackOriginHeight)
		{
			ability.DoSafely();
			ability.SetDistanceToGround(attackOriginHeight);
		}
	}
}
