using __SCRIPTS.HUD_Displays;
using UnityEngine;

namespace __SCRIPTS
{
	public class JumpController : MonoBehaviour, INeedPlayer
	{
		private Life life;
		private JumpAbility jump;
		private Animations anim;
		private AnimationEvents animEvents;
		private Body body;
		private string VerbName = "Jumping";
		private float FallInDistance = 80;

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
			jump = GetComponent<JumpAbility>();
			jump.FallFromHeight(fallFromSky ? FallInDistance : 0);
			jump.OnFall += Jump_OnFall;
			jump.OnLand += Land;
			jump.OnResting += Jump_OnResting;
			}

		private void OnDisable()
		{
			if (life == null) return;
			if (life.player == null) return;
			life.player.Controller.Jump.OnPress -= Controller_Jump;
			if (jump == null) return;
			jump.OnLand -= Land;
			jump.OnResting -= Jump_OnResting;
			jump.OnFall -= Jump_OnFall;
			life.OnWounded -= Life_Wounded;
			if (animEvents == null) return;
			animEvents.OnRoar -= AnimEvents_OnRoar;
			animEvents.OnLandingStop -= AnimEvents_OnLandingStop;


		}
		private void AnimEvents_OnLandingStop()
		{
			jump.StartResting();
		}

		private void AnimEvents_OnRoar()
		{
			jump.SetActive(true);
		}

	

		private void Controller_Jump(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;

			Jump();
		}
		private void Life_Wounded(Attack attack)
		{
			anim.ResetTrigger(Animations.LandTrigger);
			anim.SetTrigger(Animations.FlyingTrigger);
			jump.Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
		
		}

		private void Jump_OnFall(Vector2 obj)
		{
			anim.SetBool(Animations.IsFalling, true);
		}

		private void Jump_OnResting(Vector2 obj)
		{
			body.arms.StopSafely("Land");
			body.legs.StopSafely("Land");
			anim.SetBool(Animations.IsFalling, false);
		}

		private void Jump()
		{
			if (PauseManager.I.IsPaused) return;


			if (!body.arms.Do(VerbName)) return;
			if (!jump.isResting) return;



			anim.ResetTrigger(Animations.LandTrigger);
			anim.SetTrigger(Animations.JumpTrigger);
			jump.Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
			Debug.Log("regular jump start");
		}


		private void Land(Vector2 pos)
		{
			anim.ResetTrigger(Animations.JumpTrigger);
			anim.SetTrigger(Animations.LandTrigger);
			anim.SetBool(Animations.IsFalling, false);
		
			body.arms.Stop(VerbName);
			body.arms.Do("Land");
			body.legs.Do("Land");
		
		}


		
	}
}