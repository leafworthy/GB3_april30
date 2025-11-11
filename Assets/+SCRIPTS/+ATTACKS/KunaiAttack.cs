using System;
using __SCRIPTS.Projectiles;
using UnityEngine;

namespace __SCRIPTS
{
	public class KunaiAttack : Ability
	{
		public override string AbilityName => "Throw-Kunai";
		public AnimationClip throwKunaiAnimationClip;
		public AnimationClip throwAirKunaiAnimationClip;
		public event Action OnThrow;

		private bool isPressingAttack;
		private IAimAbility aim => _aim ??= GetComponent<IAimAbility>();
		private IAimAbility _aim;
		private AmmoInventory ammoInventory => _ammoInventory ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammoInventory;

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		protected override void DoAbility()
		{
			StartAttack();
		}

		private void StartAttack()
		{
			Debug.Log("start kunai attacka ");

			if (jumpAbility.IsInAir)
			{
				PlayAnimationClip(throwAirKunaiAnimationClip);
				Invoke(nameof(ThrowAirKunai), throwAirKunaiAnimationClip.length / 4);
			}
			else
			{
				PlayAnimationClip(throwKunaiAnimationClip);
				Invoke(nameof(ThrowRegularKunai), throwKunaiAnimationClip.length / 4);
			}
		}

		private void ThrowRegularKunai()
		{
			if (!ammoInventory.primaryAmmo.hasReserveAmmo(1)) return;
			ThrowKunai(aim.AimDir, body.ThrowPoint.transform.position);
		}

		private void ThrowAirKunai()
		{
			if (!ammoInventory.primaryAmmo.hasReserveAmmo(3)) return;
			var directionMult = body.BottomIsFacingRight ? 1 : -1;
			ThrowKunai(new Vector3(directionMult, -.5f, 0), body.ThrowPoint.transform.position, true);
			ThrowKunai(new Vector3(directionMult, -1f, 0), body.ThrowPoint.transform.position, true);
			ThrowKunai(new Vector3(directionMult, -.75f, 0), body.ThrowPoint.transform.position, true);
		}

		public override bool canDo() => base.canDo() && ammoInventory.primaryAmmo.hasReserveAmmo();

		private bool alreadyDone;
		private JumpAbility jumpAbility  => _jumpAbility ??= GetComponent<JumpAbility>();
		private JumpAbility _jumpAbility;


		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			if (alreadyDone) Debug.LogWarning("double dip");
			alreadyDone = true;
			player.Controller.Attack2LeftTrigger.OnPress -= StartPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= StopPressing;
			player.Controller.Attack2LeftTrigger.OnPress += StartPress;
			player.Controller.Attack2LeftTrigger.OnRelease += StopPressing;
		}

		private void StopPressing(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		private void OnDisable()
		{
			if (player == null) return;
			player.Controller.Attack2LeftTrigger.OnPress -= StartPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= StopPressing;
		}

		private void ThrowKunai(Vector2 direction, Vector2 position, bool isAirThrow = false)
		{
			Debug.Log("throw kunai");
			ammoInventory.primaryAmmo.UseAmmo(1);
			var throwHeight = position.y - transform.position.y;
			Debug.Log("throw height " + throwHeight);
			var newProjectile = Services.objectMaker.Make(Services.assetManager.FX.kunaiPrefab, position);
			var kunaiScript = newProjectile.GetComponent<Kunai>();

			kunaiScript.Throw(direction, position, throwHeight, player.spawnedPlayerAttacker, isAirThrow);
			OnThrow?.Invoke();
		}




		protected override void AnimationComplete()
		{
			if (isPressingAttack)
			{
				Debug.Log("re-offence");
				StartAttack();
			}
			else
				base.AnimationComplete();
		}

		private void StartPress(NewControlButton newControlButton)
		{
			if (isPressingAttack) return;
			isPressingAttack = true;
			Debug.Log("start press kunai");
			Do();
		}
	}
}
