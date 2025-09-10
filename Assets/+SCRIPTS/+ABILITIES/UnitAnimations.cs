using System.Collections.Generic;
using UnityEngine;
using GangstaBean.Core;

namespace __SCRIPTS
{
	public class UnitAnimations : MonoBehaviour, IPoolable
	{
		public AnimationEvents animEvents => _animEvents ??= GetComponentInChildren<AnimationEvents>();
		private AnimationEvents _animEvents;
		[SerializeField] public Animator animator;

		#region animation hashes

		public static readonly int ShootSpeed = Animator.StringToHash("ShootSpeed");

		public static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
		public static readonly int LandTrigger = Animator.StringToHash("LandTrigger");

		public static readonly int AggroTrigger = Animator.StringToHash("AggroTrigger");
		public static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
		public static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");
		public static readonly int Attack1Trigger = Animator.StringToHash("Attack1Trigger");
		public static readonly int Attack2Trigger = Animator.StringToHash("Attack2Trigger");
		public static readonly int Attack3Trigger = Animator.StringToHash("Attack3Trigger");

		public static readonly int ChargeStartTrigger = Animator.StringToHash("ChargeStartTrigger");
		public static readonly int ChargeAttackTrigger = Animator.StringToHash("ChargeAttackTrigger");
		public static readonly int JumpAttackTrigger = Animator.StringToHash("JumpAttackTrigger");
		public static readonly int FlyingTrigger = Animator.StringToHash("FlyingTrigger");
		public static readonly int KnifeTrigger = Animator.StringToHash("KnifeTrigger");
		public static readonly int ThrowTrigger = Animator.StringToHash("ThrowTrigger");
		public static readonly int DashTrigger = Animator.StringToHash("DashTrigger");
		public static readonly int ShootingTrigger = Animator.StringToHash("ShootingTrigger");
		public static readonly int ShieldTrigger = Animator.StringToHash("ShieldTrigger");

		public static readonly int IsFallingFromSky = Animator.StringToHash("FallFromSky");
		public static readonly int IsBobbing = Animator.StringToHash("IsBobbing");
		public static readonly int IsFalling = Animator.StringToHash("IsFalling");
		public static readonly int IsGlocking = Animator.StringToHash("IsGlocking");
		public static readonly int IsDead = Animator.StringToHash("IsDead");
		public static readonly int IsShooting = Animator.StringToHash("IsShooting");
		public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
		public static readonly int IsCharging = Animator.StringToHash("IsCharging");
		public static readonly int IsMoving = Animator.StringToHash("IsMoving");
		public static readonly int IsChainsawing = Animator.StringToHash("IsChainsawing");

		public static readonly int GlockTrigger = Animator.StringToHash("GlockTrigger");

		private HashSet<int> parameterHashes;
		private static int _aimDir;
		public static readonly int IsShielding = Animator.StringToHash("IsShielding");
		public static readonly int AimDir = Animator.StringToHash("AimDir");

		#endregion

		private void Awake()
		{
			InitializeAnimations();
		}

		private void InitializeAnimations()
		{
			animator = GetComponentInChildren<Animator>();
			CacheParameterHashes();
			ResetAnimatorState();
		}

		private void CacheParameterHashes()
		{
			parameterHashes = new HashSet<int>();
			foreach (var param in animator.parameters)
			{
				parameterHashes.Add(param.nameHash);
			}
		}

		public void Play(string animationClipName, int layer, float startingPlace)
		{
			if (animator == null) animator = GetComponentInChildren<Animator>();
			if (animator == null) return;


			// Check if the animation exists
			var animationExists = false;
			foreach (var t in animator.runtimeAnimatorController.animationClips)
			{
				if (t.name == animationClipName)
				{
					animationExists = true;
					break;
				}
			}

			if (!animationExists)
			{
				foreach (var clip in animator.runtimeAnimatorController.animationClips)
				{
				}

				return;
			}

			animator.Play(animationClipName, layer, startingPlace);
		}

		public void SetFloat(int trigger, float amount)
		{
			if (animator == null) animator = GetComponentInChildren<Animator>();
			if (HasParameter(trigger)) animator.SetFloat(trigger, amount);
		}

		public void ResetTrigger(int trigger)
		{
			if (animator == null) animator = GetComponentInChildren<Animator>();
			if (HasParameter(trigger)) animator.ResetTrigger(trigger);
		}

		public void SetTrigger(int trigger)
		{
			if (animator == null) animator = GetComponentInChildren<Animator>();
			if (HasParameter(trigger)) animator.SetTrigger(trigger);
		}

		public void SetBool(int parameterHash, bool value)
		{
			if (HasParameter(parameterHash))
			{
				if (animator.GetBool(parameterHash) != value)
				{
					animator.SetBool(parameterHash, value);
				}
			}
		}

		private bool HasParameter(int parameterHash) => animator != null && parameterHashes.Contains(parameterHash);

		public void SetInt(int parameterHash, int getAimDirNumberFromDegrees)
		{
			if (animator == null) animator = GetComponentInChildren<Animator>();
			if (animator == null) return;

			if (HasParameter(parameterHash)) animator.SetInteger(parameterHash, getAimDirNumberFromDegrees);
		}

		private void ResetAnimatorState()
		{
			if (animator == null) return;

			// Reset all boolean parameters to false
			SetBool(IsDead, false);
			SetBool(IsAttacking, false);
			SetBool(IsCharging, false);
			SetBool(IsMoving, false);
			SetBool(IsChainsawing, false);
			SetBool(IsShooting, false);
			SetBool(IsGlocking, false);
			SetBool(IsFalling, false);
			SetBool(IsBobbing, false);
			SetBool(IsFallingFromSky, false);
			SetBool(IsShielding, false);

			// Reset triggers
			ResetTrigger(AggroTrigger);
			ResetTrigger(HitTrigger);
			ResetTrigger(DeathTrigger);
			ResetTrigger(Attack1Trigger);
			ResetTrigger(Attack2Trigger);
			ResetTrigger(Attack3Trigger);
		}

		public void OnPoolSpawn()
		{
			// Reinitialize animations when spawned from pool

			InitializeAnimations();
		}

		public void OnPoolDespawn()
		{
			// Nothing needed when despawning
		}

	}
}
