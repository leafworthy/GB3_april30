using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __SCRIPTS
{
	public class UnitAnimations : MonoBehaviour
	{
		public AnimationEvents animEvents => _animEvents ??= GetComponentInChildren<AnimationEvents>();
		AnimationEvents _animEvents;
		public Animator animator => _animator ??= GetComponentInChildren<Animator>();
		Animator _animator;

		public AnimationClip DefaultBottomAnimation;
		HashSet<int> parameterHashes;


		#region animation hashes

		public static readonly int SelectedWeapon = Animator.StringToHash("SelectedWeapon");
		public static readonly int CurrentWeapon = Animator.StringToHash("CurrentWeapon");

		public static readonly int ShootSpeed = Animator.StringToHash("ShootSpeed");
		public static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
		public static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");
		public static readonly int Attack1Trigger = Animator.StringToHash("Attack1Trigger");
		public static readonly int ReloadTrigger = Animator.StringToHash("ReloadTrigger");
		public static readonly int ChargeAttackTrigger = Animator.StringToHash("ChargeAttackTrigger");
		public static readonly int FlyingTrigger = Animator.StringToHash("FlyingTrigger");

		public static readonly int IsUsingPrimary = Animator.StringToHash("IsUsingPrimary");
		public static readonly int IsDashing = Animator.StringToHash("IsDashing");
		public static readonly int IsFallingFromSky = Animator.StringToHash("FallFromSky");
		public static readonly int IsBobbing = Animator.StringToHash("IsBobbing");
		public static readonly int IsDead = Animator.StringToHash("IsDead");
		public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
		public static readonly int IsCharging = Animator.StringToHash("IsCharging");
		public static readonly int IsMoving = Animator.StringToHash("IsMoving");
		public static readonly int IsChainsawing = Animator.StringToHash("IsChainsawing");
		public static readonly int IsShielding = Animator.StringToHash("IsShielding");

		#endregion

		void Awake()
		{
			CacheParameterHashes();
		}

		void CacheParameterHashes()
		{
			parameterHashes = new HashSet<int>();
			foreach (var param in animator.parameters)
			{
				parameterHashes.Add(param.nameHash);
			}
		}

		public void Play(string animationClipName, int layer, float startingPlace)
		{
			if (AnimationDoesntExist(animationClipName))
			{
				Debug.Log(name + " tried to play animation clip " + animationClipName + " but it doesn't exist", this);
				return;
			}

			animator.Play(animationClipName, layer, startingPlace);
		}

		bool AnimationDoesntExist(string animationClipName)
		{
			//CHANGED THIS
			return animator.runtimeAnimatorController.animationClips.All(t => t.name != animationClipName);
		}

		public void SetFloat(int trigger, float amount)
		{
			if (HasParameter(trigger)) animator.SetFloat(trigger, amount);
		}

		public void ResetTrigger(int trigger)
		{
			if (HasParameter(trigger)) animator.ResetTrigger(trigger);
		}

		public void SetTrigger(int trigger)
		{
			if (HasParameter(trigger)) animator.SetTrigger(trigger);
		}

		public void SetBool(int parameterHash, bool value)
		{
			if (!HasParameter(parameterHash)) return;
			if (animator.GetBool(parameterHash) != value)
				animator.SetBool(parameterHash, value);
		}

		bool HasParameter(int parameterHash) => animator != null && parameterHashes.Contains(parameterHash);

		public void RevertBottomToDefault()
		{
			if (animator == null) return;
			animator.StopPlayback();
			if (DefaultBottomAnimation == null) return;
			animator.Play(DefaultBottomAnimation.name, 0);
		}

		public void SetInt (int parameterHash, int value)
		{
			if (!HasParameter(parameterHash)) return;
			if (animator.GetInteger(parameterHash) != value)
				animator.SetInteger(parameterHash, value);
		}
	}
}
