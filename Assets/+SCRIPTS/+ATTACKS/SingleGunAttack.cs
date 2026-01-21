using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class SingleGunAttack : WeaponAbility
	{
		[SerializeField] Gun currentGun;

		JumpAbility jumpAbility => _jumpAbility ??= GetComponent<JumpAbility>();
		JumpAbility _jumpAbility;
		public override string AbilityName => "Gun Attack " + currentState;
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState is weaponState.idle or weaponState.resuming;

		bool isPressingShoot;

		public event Action OnNeedsReload;
		public event Action OnEmpty;

		public void EquipGun(Gun newGun)
		{
			if(newGun == null)
			{
				Debug.LogError("Trying to equip null gun");
				return;
			}
			if (newGun == currentGun) return;
			currentGun = newGun;
			PullOutGun();
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
			if (!isIdle || !jumpAbility.IsResting) return;

			if (!currentGun.Shoot())
			{
				Debug.Log("can't shoot");
				return;
			}
			SetState(weaponState.attacking);
			PlayShootAnimation();
		}

		void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(currentGun.GetShootClipName(), currentGun.AttackRate, 1);
		}

		void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(currentGun.AimDir.x >= 0);
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
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
			PlayAnimationClip(currentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			StopListeningToEvents();
			ListenToEvents();
			PullOutGun();
		}

		void PullOutGun()
		{
			TryToActivate();
		}


		void ListenToEvents()
		{
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		void OnDisable()
		{
			StopListeningToEvents();
		}

		void StopListeningToEvents()
		{
			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		void FixedUpdate()
		{
			if (currentState != weaponState.idle) return;
			if (isPressingShoot) StartAttacking();
			else Aim();
		}

		void Aim()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClipWithoutEvent(currentGun.GetClipNameFromDegrees(), 1);
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
