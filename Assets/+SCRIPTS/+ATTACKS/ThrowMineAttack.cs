using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class ThrowMineAttack : Ability
	{
		public override string AbilityName => "Throw-Mine";
		public AnimationClip mineDropAnimation;
		Vector2 startPoint;
		Vector2 endPoint;
		public event Action<Vector2, Player> OnThrow;
		AmmoInventory ammo => _ammo ??= GetComponent<AmmoInventory>();
		AmmoInventory _ammo;

		List<Mine> ActiveMines = new();
		bool isPressingThrowMine;
		bool isPressingDetonate;

		bool isThrowing;
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && ammo.secondaryAmmo.hasReserveAmmo();

		void DropMine()
		{
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = transform.position;
			OnThrow?.Invoke(startPoint, defence.player);
			isThrowing = true;

			var newProjectile = Services.objectMaker.Make(Services.assetManager.FX.minePrefab, startPoint);
			var newMine = newProjectile.GetComponent<Mine>();
			newMine.Launch(startPoint, offence);
			newMine.OnSelfDetonate += RemoveMine;
			ActiveMines.Add(newMine);
		}

		protected override void DoAbility()
		{
			PlayAnimationClip(mineDropAnimation, 1);
			Invoke(nameof(DropMine), mineDropAnimation.length / 2);

			anim.Play(mineDropAnimation.name, 1, 0);
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			ListenToPlayer();
		}

		void OnDestroy()
		{
			StopListeningToPlayer();
		}

		void StopListeningToPlayer()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.SwapWeaponSquare.OnPress -= Player_ThrowPress;
			player.Controller.SwapWeaponSquare.OnRelease -= Player_ThrowRelease;
		}

		void ListenToPlayer()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.SwapWeaponSquare.OnPress += Player_ThrowPress;
			player.Controller.SwapWeaponSquare.OnRelease += Player_ThrowRelease;
		}


		void DetonateMine()
		{
			if (ActiveMines.Count <= 0) return;
			if (ActiveMines[0] == null) return;
			ActiveMines[0].Detonate();
			RemoveMine(ActiveMines[0]);
		}

		protected override void AnimationComplete()
		{
			isThrowing = false;
			if (isPressingThrowMine)
			{
				Player_ThrowPress(null);
				return;
			}

			base.AnimationComplete();
			lastArmAbility?.TryToActivate();
		}

		void Player_ThrowPress(NewControlButton newControlButton)
		{
			if (Services.pauseManager.IsPaused) return;
			if (ActiveMines.Count > 0)
			{
				DetonateMine();
				return;
			}

			if (isThrowing) return;
			if (body.doableArms.CurrentAbility is not ThrowMineAttack) lastArmAbility = body.doableArms.CurrentAbility;
			TryToActivate();
		}

		void Player_ThrowRelease(NewControlButton obj)
		{
			isPressingThrowMine = false;
		}

		void RemoveMine(Mine mine)
		{
			if (!ActiveMines.Contains(mine)) return;
			ActiveMines.Remove(mine);
			mine.OnSelfDetonate -= RemoveMine;
		}
	}
}
