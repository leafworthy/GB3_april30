using System;
using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{


	[DisallowMultipleComponent, RequireComponent(typeof(DoableNadeAttackAbility))]
	public class DoableNadeAttackAbility : ServiceAbility
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;

		private GunAimAbility aim;
		private AmmoInventory ammo;
		private Player player;

		private bool isAiming;
		private float currentCooldownTime;
		public override string VerbName => "Nade-Attack";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canDo() => base.canDo() && ammo.secondaryAmmo.hasReserveAmmo();

		public override bool canStop() => false;

		public override void StartActivity()
		{
			OnShowAiming?.Invoke();
			isAiming = true;

		}

		private string AnimationName = "ThrowNade_Top";
		private AnimationClip animationClip;

		public event Action<Vector2, Vector2, float, Player> OnThrow;
		public event Action OnShowAiming;
		public event Action OnHideAiming;
		public event Action<Vector2, Vector2> OnAimAt;
		public event Action<Vector2, Vector2> OnAimInDirection;

		private void Update()
		{
			if (isAiming)
			{
				if (player.isUsingMouse) AimAt(CursorManager.GetMousePosition());
				else Aim();
				OnShowAiming?.Invoke();
			}
			else
				OnHideAiming?.Invoke();

		}

		public void Aim()
		{
			isAiming = true;
			if (body == null) return;

			startPoint = body.AimCenter.transform.position;
			endPoint = !player.Controller.AimAxis.isActive ? move.GetMoveAimPoint() : aim.GetAimPoint();

			OnAimInDirection?.Invoke(startPoint, endPoint);
		}

		private void AimAt(Vector2 aimPos)
		{
			startPoint = body.AimCenter.transform.position;
			endPoint = aimPos;
			OnAimAt?.Invoke(startPoint, endPoint);
		}

		public void Throw()
		{
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime, (endPoint.y - startPoint.y) / throwTime);
			OnThrow?.Invoke(startPoint, velocity, throwTime, life.player);
		}

		public void ReleaseNade()
		{
			Debug.Log("[Grenade] Player released grenade button");
			isAiming = false;
			PlayAnimationClip(AnimationName, 1);
			OnHideAiming?.Invoke();
		}
	}
}
