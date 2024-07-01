using System;
using __SCRIPTS._ABILITIES;
using __SCRIPTS._CAMERA;
using __SCRIPTS._COMMON;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS._ATTACKS
{
	public class NadeAbility : MonoBehaviour
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;
		private const float throwDistanceMultiplier = 60;

		private GunAimAbility aim;
		private AmmoInventory ammo;
		private Arms arms=>body.arms;
		private Player player;
		private Life life;
		private Body body;
		private Animations anim;

		private bool IsAiming;
		private float currentCooldownTime;
		private float cooldownRate=> life.AttackRate;

		private string VerbName = "nading";
		private string AimVerbName = "aiming";
		private string AnimationName = "Top-Throw-Nade";

		public event Action<Vector2, Vector2, float, Player> OnThrow;
		public event Action OnShowAiming;
		public event Action OnHideAiming;
		public event Action<Vector2, Vector2> OnAimAt;
		public event Action<Vector2, Vector2> OnAimInDirection;

		private void Start()
		{
			anim = GetComponent<Animations>();
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			player = life.player;
			 
			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();
			ListenToPlayer();
			OnHideAiming?.Invoke();
			
		}


		private void OnDisable()
		{
			if (player == null) return;
			player.Controller.AimAxis.OnChange -= Player_OnAim;
			player.Controller.Attack2.OnPress -= Player_NadePress;
			player.Controller.Attack2.OnRelease -= Player_NadeRelease;
			if (animationEvents == null) return;
			animationEvents.OnThrow -= Anim_Throw;
			animationEvents.OnThrowStop -= Anim_ThrowStop;
		}

		private void ListenToPlayer()
		{
			if (player == null) return;
			animationEvents = anim.animEvents;
			animationEvents.OnThrow += Anim_Throw;
			animationEvents.OnThrowStop += Anim_ThrowStop;

			
			player.Controller.AimAxis.OnChange += Player_OnAim;
			player.Controller.Attack2.OnPress += Player_NadePress;
			player.Controller.Attack2.OnRelease += Player_NadeRelease;
		}

		private void Anim_ThrowStop()
		{
			arms.Stop("NadeThrowing");
			anim.SetBool(Animations.IsShooting, false);
		}


		private void Update()
		{
			if (IsAiming)
				OnShowAiming?.Invoke();
			else
				OnHideAiming?.Invoke();

			switch (IsAiming)
			{
				case true when player.isUsingMouse:
					AimAt(CursorManager.GetMousePosition());

					return;
				case true:

					Aim(aim.AimDir);
					break;
			}
		}

		private void Player_NadePress(NewControlButton newControlButton)
		{
			if (GlobalManager.IsPaused) return;

			if (!arms.Do(AimVerbName)) return;

			if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades)) return;
			IsAiming = true;
			OnShowAiming?.Invoke();
		}

		private void Player_NadeRelease(NewControlButton newControlButton)
		{
			if (GlobalManager.IsPaused) return;
			if (!IsAiming) return;

			arms.Stop(AimVerbName);
			IsAiming = false;
			NadeWithCooldown(aim.AimDir);
			OnHideAiming?.Invoke();
			anim.SetBool(Animations.IsShooting, false);
		}

		private void NadeWithCooldown(Vector3 target)
		{
			if (!(Time.time >= currentCooldownTime)) return;

			if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades)) return;

			if (!arms.Do(VerbName)) return;
			ammo.UseAmmo(AmmoInventory.AmmoType.nades, 1);
			anim.Play(AnimationName, 1, 0);
			currentCooldownTime = Time.time + cooldownRate;
		}

		private void Anim_Throw()
		{
			endPoint = aim.GetAimPoint();
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime, (endPoint.y - startPoint.y) / throwTime);

			OnThrow?.Invoke(startPoint, velocity, throwTime, life.player);
		}

		private void Player_OnAim(IControlAxis controlAxis, Vector2 aimDir)
		{
			Aim(controlAxis.GetCurrentAngle());
		}

		private void Aim(Vector2 aimDir)
		{
			if (body == null) return;
			startPoint = body.AimCenter.transform.position;
			endPoint = (Vector2) body.AimCenter.transform.position + aimDir * throwDistanceMultiplier;

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