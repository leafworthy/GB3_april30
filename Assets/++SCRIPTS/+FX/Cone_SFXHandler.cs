using UnityEngine;

public class Cone_SFXHandler : MonoBehaviour
{
	private Animations anim;
	private AnimationEvents animEvents;
	private RecycleGameObject recycle;

	private void Awake()
	{
		
		Activate();
	}

	private void Activate()
	{
		anim = GetComponent<Animations>();
		animEvents = anim.animEvents;
		animEvents.OnAttackHit += Anim_OnAttackHit;
		animEvents.OnStep += Anim_OnStep;
		animEvents.OnRoar += Anim_OnRoar;
		animEvents.OnHitStart += Anim_OnHit;
		animEvents.OnDieStart += Anim_OnDie;
	}

	private void CleanUp()
	{
		animEvents.OnAttackHit -= Anim_OnAttackHit;
		animEvents.OnStep -= Anim_OnStep;
		animEvents.OnRoar -= Anim_OnRoar;
		animEvents.OnHitStart -= Anim_OnHit;
		animEvents.OnDieStart -= Anim_OnDie;
	}




	private void Anim_OnDie()
	{
		ASSETS.sounds.cone_die_sounds.PlayRandom();
	}

	private void Anim_OnHit()
	{

		ASSETS.sounds.cone_gethit_sounds.PlayRandom();
	}

	private void Anim_OnRoar()
	{
		ASSETS.sounds.cone_roar_sounds.PlayRandom();
	}

	private void Anim_OnAttackHit(int attackType)
	{
		ASSETS.sounds.cone_attack_sounds.PlayRandom();
	}

	private void Anim_OnStep()
	{
		ASSETS.sounds.cone_walk_sounds.PlayRandom();
	}
}
