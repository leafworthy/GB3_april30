using __SCRIPTS;
using UnityEngine;

public class StartingAnimation : MonoBehaviour
{
	public AnimationClip startAnimation;
	private MoveAbility moveAbility => GetComponent<MoveAbility>();

	private UnitAnimations anim =>   GetComponent<UnitAnimations>();

	private void Start()
	{
		anim.Play(startAnimation.name,0,0);
		moveAbility.SetCanMove(false);
		Invoke(nameof(AnimationComplete), startAnimation.length);
	}

	private void AnimationComplete()
	{
		moveAbility.SetCanMove(true);
	}
}