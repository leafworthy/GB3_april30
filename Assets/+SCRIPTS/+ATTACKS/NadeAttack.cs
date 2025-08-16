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
		private UnitAnimations anim;

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


		public void SetPlayer(Player _player)
		{
			StopListeningToPlayer();
			anim = GetComponent<UnitAnimations>();
			animationEvents = anim.animEvents;
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			this.player = _player;
			move = GetComponent<MoveAbility>();

			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();
			ListenToPlayer();
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		private void OnDestroy()
		{
			StopListeningToPlayer();
		}

		private void StopListeningToPlayer()
		{
			if (anim == null) return;
			animationEvents.OnThrow -= Anim_Throw;
			animationEvents.OnThrowStop -= Anim_ThrowStop;
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

			if (!ammo.secondaryAmmo.hasReserveAmmo())
			{
				return;
			}

			if (!arms.Do(aimActivity))
			{
				Debug.Log("[Grenade] BLOCKED: Arms busy with: " +   (arms.currentActivity?.VerbName ?? "nothing"));
				return;
			}

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
				arms.Stop(aimActivity);
				return;
			}

			if (arms.currentActivity == aimActivity)
			{

				if (arms.DoWithCompletion(this, GangstaBean.Core.CompletionReason.NewActivity))
				{
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
