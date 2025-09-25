using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class NadeAttack : Ability
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;

		private IAimAbility aim => _aim ??= GetComponent<IAimAbility>();
		private IAimAbility _aim;
		private MoveAbility move;
		private AmmoInventory ammo;

		public override string AbilityName => "Nade";
		private JumpAbility jump => _jump ??= GetComponent<JumpAbility>();
		private JumpAbility _jump;
		private DoableArms arms => body.doableArms;

		public event Action<Vector2, Vector2, float, Life> OnThrow;
		public event Action OnShowAiming;
		public event Action OnHideAiming;
		public event Action<Vector2, Vector2> OnAimAt;
		public event Action<Vector2, Vector2> OnAimInDirection;
		public bool IsAiming;
		private float currentCooldownTime;
		[SerializeField] private AnimationClip animationClip;

		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canStop(IDoableAbility abilityToStopFor) => IsAiming;
		public override bool canDo() => ammo.secondaryAmmo.hasReserveAmmo() && !jump.IsJumping && base.canDo();

		protected override void DoAbility()
		{
			ThrowGrenade();
		}

		private void ShowAiming()
		{
			IsAiming = true;
			OnShowAiming?.Invoke();
		}

		public override void Stop()
		{
			base.Stop();
			HideAiming();
			lastArmAbility.Resume();
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			animationEvents = anim.animEvents;
			move = GetComponent<MoveAbility>();
			ammo = GetComponent<AmmoInventory>();
			StopListeningToPlayer();
			ListenToPlayer();
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void StopListeningToPlayer()
		{
			if (anim == null) return;
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.AimAxis.OnChange -= Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_NadeRelease;
		}

		private void ListenToPlayer()
		{
			if (player == null) return;
			animationEvents = anim.animEvents;
			player.Controller.AimAxis.OnChange += Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress += Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_NadeRelease;
		}

		private void Update()
		{
			if (IsAiming)
			{
				Aim();
				OnShowAiming?.Invoke();
			}
			else
				OnHideAiming?.Invoke();
		}

		private void Player_NadePress(NewControlButton newControlButton)
		{
			if (!IsAiming && canDo()) ShowAiming();
		}


		private void Player_NadeRelease(NewControlButton newControlButton)
		{
			Do();
		}

		private void ThrowGrenade()
		{
			PlayAnimationClip(animationClip, 1);
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime, (endPoint.y - startPoint.y) / throwTime);
			OnThrow?.Invoke(startPoint, velocity, throwTime, life);
			HideAiming();
		}

		private void HideAiming()
		{
			IsAiming = false;
			OnHideAiming?.Invoke();
		}

		private void Player_OnAim(IControlAxis controlAxis, Vector2 aimDir)
		{
			if (!IsAiming) return;
			Aim();
		}

		private void Aim()
		{
			if (body == null) return;

			startPoint = body.AimCenter.transform.position;
			endPoint = !player.Controller.AimAxis.isActive ? move.GetMoveAimPoint() : aim.GetAimPoint();

			OnAimInDirection?.Invoke(startPoint, endPoint);
		}
	}
}
