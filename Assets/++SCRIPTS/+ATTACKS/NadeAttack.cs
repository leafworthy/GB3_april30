using System;
using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class NadeAttack : MonoBehaviour
	{
		private AnimationEvents animationEvents;
		private Vector2 startPoint;
		private Vector2 endPoint;

		private const int throwTime = 30;
		private const float throwDistanceMultiplier = 60;

		private GunAimAbility aim;
		private MoveAbility move;
		private AmmoInventory ammo;
		private Arms arms=>body.arms;
		private Player player;
		private Life life;
		private Body body;
		private Animations anim;

		private bool IsAiming;
		private float currentCooldownTime;
		private float cooldownRate=> life.PrimaryAttackRate;

		private string VerbName = "nading";
		private string AimVerbName = "aiming";
		private string AnimationName = "Top-Throw-Nade";
		private bool _isAiming;

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
			move = GetComponent<MoveAbility>();

			ammo = GetComponent<AmmoInventory>();
			aim = GetComponent<GunAimAbility>();

			OnHideAiming?.Invoke();
			ListenToPlayer();
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (player.Controller == null) return;
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
				if(player.isUsingMouse) AimAt(CursorManager.GetMousePosition());
				else Aim(aim.AimDir);
				OnShowAiming?.Invoke();
			}
			else
			{
		
				OnHideAiming?.Invoke();
			}
		}

		private void Player_NadePress(NewControlButton newControlButton)
		{
			if (PauseManager.IsPaused) return;
			if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades))
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
			if (PauseManager.IsPaused) return;
			
			IsAiming = false;
			if (!ammo.HasReserveAmmo(AmmoInventory.AmmoType.nades))
			{
				Debug.Log("no nades");
				return;
			}


			anim.Play(AnimationName, 1, 0);
			OnHideAiming?.Invoke();
		}

	

		private void Anim_Throw()
		{
			ammo.UseAmmo(AmmoInventory.AmmoType.nades, 1);
			startPoint = body.AimCenter.transform.position;
			var velocity = new Vector3((endPoint.x - startPoint.x) / throwTime, (endPoint.y - startPoint.y) / throwTime);
			Debug.Log("nade throw");
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
			if (!player.Controller.AimAxis.isActive)
			{
				endPoint =  move.GetMoveAimPoint();
			}
			else
			{
				endPoint = aim.GetAimPoint();
			}

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