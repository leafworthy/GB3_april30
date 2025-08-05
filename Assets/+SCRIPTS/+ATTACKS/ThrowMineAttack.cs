using System;
using System.Collections.Generic;
using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class ThrowMineAttack : ServiceUser, INeedPlayer, IActivity
	{
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;

		private Player player;

		private GunAimAbility aim;
		private MoveAbility move;
		private AmmoInventory ammo;
		private JumpAbility jump;
		private Life life;
		private Body body;
		private Arms arms => body.arms;
		private UnitAnimations anim;
		private AnimationEvents animationEvents;

		private bool isPressing;
		private List<Mine> ActiveMines  = new();
		private bool detonatePressed;
		public AnimationClip mineDropAnimation;
		public string VerbName => "Throw-Mine";

		public bool TryCompleteGracefully(GangstaBean.Core.CompletionReason reason, GangstaBean.Core.IActivity newActivity = null)
		{
			switch (reason)
			{
				case GangstaBean.Core.CompletionReason.AnimationInterrupt:
				case GangstaBean.Core.CompletionReason.NewActivity:
					return true;
			}
			return false;
		}

		public event Action<Vector2, Player> OnThrow;

		public void SetPlayer(Player _player)
		{
			anim = GetComponent<UnitAnimations>();
			animationEvents = anim.animEvents;
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			player = _player;
			jump = GetComponent<JumpAbility>();
			move = GetComponent<MoveAbility>();
			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();
			ListenToPlayer();
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void StopListeningToPlayer()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (anim == null) return;
			animationEvents.OnThrow -= Anim_Throw;
			animationEvents.OnThrowStop -= Anim_ThrowStop;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_ThrowPress;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_ThrowRelease;
			player.Controller.ReloadTriangle.OnPress -= Player_DetonatePress;
			player.Controller.ReloadTriangle.OnRelease -= Player_DetonateRelease;
		}

		private void ListenToPlayer()
		{
			if (player == null) return;
			animationEvents = anim.animEvents;
			animationEvents.OnThrow += Anim_Throw;
			animationEvents.OnThrowStop += Anim_ThrowStop;
			player.Controller.Attack2LeftTrigger.OnPress += Player_ThrowPress;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_ThrowRelease;
			player.Controller.ReloadTriangle.OnPress += Player_DetonatePress;
			player.Controller.ReloadTriangle.OnRelease += Player_DetonateRelease;
		}

		private void Player_DetonateRelease(NewControlButton obj)
		{
			if (!detonatePressed) return;
			detonatePressed = false;
		}

		private void Player_DetonatePress(NewControlButton obj)
		{
			if (detonatePressed) return;
			detonatePressed = true;
			if (ActiveMines.Count <= 0) return;
			if (ActiveMines[0] == null) return;
			ActiveMines[0].Detonate();
			RemoveMine(ActiveMines[0]);
		}

		private void Anim_ThrowStop()
		{
			arms.Stop(this);
			if (isPressing) Player_ThrowPress(null);
		}

		private void Player_ThrowPress(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			if (isPressing) return;

			if (!ammo.secondaryAmmo.hasReserveAmmo())
			{

				return;
			}

			if (!arms.Do(this))
			{
				if ((jump.IsJumping))
				{
					if (arms.currentActivity?.VerbName == VerbName) return;
					arms.StopCurrentActivity();
					arms.Do(this);
				}
				else
				{
					return;
				}
			}

			isPressing = true;

			anim.Play(mineDropAnimation.name, 1, 0);
		}

		private void Player_ThrowRelease(NewControlButton obj)
		{

			isPressing = false;
		}

		private void Anim_Throw()
		{
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = transform.position;
			OnThrow?.Invoke(startPoint, life.player);

			var newProjectile = objectMaker.Make( assets.FX.minePrefab, startPoint);
			var newMine = newProjectile.GetComponent<Mine>();
			newMine.Launch(startPoint, player);
			newMine.OnSelfDetonate += RemoveMine;
			ActiveMines.Add(newMine);
		}

		private void RemoveMine(Mine mine)
		{
			if (ActiveMines.Contains(mine))
			{
				ActiveMines.Remove(mine);
				mine.OnSelfDetonate -= RemoveMine;
			}
			else
			{

			}
		}

	}
}
