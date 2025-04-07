using System;
using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using UnityEngine;

namespace __SCRIPTS
{
	public class NadeAttack : MonoBehaviour, INeedPlayer
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
		
		public static string AimVerbName = "aiming";
		public static string VerbName = "nading";
		private string AnimationName = "Top-Throw-Nade";
		private bool _isAiming;

		public event Action<Vector2, Vector2, float, Player> OnThrow;
		public event Action OnShowAiming;
		public event Action OnHideAiming;
		public event Action<Vector2, Vector2> OnAimAt;
		public event Action<Vector2, Vector2> OnAimInDirection;

		public void SetPlayer(Player _player)
		{
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			player = _player;
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
			arms.StopSafely(VerbName);
			Debug.Log("nade stop");
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
		}

		private void Player_NadePress(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			if (!ammo.secondaryAmmo.hasReserveAmmo())
			{
				Debug.Log("no nades");
				return;
			}

			if (!arms.Do(AimVerbName))
			{
				Debug.Log("can't nade " + arms.currentActivity);
				return;
			}

			Debug.Log("nade aiming start");
			IsAiming = true;
			OnShowAiming?.Invoke();
		}

		private void Player_NadeRelease(NewControlButton newControlButton)
		{
			if (PauseManager.I.IsPaused) return;
			OnHideAiming?.Invoke();
			IsAiming = false;
			if (!ammo.secondaryAmmo.hasReserveAmmo())
			{
				arms.StopSafely(AimVerbName);
				Debug.Log("no nades");
				return;
			}

			if (arms.currentActivity == AimVerbName)
			{
				arms.StopSafely(AimVerbName);
				arms.Do(VerbName);
				Debug.Log("nade release");
				anim.Play(AnimationName, 1, 0);
				
			}
			else
			{
				arms.StopSafely(AimVerbName);
			}
		
		}


		private void Anim_Throw()
		{
			ammo.secondaryAmmo.UseAmmo(1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime,
				(endPoint.y - startPoint.y) / throwTime);
			OnThrow?.Invoke(startPoint, velocity, throwTime, life.player);
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