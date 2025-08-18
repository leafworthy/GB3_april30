using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[DisallowMultipleComponent, RequireComponent(typeof(DoableJumpAbility))]
	public class DoableJumpController : AbilityController
	{
		private DoableJumpAbility _ability;
		private DoableJumpAbility ability => _ability ??= GetComponent<DoableJumpAbility>();

		private LandingActivity landingActivity = new();

		protected override void ListenToPlayer()
		{
			life.OnWounded += Life_Wounded;
			life.player.Controller.Jump.OnPress += Controller_Jump;
			life.OnDying += Life_OnDying;
			body.OnFallFromLandable +=  Body_OnFallFromLandable;
			player.Controller.Jump.OnPress += Controller_Jump;
			animEvents.OnRoar += AnimEvents_OnRoar;
			ability.OnLand += Land;

			ability.FallFromLandable();
		}

		protected override void StopListeningToPlayer()
		{
			life.OnWounded -= Life_Wounded;
			life.player.Controller.Jump.OnPress -= Controller_Jump;
			life.OnDying -= Life_OnDying;
			body.OnFallFromLandable -= Body_OnFallFromLandable;
			player.Controller.Jump.OnPress -= Controller_Jump;
			animEvents.OnRoar -= AnimEvents_OnRoar;
			ability.OnLand -= Land;
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



		private void Land(Vector2 pos)
		{
			anim.ResetTrigger(UnitAnimations.JumpTrigger);
			anim.SetTrigger(UnitAnimations.LandTrigger);
			anim.SetBool(UnitAnimations.IsFalling, false);
			body.legs.StopCurrentActivity();
			body.arms.StopCurrentActivity();
			body.arms.Do(landingActivity);
			body.legs.Do(landingActivity);
		}
	}
}
