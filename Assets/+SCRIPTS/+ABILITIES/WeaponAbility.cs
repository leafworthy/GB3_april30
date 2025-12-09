using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class WeaponAbility : Ability
	{
		public enum weaponState
		{
			not,
			pullOut,
			idle,
			attacking,
			resuming
		}


		protected void SetState(weaponState state)
		{
			currentState = state;
		}
		protected bool  isActive = true;
		public weaponState currentState {get; private set;}
		public AnimationClip pullOutAnimationClip;
		public override string AbilityName => "Weapon-Ability";

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle;

		protected override void DoAbility()
		{
			if(currentState != weaponState.resuming)PullOut();
			else
			{
				StartIdle();
			}
		}

		protected virtual void PullOut()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(pullOutAnimationClip, 1);

		}

		public override void Resume()
		{
			SetState(weaponState.resuming);
			Try();
		}

		protected override void AnimationComplete()
		{
			StartIdle();
		}

		public override void Stop()
		{
			currentState = weaponState.not;
			isActive = false;
			base.Stop();
		}

		protected virtual void StartIdle()
		{
			SetState(weaponState.idle);
			isActive = true;
		}
	}
}
