using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public abstract class WeaponAbility : Ability
	{
		protected enum weaponState
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
		protected bool isActive => currentState is weaponState.idle or weaponState.attacking;
		protected bool isIdle => currentState is weaponState.idle;

		protected weaponState currentState {get; private set;}

		public AnimationClip pullOutAnimationClip;
		public override string AbilityName => "Weapon-Ability";

		public override bool requiresArms() => true;
		public override bool requiresLegs() => false;

		public override bool canStop(Ability abilityToStopFor) => isIdle;

		protected override void DoAbility()
		{
			if(currentState != weaponState.resuming && currentState != weaponState.pullOut)
			{
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
			TryToDoAbility();
		}

		protected override void AnimationComplete()
		{
			StartIdle();
		}

		public override void StopAbilityBody()
		{
			SetState(weaponState.not);
			base.StopAbilityBody();
		}


	}
}
