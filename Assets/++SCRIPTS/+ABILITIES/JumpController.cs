using UnityEngine;
using UnityEngine.PlayerLoop;

public class JumpController : MonoBehaviour
{
	private Life life;
	private JumpAbility jump;
	private Animations anim;
	private AnimationEvents animEvents;
	private Body body;
	private string VerbName = "Jumping";
	private float FallInDistance = 80;

	public void Init(bool fallFromSky)
	{
		life = GetComponent<Life>();
		life.player.Controller.Jump.OnPress += Controller_Jump;

		anim = GetComponent<Animations>();
		animEvents = anim.animEvents;
		animEvents.OnRoar += AnimEvents_OnRoar;
		animEvents.OnLandingStop += AnimEvents_OnLandingStop;
		body = GetComponent<Body>();
		jump = GetComponent<JumpAbility>();
		jump.OnLand += Land;
		jump.OnResting += Jump_OnResting;
		jump.OnFall += Jump_OnFall;
		jump.FallFromHeight(fallFromSky ? FallInDistance : 0);
		life.OnDamaged += Life_Damaged;



	}

	private void AnimEvents_OnLandingStop()
	{
		jump.StartResting();
	}

	private void AnimEvents_OnRoar()
	{
		jump.SetActive(true);
	}

	private void OnDisable()
	{ 
		if(life == null) return;
		if(life.player == null) return;
		life.player.Controller.Jump.OnPress -= Controller_Jump;
	 
		if(jump == null) return;
		 jump.OnLand -= Land;
		 jump.OnResting -= Jump_OnResting;
		 jump.OnFall -= Jump_OnFall;
		 life.OnDamaged -= Life_Damaged;
		 animEvents.OnRoar -= AnimEvents_OnRoar;
		 animEvents.OnLandingStop -= AnimEvents_OnLandingStop;
		  
			
	}

	private void Controller_Jump(NewControlButton newControlButton)
	{
		if (PauseManager.IsPaused) return;

		Debug.Log("controller jump start");
		Jump();
	}
	private void Life_Damaged(Attack attack)
	{
		anim.ResetTrigger(Animations.LandTrigger);
		anim.SetTrigger(Animations.FlyingTrigger);
		jump.Jump(body.GetCurrentLandableHeight(), life.JumpSpeed, 99);
		Debug.Log("regular jump start");
		
		Debug.Log("damaged");
		
	}

	private void Jump_OnFall(Vector2 obj)
	{
		anim.SetBool(Animations.IsFalling, true);
	}

	private void Jump_OnResting(Vector2 obj)
	{
		body.arms.Stop("Land");
		body.legs.Stop("Land");
		anim.SetBool(Animations.IsFalling, false);
	}

	private void Jump(float modifier = 1)
	{
		if (PauseManager.IsPaused) return;


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