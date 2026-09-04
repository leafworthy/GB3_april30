using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class SimpleWeaponAttack : WeaponAbility, IGunAttack
	{


		public GameObject AttackStartPoint;
		public int WeaponAttackIndex;
		public Gun CurrentGun => _gun;
		public Gun _gun;
		public bool IsUsingPrimaryGun => true;
		IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _aimAbility;
		Vector2 AimDir => aimAbility.AimDir;
		public override string AbilityName => "Simple-Attack";
		protected override bool requiresArms() => true;
		protected override bool requiresLegs() => false;
		float cooldownCounter;
		bool isPressingShoot;

		public override bool canStop(IDoableAbility abilityToStopFor) => currentState is weaponState.idle or weaponState.not;

		public event Action<Vector2> OnStartAttacking;
		public event Action<Vector2> OnStopAttacking;
		public event Action<Vector2> OnStopContinuouslyAttacking;
		public event Action<Vector2> OnStartContinusoulyAttacking;
		public event Action<Vector2> OnReload;


		public override void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			StopListeningToEvents();
			ListenToEvents();
		}

		void StopAttacking()
		{
			if (currentState != weaponState.attacking) return;
			StartIdle();
			Aim();
			OnStopAttacking?.Invoke(transform.position);
		}

		void ListenToEvents()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress += PlayerControllerReloadPress;
		}

		void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			isPressingShoot = true;
			StartAttacking();
		}

		void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
			StopAttacking();
		}

		void StopListeningToEvents()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
			player.Controller.ReloadTriangle.OnPress -= PlayerControllerReloadPress;
		}
		void PlayerControllerReloadPress(NewControlButton obj)
		{
			if (currentState != weaponState.idle) return;
			StartReloading();
		}

		public override void StopAbility()
		{
			if(CurrentGun.isContinuous)OnStopContinuouslyAttacking?.Invoke(transform.position);
			StopBody();
			SetState(weaponState.not);
		}

		public void SwapGuns()
		{
		}

		public bool CanSwapGuns() => false;

		public override void Resume()
		{
			StartIdle();
		}

		protected override void StartIdle()
		{
			SetState(weaponState.idle);
			if (!CurrentGun.CanReload() || !CurrentGun.MustReload()) return;
			StartReloading();
		}

		void StartReloading()
		{
			if (!isPressingShoot || currentState != weaponState.idle) return;
			SetState(weaponState.reloading);
			OnReload?.Invoke(transform.position);
			PlayAnimationClip(CurrentGun.pullOutAnimationClip, 1);
		}

		void StartAttacking()
		{
			if (!CurrentGun.CanShoot()) return;
			CurrentGun.Shoot();
			SetState(weaponState.attacking);
			anim.SetBool(UnitAnimations.IsAttacking, true);
			OnStartAttacking?.Invoke(transform.position);
			PlayShootAnimation();
		}

		void PlayShootAnimation()
		{
			TopFaceCorrectDirection();
			anim.SetFloat(UnitAnimations.ShootSpeed, 1);
			PlayAnimationClip(CurrentGun.GetShootClipName(), CurrentGun.AttackRate, 1);
		}

		void TopFaceCorrectDirection()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

		protected void FixedUpdate()
		{
			if (!isActive) return;

			switch (currentState)
			{
				case weaponState.idle:
					if(isPressingShoot)StartAttacking();
					Aim();
					break;
				case weaponState.attacking:
					if(CurrentGun.isContinuous) AttackContinuously();
					break;
				case weaponState.reloading:
					break;
				case weaponState.not:
					break;
				case weaponState.pullOut:
					break;
				case weaponState.resuming:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void Aim()
		{
			TopFaceCorrectDirection();
			if (CurrentGun.hasSimpleAiming) return;
			if (!body.doableArms.CanDoActivity(this) && !body.doableArms.isDoingAbility(this)) return;
			anim.SetFloat(UnitAnimations.ShootSpeed, 0);
			PlayAnimationClipWithoutEvent(CurrentGun.GetClipNameFromDegrees(), 1);
		}
		void AttackContinuously()
		{
			cooldownCounter += Time.fixedDeltaTime;
			if (!(cooldownCounter >= offence.stats.Stats.Rate(WeaponAttackIndex))) return;
			cooldownCounter = 0;
			MyAttackUtilities.HitTargetsWithinRange(offence, AttackStartPoint.transform.position, offence.stats.Stats.Range(WeaponAttackIndex),
				offence.stats.Stats.Damage(WeaponAttackIndex));
		}
		protected override void DoAbility()
		{
			if (currentState == weaponState.not) PullOutWeapon();
		}

		protected override void AnimationComplete()
		{
			switch (currentState)
			{
				case weaponState.not:
					break;
				case weaponState.pullOut:
					StartIdle();
					break;
				case weaponState.idle:
					break;
				case weaponState.attacking:
					StartIdle();
					break;
				case weaponState.resuming:
					StartIdle();
					break;
				case weaponState.reloading:
					StartIdle();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void PullOutWeapon()
		{
			Debug.Log("pull out here, hudSlotState: " + currentState, this);
			SetState(weaponState.pullOut);
			PlayAnimationClip(CurrentGun.pullOutAnimationClip, 1);
			OnStartContinusoulyAttacking?.Invoke(transform.position);
		}
	}
}
