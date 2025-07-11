using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class LandingActivity : IActivity
	{
		public string VerbName => "Landing";

		public bool TryCompleteGracefully(CompletionReason reason, IActivity newActivity = null) => false;
	}

	public class JumpController : ServiceUser, INeedPlayer
	{
		private Life life;
		private JumpAbility jumpAbility;
		private Animations anim;
		private AnimationEvents animEvents;
		private Body body;
		private float FallInDistance = 80;
		private LandingActivity landingActivity = new();
		public string VerbName => "Jumping";

		public void SetPlayer(Player _player)
		{
			life = GetComponent<Life>();
			life.OnWounded += Life_Wounded;
			life.player.Controller.Jump.OnPress += Controller_Jump;

			anim = GetComponent<Animations>();
			animEvents = anim.animEvents;
			animEvents.OnRoar += AnimEvents_OnRoar;
			animEvents.OnLandingStop += AnimEvents_OnLandingStop;
			body = GetComponent<Body>();
		}

		public void SetFallFromSky(bool fallFromSky)
		{
			jumpAbility = GetComponent<JumpAbility>();
			jumpAbility.FallFromHeight(fallFromSky ? FallInDistance : 0);
			jumpAbility.OnFall += Jump_OnFall;
			jumpAbility.OnLand += Land;
			jumpAbility.OnResting += Jump_OnResting;
		}

		private void OnDisable()
		{
			if (life == null) return;
			if (life.player == null) return;
			life.player.Controller.Jump.OnPress -= Controller_Jump;
			if (jumpAbility == null) return;
			jumpAbility.OnLand -= Land;
			jumpAbility.OnResting -= Jump_OnResting;
			jumpAbility.OnFall -= Jump_OnFall;
			life.OnWounded -= Life_Wounded;
			if (animEvents == null) return;
			animEvents.OnRoar -= AnimEvents_OnRoar;
			animEvents.OnLandingStop -= AnimEvents_OnLandingStop;
		}

		private void AnimEvents_OnLandingStop()
		{
			jumpAbility.StartResting();
		}

		private void AnimEvents_OnRoar()
		{
			jumpAbility.SetActive(true);
		}

		private void Controller_Jump(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;

			Jump();
		}

		private void Life_Wounded(Attack attack)
		{
			anim.ResetTrigger(Animations.LandTrigger);
			anim.SetTrigger(Animations.FlyingTrigger);
			jumpAbility.Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
		}

		private void Jump_OnFall(Vector2 obj)
		{
			anim.SetBool(Animations.IsFalling, true);
		}

		private void Jump_OnResting(Vector2 obj)
		{
			body.arms.Stop(landingActivity);
			body.legs.Stop(landingActivity);
			anim.SetBool(Animations.IsFalling, false);
		}

		public void Jump()
		{
			if (pauseManager.IsPaused) return;

			if (!body.arms.Do(jumpAbility)) return;
			if (!jumpAbility.isResting) return;

			anim.ResetTrigger(Animations.LandTrigger);
			anim.SetTrigger(Animations.JumpTrigger);
			jumpAbility.Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
		}

		private void Land(Vector2 pos)
		{
			anim.ResetTrigger(Animations.JumpTrigger);
			anim.SetTrigger(Animations.LandTrigger);
			anim.SetBool(Animations.IsFalling, false);
			body.legs.StopCurrentActivity();
			body.arms.StopCurrentActivity();
			body.arms.Do(landingActivity);
			body.legs.Do(landingActivity);
		}
	}
}
