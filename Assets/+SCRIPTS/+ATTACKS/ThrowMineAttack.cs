using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class ThrowMineAttack : Ability
	{
		public override string AbilityName => "Throw-Mine";
		public AnimationClip mineDropAnimation;
		private Vector2 startPoint;
		private Vector2 endPoint;
		public event Action<Vector2, Player> OnThrow;
		private AmmoInventory ammo => _ammo ??= GetComponent<AmmoInventory>();
		private AmmoInventory _ammo;
		private CharacterJumpAbility jumpAbility => _jumpAbility ??= GetComponent<CharacterJumpAbility>();
		private CharacterJumpAbility _jumpAbility;

		private List<Mine> ActiveMines = new();
		private bool isPressingThrowMine;
		private bool isPressingDetonate;
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && ammo.secondaryAmmo.hasReserveAmmo() && jumpAbility.IsResting;

		private void DropMine()
		{
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = transform.position;
			OnThrow?.Invoke(startPoint, life.Player);

			var newProjectile = Services.objectMaker.Make(Services.assetManager.FX.minePrefab, startPoint);
			var newMine = newProjectile.GetComponent<Mine>();
			newMine.Launch(startPoint, player);
			newMine.OnSelfDetonate += RemoveMine;
			ActiveMines.Add(newMine);
		}

		protected override void DoAbility()
		{
			PlayAnimationClip(mineDropAnimation, 1);
			Invoke(nameof(DropMine), mineDropAnimation.length / 2);

			anim.Play(mineDropAnimation.name, 1, 0);
		}



		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			ListenToPlayer();
		}

		private void OnDestroy()
		{
			StopListeningToPlayer();
		}

		private void StopListeningToPlayer()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_ThrowPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_ThrowRelease;
			player.Controller.ReloadTriangle.OnPress -= Player_DetonatePress;
			player.Controller.ReloadTriangle.OnRelease -= Player_DetonateRelease;
		}

		private void ListenToPlayer()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.Attack2LeftTrigger.OnPress += Player_ThrowPress;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_ThrowRelease;
			player.Controller.ReloadTriangle.OnPress += Player_DetonatePress;
			player.Controller.ReloadTriangle.OnRelease += Player_DetonateRelease;
		}

		private void Player_DetonateRelease(NewControlButton obj) => isPressingDetonate = false;

		private void Player_DetonatePress(NewControlButton obj)
		{
			if (isPressingDetonate) return;
			isPressingDetonate = true;
			DetonateMine();
		}

		private void DetonateMine()
		{
			if (ActiveMines.Count <= 0) return;
			if (ActiveMines[0] == null) return;
			ActiveMines[0].Detonate();
			RemoveMine(ActiveMines[0]);
		}

		protected override void AnimationComplete()
		{
			if (isPressingThrowMine)
			{
				Player_ThrowPress(null);
				return;
			}

			base.AnimationComplete();
		}

		private void Player_ThrowPress(NewControlButton newControlButton)
		{
			if (Services.pauseManager.IsPaused) return;
			Do();
		}

		private void Player_ThrowRelease(NewControlButton obj)
		{
			isPressingThrowMine = false;
		}

		private void RemoveMine(Mine mine)
		{
			if (!ActiveMines.Contains(mine)) return;
			ActiveMines.Remove(mine);
			mine.OnSelfDetonate -= RemoveMine;
		}
	}
}
