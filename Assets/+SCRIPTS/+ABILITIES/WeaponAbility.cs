using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class WeaponAbility : Ability
	{
		public AnimationClip pullOutAnimationClip;

		protected enum weaponState
		{
			not,
			pullOut,
			idle,
			attacking,
			resuming,
			reloading
		}
		protected void SetState(weaponState state)
		{
			currentState = state;
		}
		protected bool isActive => currentState is weaponState.idle or weaponState.attacking;
		protected bool isIdle => currentState is weaponState.idle;

		protected weaponState currentState {get; private set;}
		public override string AbilityName => "Weapon-Ability";

		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;

		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle;

		protected override void DoAbility()
		{
			if(currentState != weaponState.resuming && currentState != weaponState.pullOut)
			{
				Debug.Log("doing pull out");
				PullOutWeapon();
			}
			else
			{
				StartIdle();
			}
		}

		protected virtual void PullOutWeapon()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(pullOutAnimationClip, 1);

		}

		protected virtual void StartIdle()
		{
			SetState(weaponState.idle);
		}
		public override void Resume()
		{
			SetState(weaponState.resuming);
			TryToActivate();
		}

		protected override void AnimationComplete()
		{
			StartIdle();
		}

		public override void StopAbility()
		{
			currentState = weaponState.not;
			base.StopAbility();
		}


	}
}
