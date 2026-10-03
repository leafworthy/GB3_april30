using System;
using __SCRIPTS.Projectiles;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class SimpleNadeAttack : Ability
	{
		Vector2 startPoint;
		Vector2 endPoint;

		const int throwTime = 30;

		IAimAbility aim => _aim ??= GetComponent<IAimAbility>();
		IAimAbility _aim;
		MoveAbility move;
		AmmoInventory ammo;

		public override string AbilityName => "Nade";
		JumpAbility jump => _jump ??= GetComponent<JumpAbility>();
		JumpAbility _jump;

		float currentCooldownTime;
		[SerializeField] AnimationClip animationClip;

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canStop(IDoableAbility abilityToStopFor) => abilityToStopFor is JumpAbility or DashAbility or RunDashAbility or ChainsawAttack;
		public override bool canDo() => ammo.secondaryAmmo.hasReserveAmmo() && !jump.IsInAir && base.canDo();

		protected override void DoAbility()
		{
			ThrowGrenade();
		}

		public override void StopAbility()
		{
			base.StopAbility();
			lastArmAbility?.Resume();
		}

		public override void SetPlayer(Player newPlayer)
		{
			base.SetPlayer(newPlayer);
			move = GetComponent<MoveAbility>();
			ammo = GetComponent<AmmoInventory>();
			StopListeningToPlayer();
			ListenToPlayer();
		}

		void OnDisable()
		{
			StopListeningToPlayer();
		}

		void StopListeningToPlayer()
		{
			if (player == null) return;
			if (player.Controller == null) return;
			if (player.Controller.AimAxis == null) return;
			player.Controller.AimAxis.OnChange -= Player_OnAim;
			if (player.Controller.Attack2LeftTrigger == null) return;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_NadePress;
		}

		void ListenToPlayer()
		{
			if (player == null) return;
			player.Controller.AimAxis.OnChange += Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress += Player_NadePress;
		}


		void Player_NadePress(NewControlButton newControlButton)
		{
			TryToActivate();
		}



		void ThrowGrenade()
		{
			PlayAnimationClip(animationClip, 1);
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime, (endPoint.y - startPoint.y) / throwTime);

			var newProjectile = Services.objectMaker.Make(Services.assetManager.FX.nadePrefab, body.AimCenter.transform.position);
			if (newProjectile == null) return;

			var nadeThrower = newProjectile.GetComponent<Nade>();
			if (nadeThrower == null) return;

			nadeThrower.Launch(startPoint, velocity, throwTime, offence);

		}



		void Player_OnAim(IControlAxis controlAxis, Vector2 aimDir)
		{
			if (defence.IsDead()) return;
			if (Services.pauseManager.IsPaused) return;
			Aim();
		}

		void Aim()
		{
			if (body == null) return;

			startPoint = body.AimCenter.transform.position;
			if (!player.Controller.AimAxis.isActive && !player.Controller.MoveAxis.isActive)
			{
				var whichDir = body.TopIsFacingRight;
				endPoint = whichDir ? (Vector2) transform.position + Vector2.right * 30 : (Vector2) transform.position + Vector2.left * 30;
			}
			else
				endPoint = !player.Controller.AimAxis.isActive ? move.GetMoveAimPoint() : aim.GetAimPoint();

		}
	}
}
