using System;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class NadeAttack : ServiceAbility
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;

		private GunAimAbility aim;
		private MoveAbility move;
		private AmmoInventory ammo;

		public override string VerbName => NadeVerbName;
		private DoableJumpAbility jump => _jump ??= GetComponent<DoableJumpAbility>();
		private DoableJumpAbility _jump;
		private DoableArms arms => body.doableArms;

		public event Action<Vector2, Vector2, float, Player> OnThrow;
		public event Action OnShowAiming;
		public event Action OnHideAiming;
		public event Action<Vector2, Vector2> OnAimAt;
		public event Action<Vector2, Vector2> OnAimInDirection;
		public bool IsAiming;
		private float currentCooldownTime;
		[SerializeField] private AnimationClip animationClip;

		public static string NadeVerbName = "Nade";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		public override bool canStop() => IsAiming;
		public override bool canDo() => ammo.secondaryAmmo.hasReserveAmmo() && !jump.IsJumping && base.canDo();

		protected override void DoAbility()
		{
			Debug.Log("throw grenade");
			ThrowGrenade();
		}

		private void ShowAiming()
		{
			Debug.Log("show aiming");
			IsAiming = true;
			OnShowAiming?.Invoke();
		}

		public override void Stop()
		{
			base.Stop();
			Debug.Log("aiming stop");
			HideAiming();
		}



		public override void SetPlayer(Player _player)
		{
			Debug.Log("setting player for nade attack");
			base.SetPlayer(_player);
			animationEvents = anim.animEvents;
			move = GetComponent<MoveAbility>();
			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();
			StopListeningToPlayer();
			ListenToPlayer();
		}

		private void OnDisable()
		{
			Debug.Log("stop listen");
			StopListeningToPlayer();
		}

		private void OnDestroy()
		{
			Debug.Log("stop listen");
			StopListeningToPlayer();
		}

		private void StopListeningToPlayer()
		{
			Debug.Log("stop listen");
			if (anim == null) return;
			animationEvents.OnThrowStop -= Anim_ThrowStop;
			if (player == null) return;
			if (player.Controller == null) return;
			player.Controller.AimAxis.OnChange -= Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_NadeRelease;
		}

		private void ListenToPlayer()
		{
			Debug.Log("listen to player");
			if (player == null)
			{
				Debug.Log("No player assigned to nade attack");
				return;
			}
			animationEvents = anim.animEvents;
			animationEvents.OnThrowStop += Anim_ThrowStop;
			player.Controller.AimAxis.OnChange += Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress += Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_NadeRelease;
		}

		private void Anim_ThrowStop()
		{
			arms.Stop(this);
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
			Debug.Log("nade press");
			if (!IsAiming && canDo())
			{
				ShowAiming();
			}
		}

		protected override void AnimationComplete()
		{
			base.AnimationComplete();
		}

		private void Player_NadeRelease(NewControlButton newControlButton)
		{
			Debug.Log("nade release");
			if (!canDo())
			{
				Debug.Log("can't do nade");
				Stop();
			}

			Do();
		}

		private void ThrowGrenade()
		{
			PlayAnimationClip(animationClip, 1);
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime, (endPoint.y - startPoint.y) / throwTime);
			OnThrow?.Invoke(startPoint, velocity, throwTime, life.player);
			HideAiming();
		}

		private void HideAiming()
		{
			Debug.Log("hide aiming");
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
