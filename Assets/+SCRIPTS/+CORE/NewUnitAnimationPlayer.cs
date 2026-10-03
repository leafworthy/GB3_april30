using System;
using System.Linq;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class KrazyKarrotAnimations : MonoBehaviour
{
	public NewUnitAnimationPlayer animation => _animation ??= GetComponent<NewUnitAnimationPlayer>();
	NewUnitAnimationPlayer _animation;
	public NewMoveAbility moveAbility => _moveAbility ??= GetComponent<NewMoveAbility>();
	NewMoveAbility _moveAbility;

	public Body body => _body ??= GetComponent<Body>();
	Body _body;

	public JumpAbility jumps => _jumps ??= GetComponent<JumpAbility>();
	JumpAbility _jumps;

	public AnimationClip StandingAnimationClip;
	void Start()
	{
		if (animation == null) return;
		animation.OnAnimationComplete += Animation_OnAnimationComplete;
		moveAbility.OnMove += MoveAbility_OnMove;
		 moveAbility.OnStopMoving += MoveAbility_OnStopMoving;
	}

	void MoveAbility_OnStopMoving()
	{
		if (jumps.IsResting)
		{
			animation.PlayAnimationClip(moveAbility.standAnimationClip);
		}
	}

	void MoveAbility_OnMove()
	{
		if (jumps.IsResting)
		{
			animation.PlayAnimationClip(moveAbility.moveAnimationClip);
		}
	}

	void Animation_OnAnimationComplete()
	{

	}
}

namespace __SCRIPTS
{
	public class NewUnitAnimationPlayer : MonoBehaviour
	{
		Animator animator => _animator ??= GetComponentInChildren<Animator>();
		Animator _animator;
		public event Action OnAnimationComplete;

		protected virtual void AnimationComplete()
		{
			OnAnimationComplete?.Invoke();
		}
		void Play(string animationClipName, int layer, float startingPlace)
		{
			if (animator == null) return;
			if (AnimationDoesntExist(animationClipName)) return;
			animator.Play(animationClipName, layer, startingPlace);
		}

		public void PlayAnimationClip(string clipName, float length, int layer = 0)
		{
			Play(clipName, layer, 0);
			if (length != 0) Invoke(nameof(AnimationComplete), length);
		}

		public void PlayAnimationClipWithoutEvent(string clipName, int layer = 0)
		{
			Play(clipName, layer, 0);
		}

		public void PlayAnimationClip(AnimationClip clip, int layer = 0)
		{
			Debug.Log("playing clip " + clip.name + " with layer " + layer);
			PlayAnimationClip(clip.name, clip.length, layer);
		}
		bool AnimationDoesntExist(string animationClipName)
		{
			var animationExists = animator.runtimeAnimatorController.animationClips.Any(t => t.name == animationClipName);
			if (!animationExists) return true;
			return false;
		}

	}
}
