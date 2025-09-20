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
			putAway,
			resuming
		}


		protected void SetState(weaponState state)
		{
			currentState = state;
		}
		protected bool  isActive = true;

		public weaponState currentState {get; private set;}
		public event Action OnPutAwayComplete;
		public AnimationClip pullOutAnimationClip;
		public AnimationClip putAwayAnimationClip;
		public override string AbilityName => "Weapon-Ability";

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle;

		protected override void DoAbility()
		{
			PullOut();
		}

		protected virtual void PullOut()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(pullOutAnimationClip, 1);

		}

		public virtual void PutAway()
		{
			SetState(weaponState.putAway);
			PlayAnimationClip(putAwayAnimationClip, 1);
		}


		protected override void AnimationComplete()
		{
			switch (currentState)
			{
				case weaponState.putAway:
					base.AnimationComplete();
					OnPutAwayComplete?.Invoke();
					Stop();
					Debug.Log("[WEAPON] put away complete in weapon ability base", this);
					return;
				case weaponState.pullOut:
					Debug.Log("[WEAPON] pull out complete in weapon ability base", this);
					StartIdle();
					break;
				case weaponState.attacking:
					Debug.Log("[WEAPON] attack complete in weapon ability base", this);
					 StartIdle();
					 break;
			}
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
