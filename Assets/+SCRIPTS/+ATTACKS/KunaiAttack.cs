using System;
using __SCRIPTS.Projectiles;
using UnityEngine;

namespace __SCRIPTS
{
	public class KunaiAttack : Ability
	{
		public override string AbilityName => "Throw-Kunai";
		public AnimationClip throwKunaiAnimationClip;
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
			PlayAnimationClip(throwKunaiAnimationClip);
			Invoke( nameof(ThrowKunai), throwKunaiAnimationClip.length / 2);
		}

		public override bool canDo() => ammoInventory.primaryAmmo.hasReserveAmmo();



		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.Attack2LeftTrigger.OnPress += StartPress;
			player.Controller.Attack2LeftTrigger.OnRelease += StopPressing;
		}

		private void StopPressing(NewControlButton obj)
		{
			isPressingAttack = false;
		}

		private void OnDisable()
		{
			player.Controller.Attack2LeftTrigger.OnPress -= StartPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= StopPressing;
		}

		private void ThrowKunai()
		{
			ammoInventory.primaryAmmo.UseAmmo(1);
			var throwHeight = body.ThrowPoint.transform.position.y - transform.position.y;
			var newProjectile = Services.objectMaker.Make(Services.assetManager.FX.kunaiPrefab, transform.position);
			var kunaiScript = newProjectile.GetComponent<Kunai>();
			kunaiScript.Throw(aim.AimDir, transform.position, throwHeight, player.spawnedPlayerDefence);
			OnThrow?.Invoke();
		}

		protected override void AnimationComplete()
		{
			if (isPressingAttack)
			{
				StartAttack();
			}
			else
			{
				base.AnimationComplete();
			}
		}

		private void StartPress(NewControlButton newControlButton)
		{
			isPressingAttack = true;
			Do();
		}


	}
}
