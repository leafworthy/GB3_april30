using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttackSingle : WeaponAbility
	{
		public Gun CurrentGun;
		IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _aimAbility;
		public Vector2 AimDir => aimAbility.AimDir;
		public override string AbilityName => "Gun Attack " +CurrentGun.name + " " +currentState;
		public override bool requiresArms() => true;
		public override bool requiresLegs() => false;
		public override bool canStop(Ability abilityToStopFor) => currentState is weaponState.idle or weaponState.resuming;

		bool isPressingShoot;
		public event Action OnEmpty;
		public event Action OnNeedsReload;

		public override void StopAbilityBody()
		{
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			base.StopAbilityBody();
		}

#pragma warning disable UDR0001
		static string[] PrimaryAnimationClips =
		{
			"E",
			"EES",
			"ES",
			"SE",
			"SSE",
			"SSE",
			"SE",
			"ES",
			"EES",
			"E",
			"EEN",
			"EN",
			"NE",
			"NNE",
			"NNE",
			"NE",
			"EN",
			"EEN"
		};
#pragma warning restore UDR0001

		void StartAttacking()
		{
			if (!isIdle || !body.isGrounded) return;

			if (!CurrentGun.CanShoot())
			{

				if (CurrentGun.CanReload()) Gun_OnNeedsReload();
				else OnEmpty?.Invoke();


				return;
			}

			SetState(weaponState.attacking);
			PlayShootAnimation();
		}

		void PlayShootAnimation()
		{
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(CurrentGun.GetShootClipName(), CurrentGun.AttackRate, 1);
		}
		void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			StopAbilityBody();
			if (CurrentGun is not PrimaryGun) return;
			if (!CurrentGun.MustReload()) return;
			if (CurrentGun.CanReload()) OnNeedsReload?.Invoke();
			else OnEmpty?.Invoke();
		}

		protected override void DoAbility()
		{
			switch (currentState)
			{
				case weaponState.resuming:
					StartIdle();
					return;
				default:
					PullOutWeapon();
					break;
			}
		}

		protected override void PullOutWeapon()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(CurrentGun.PullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);

			StopListeningToEvents();
			ListenToEvents();
			TryToDoAbility();
		}





		void Gun_OnNeedsReload()
		{
			StopAbilityBody();
			OnNeedsReload?.Invoke();
			StartReloading();
		}

		void StartReloading()
		{
		}

		void ListenToEvents()
		{
			if (CurrentGun != null) CurrentGun.OnEmpty += Gun_OnEmpty;
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		void Gun_OnEmpty()
		{
			StopAbilityBody();
			OnEmpty?.Invoke();
		}

		void OnDestroy()
		{
			StopListeningToEvents();
		}

		void OnDisable()
		{
			StopListeningToEvents();
		}

		void StopListeningToEvents()
		{

			if (CurrentGun != null) CurrentGun.OnEmpty -= Gun_OnEmpty;
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		void FixedUpdate()
		{
			if (!isActive) return;
			if (isPressingShoot) StartAttacking();
			else if (currentState == weaponState.idle) Aim();
		}

		void Aim()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClipWithoutEvent(CurrentGun.GetClipNameFromDegrees(), 1);
		}

		void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			StartAttacking();
		}

		void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}


	}
}
