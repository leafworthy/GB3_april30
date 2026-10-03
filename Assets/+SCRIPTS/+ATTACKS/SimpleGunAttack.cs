using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class SimpleGunAttack : GunAttack
	{

		IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _aimAbility;
		public Vector2 AimDir => aimAbility.AimDir;
		public override string AbilityName => "Gun Attack " + currentState;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || currentState == weaponState.resuming;

		JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		JumpAbility _jumpAbility;

		public override void Resume()
		{
			SetState(weaponState.idle);
		}

		void Start()
		{
			TryToActivate();
		}

		protected override void StartAttacking()
		{
			if (!isIdle || !jumpAbility.IsResting) return;

			if (!CurrentGun.Shoot()) return;
			SetState(weaponState.attacking);
			PlayShootAnimation();
		}

		protected override void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(CurrentGun.GetShootClipName(), CurrentGun.AttackRate, 1);
		}

		protected override void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected override void DoAbility()
		{
			Debug.Log("current state: " + currentState, this);
			switch (currentState)
			{
				case weaponState.resuming:
				case weaponState.pullOut:
					StartIdle();
					break;
				case weaponState.not:
					PullOutWeapon();
					break;
				case weaponState.idle:
					StartIdle();
					break;
				case weaponState.attacking:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}



		protected override void PullOutWeapon()
		{
			Debug.Log("pull out here, hudSlotState: " + currentState, this);
			SetState(weaponState.pullOut);
			anim.SetBool(UnitAnimations.IsUsingPrimary, true);
			PlayAnimationClip(CurrentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			StopListeningToEvents();
			ListenToEvents();
			if (CurrentGun.HasAnyAmmo()) TryToActivate();
		}


		protected override void ListenToEvents()
		{
			if (primaryGun != null) primaryGun.OnNeedsReload += Gun_OnNeedsReload;
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		void OnDisable()
		{
			StopListeningToEvents();
		}

		protected override void StopListeningToEvents()
		{
			if (primaryGun != null) primaryGun.OnNeedsReload -= Gun_OnNeedsReload;

			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.Attack1RightTrigger == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}


	}
}
