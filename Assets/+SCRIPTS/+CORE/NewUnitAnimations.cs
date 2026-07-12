using System;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class NewUnitAnimations : MonoBehaviour
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

		public void SetMoving(bool b)
		{
		}
	}
}
