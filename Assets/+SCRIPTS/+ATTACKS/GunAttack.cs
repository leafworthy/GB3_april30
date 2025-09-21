using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable]
	public class GunAttack : WeaponAbility
	{
		public Gun CurrentGun => currentGun;
		public AnimationClip simpleShootAnimationClip;
		private Gun currentGun;
		private float currentCooldownTime;

		public Gun primaryGun => _primaryGun ??= GetComponent<PrimaryGun>();
		private Gun _primaryGun;
		public Gun unlimitedGun => _unlimitedGun ??= GetComponent<UnlimitedGun>();
		private Gun _unlimitedGun;
		private IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _aimAbility;
		public Vector2 AimDir => aimAbility.AimDir;
		public override string AbilityName => "Gun Attack " + currentState.ToString();
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		public override bool canStop(IDoableAbility abilityToStopFor) => currentState == weaponState.idle || currentState == weaponState.resuming;
		public bool IsUsingPrimaryGun => currentGun is PrimaryGun;
		private bool isPressingShoot;

		public event Action OnNeedsReload;

		public override void Stop()
		{
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			base.Stop();
		}

#pragma warning disable UDR0001
		private static string[] PrimaryAnimationClips =
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

		private void StartAttacking()
		{
			if (!isActive || currentState != weaponState.idle) return;
			SetState(weaponState.attacking);
			CurrentGun.Shoot(AimDir);
			PlayShootAnimation();

		}

		private void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			if (currentGun.simpleShoot)

				PlayAnimationClip(simpleShootAnimationClip.name, CurrentGun.AttackRate, 1);
			else
				PlayAnimationClip(currentGun.GetClipNameFromDegrees(), CurrentGun.AttackRate, .25f,1);
		}
		private void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected override void StartIdle()
		{
			base.StartIdle();
			if (!currentGun.MustReload()) return;
			Stop();
			OnNeedsReload?.Invoke();
		}

		protected override void DoAbility()
		{
			Debug.Log("gun attack start");
			switch (currentState)
			{
				case weaponState.resuming:
					StartIdle();
					return;
				default:
					PullOut();
					break;
			}
		}

		protected override void PullOut()
		{
			SetState(weaponState.pullOut);
			PlayAnimationClip(currentGun.pullOutAnimationClip, 1);
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			currentGun = primaryGun;

			StopListeningToEvents();
			ListenToEvents();
			Do();
		}

		private void Gun_OnNeedsReload()
		{
			Stop();
			OnNeedsReload?.Invoke();
		}

		public override bool canDo() => base.canDo() && CurrentGun.CanShoot();

		private void ListenToEvents()
		{
			if (primaryGun != null) primaryGun.OnNeedsReload += Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload += Gun_OnNeedsReload;

			if (player == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		private void OnDestroy()
		{
			StopListeningToEvents();
		}

		private void StopListeningToEvents()
		{
			if (primaryGun != null) primaryGun.OnNeedsReload -= Gun_OnNeedsReload;
			if (unlimitedGun != null) unlimitedGun.OnNeedsReload -= Gun_OnNeedsReload;

			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		private void FixedUpdate()
		{
			if (!isActive) return;
			if (isPressingShoot && currentState != weaponState.attacking) StartAttacking();
			else if (currentState == weaponState.idle) Aim();
		}

		private void Aim()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClip(currentGun.GetClipNameFromDegrees(), 0, 1);
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			StartAttacking();
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}


		private void SwitchGuns(bool toPrimary)
		{
			currentGun = toPrimary ? primaryGun : unlimitedGun;
			Stop();
			Do();
		}

		public void SwapGuns()
		{
			Debug.Log("swapping guns to isprimary= " + (!(currentGun is PrimaryGun)));
			SwitchGuns(!(currentGun is PrimaryGun));
		}
	}
}
