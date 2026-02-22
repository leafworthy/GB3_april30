using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;

public class StartingAnimation : MonoBehaviour
{
	public List<AudioClip> startSound = new();
	public AnimationClip startAnimation;
	MoveAbility moveAbility => GetComponent<MoveAbility>();

	UnitAnimations anim => GetComponent<UnitAnimations>();

	void Start()
	{
		anim.Play(startAnimation.name, 0, 0);
		startSound?.PlayRandomAt(transform.position);
		moveAbility.SetCanMove(false);
		Invoke(nameof(AnimationComplete), startAnimation.length);
	}

	void AnimationComplete()
	{
		moveAbility.SetCanMove(true);
	}
}
