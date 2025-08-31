using System;
using __SCRIPTS.HUD_Displays;
using __SCRIPTS.Projectiles;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class PrimaryAttack_Kunai : ServiceUser, INeedPlayer, IActivity
	{
		private AnimationEvents animationEvents;

		private IAimAbility aim;
		private UnitAnimations anim;
		private Body body;
		private AmmoInventory ammoInventory;
		public string VerbName => "Throw-Kunai";
		private bool isPressing;
		private Player player;
		private string verbName;
		public event Action<Vector3, Vector3, float, Life, bool> OnThrow;

		public bool TryCompleteGracefully(CompletionReason reason, IActivity newActivity = null)
		{
			switch (reason)
			{
				case CompletionReason.AnimationInterrupt:
				case CompletionReason.NewActivity:
					// Handle graceful completion
					isPressing = false;
					body.arms.Stop(this);
					anim.ResetTrigger(UnitAnimations.ThrowTrigger);
					return true;
			}
			return false;
		}

		private void OnEnable()
		{
			ammoInventory = GetComponent<AmmoInventory>();
			body = GetComponent<Body>();
			anim = GetComponent<UnitAnimations>();
			aim = GetComponent<IAimAbility>();


		}

		public void SetPlayer(Player _player)
		{
			if (_player == null) return;
			player = _player;
			player.Controller.Attack1RightTrigger.OnPress += StartPress;
			player.Controller.Attack1RightTrigger.OnRelease += StopPressing;
			animationEvents = anim.animEvents;
			animationEvents.OnThrow += Anim_Throw;
			animationEvents.OnThrowStop += Anim_ThrowStop;
		}
		private void StopPressing(NewControlButton obj)
		{
			if (!isPressing) return;
			isPressing = false;
			body.arms.Stop(this);
			anim.ResetTrigger(UnitAnimations.ThrowTrigger);
		}

		private void OnDisable()
		{
			player.Controller.Attack1RightTrigger.OnPress -= StartPress;
			player.Controller.Attack1RightTrigger.OnRelease -= StopPressing;
			animationEvents.OnThrow -= Anim_Throw;
			animationEvents.OnThrowStop -= Anim_ThrowStop;

		}

		private void Anim_Throw()
		{
			if (!ammoInventory.primaryAmmo.hasReserveAmmo()) return;
			if (!body.arms.Do(this)) return;
			ammoInventory.primaryAmmo.UseAmmo( 1);

			var throwHeight = body.ThrowPoint.transform.position.y - transform.position.y;
			var newProjectile = objectMaker.Make(assets.FX.kunaiPrefab, transform.position);
			var kunaiScript = newProjectile.GetComponent<Kunai>();
			kunaiScript.Throw(aim.AimDir, transform.position, throwHeight, player.spawnedPlayerDefence);
			OnThrow?.Invoke(aim.AimDir, transform.position, throwHeight, player.spawnedPlayerDefence, false);
		}

		private void Anim_ThrowStop()
		{
			body.arms.Stop(this);
			if (isPressing)
			{
				StartAttack();
			}
			else
			{

			}
		}


		private void StartPress(NewControlButton newControlButton)
		{
			if (isPressing) return;
			isPressing = true;
			StartAttack();
		}

		private void StartAttack()
		{
			if (!ammoInventory.primaryAmmo.hasReserveAmmo())
			{

				return;
			}

			anim.SetTrigger(UnitAnimations.ThrowTrigger);

		}
	}
}
