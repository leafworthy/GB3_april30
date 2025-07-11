using System;
using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class NadeAimActivity : IActivity
	{
		public string VerbName => "Nade-Aim";

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
	}
	public class NadeAttack : ServiceUser, INeedPlayer, IActivity
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;

		private GunAimAbility aim;
		private MoveAbility move;
		private AmmoInventory ammo;
		private Arms arms => body.arms;
		private Player player;
		private Life life;
		private Body body;
		private Animations anim;

		private bool IsAiming;
		private float currentCooldownTime;
		private float throwStartTime;
		private bool isThrowingGrenade;
		private const float THROW_TIMEOUT = 3f; // Safety timeout for throw animation

		private NadeAimActivity aimActivity = new NadeAimActivity();
		public  string VerbName => "Nade-Attack";
		private string AnimationName = "ThrowNade_Top";
		private bool _isAiming;

		public event Action<Vector2, Vector2, float, Player> OnThrow;
		public event Action OnShowAiming;
		public event Action OnHideAiming;
		public event Action<Vector2, Vector2> OnAimAt;
		public event Action<Vector2, Vector2> OnAimInDirection;

		public bool TryCompleteGracefully(GangstaBean.Core.CompletionReason reason, GangstaBean.Core.IActivity newActivity = null)
		{
			switch (reason)
			{
				case GangstaBean.Core.CompletionReason.AnimationInterrupt:
					if (isThrowingGrenade)
					{
						isThrowingGrenade = false;
						OnHideAiming?.Invoke();
						return true;
					}
					if (IsAiming)
					{
						IsAiming = false;
						OnHideAiming?.Invoke();
						return true;
					}
					break;
			}
			return false;
		}

		public void SetPlayer(Player _player)
		{
			anim = GetComponent<Animations>();
			animationEvents = anim.animEvents;
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			player = _player;
			move = GetComponent<MoveAbility>();

			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();
			Debug.Log("[Grenade] SetPlayer called, connecting input events");
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
			player.Controller.AimAxis.OnChange -= Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress -= Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease -= Player_NadeRelease;
		}

		private void ListenToPlayer()
		{
			if (player == null) return;
			animationEvents = anim.animEvents;
			animationEvents.OnThrow += Anim_Throw;
			animationEvents.OnThrowStop += Anim_ThrowStop;
			player.Controller.AimAxis.OnChange += Player_OnAim;
			player.Controller.Attack2LeftTrigger.OnPress += Player_NadePress;
			player.Controller.Attack2LeftTrigger.OnRelease += Player_NadeRelease;
		}

		private void Anim_ThrowStop()
		{

			isThrowingGrenade = false;
			arms.Stop(this);
		}


		private void Update()
		{
			if (IsAiming)
			{
				if (player.isUsingMouse) AimAt(CursorManager.GetMousePosition());
				else Aim();
				OnShowAiming?.Invoke();
			}
			else
				OnHideAiming?.Invoke();

			// Safety timeout for stuck throw animation
			if (isThrowingGrenade && Time.time - throwStartTime > THROW_TIMEOUT)
			{

				isThrowingGrenade = false;
				arms.Stop(this);
			}
		}

		private void Player_NadePress(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			Debug.Log("[Grenade] Player pressed grenade button");

			if (!ammo.secondaryAmmo.hasReserveAmmo())
			{
				Debug.Log("[Grenade] BLOCKED: No ammo for grenade");
				return;
			}

			if (!arms.Do(aimActivity))
			{
				Debug.Log("[Grenade] BLOCKED: Arms busy, cannot start aiming");
				return;
			}

			Debug.Log("[Grenade] SUCCESS: Started aiming");
			IsAiming = true;
			OnShowAiming?.Invoke();
		}

		private void Player_NadeRelease(NewControlButton newControlButton)
		{
			if (pauseManager.IsPaused) return;
			Debug.Log("[Grenade] Player released grenade button");
			OnHideAiming?.Invoke();
			IsAiming = false;

			if (!ammo.secondaryAmmo.hasReserveAmmo())
			{
				Debug.Log("[Grenade] BLOCKED: No ammo");
				arms.Stop(aimActivity);
				return;
			}

			if (arms.currentActivity == aimActivity)
			{
				Debug.Log("[Grenade] Stopping aim activity and starting throw");

				if (arms.DoWithCompletion(this, GangstaBean.Core.CompletionReason.NewActivity))
				{
					Debug.Log("[Grenade] SUCCESS: Starting throw animation");
					isThrowingGrenade = true;
					throwStartTime = Time.time;
					anim.Play(AnimationName, 1, 0);
				}
				else
				{
					Debug.Log("[Grenade] BLOCKED: Could not transition from aim to throw");
				}
			}
			else
			{
				Debug.Log($"[Grenade] BLOCKED: Expected aim activity but current is {arms.currentActivity?.VerbName}");
				arms.Stop(aimActivity);
			}
		}


		private void Anim_Throw()
		{

			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime,
				(endPoint.y - startPoint.y) / throwTime);


			if (OnThrow != null)
			{
				OnThrow.Invoke(startPoint, velocity, throwTime, life.player);

			}
			else
			{

			}
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

		private void AimAt(Vector2 aimPos)
		{
			startPoint = body.AimCenter.transform.position;
			endPoint = aimPos;
			OnAimAt?.Invoke(startPoint, endPoint);
		}


	}
}
