using System;
using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class HumanUnitController : MonoBehaviour, IUnitController
	{
		public Vector2 GetAimAxisAngle() {
			{
				if (owner.isUsingMouse)
					return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
				return AimAxis.GetCurrentAngle();
			}
		}

		public bool GetAimAxisInactive() => AimAxis.currentMagnitudeIsTooSmall();

		public Vector2 GetMoveAxisAngle() =>   MoveAxis.GetCurrentAngle();

		public bool GetMoveAxisInactive() => MoveAxis.currentMagnitudeIsTooSmall();

		public Vector2 GetUIAxisAngle() =>   UIAxis.GetCurrentAngle();

		public bool GetUIAxisInactive() =>   UIAxis.currentMagnitudeIsTooSmall();
		public event Action<Vector2> OnAimAxis_Change;
		public event Action OnAimAxis_Inactive;
		public event Action OnAimAxis_Active;
		public event Action<Vector2> OnMoveAxis_Change;

		public event Action<NewInputAxis> OnMoveAxis_Inactive;
		public event Action<NewInputAxis> OnMoveAxis_Active;
		public event Action<NewInputAxis> OnUIAxis_Up;
		public event Action<NewInputAxis> OnUIAxis_Down;
		public event Action<NewInputAxis> OnUIAxis_Left;
		public event Action<NewInputAxis> OnUIAxis_Right;

		public event Action<NewControlButton> OnSelect_Pressed;
		public event Action<NewControlButton> OnSelect_Released;
		public event Action<NewControlButton> OnPause_Pressed;
		public event Action<NewControlButton> OnPause_Released;
		public event Action<NewControlButton> OnUnpause_Pressed;
		public event Action<NewControlButton> OnUnpause_Released;
		public event Action<NewControlButton> OnCancel_Pressed;
		public event Action<NewControlButton> OnCancel_Released;
		public event Action<NewControlButton> OnJump_Pressed;
		public event Action<NewControlButton> OnJump_Released;
		public event Action<NewControlButton> OnAttack1_Pressed;
		public event Action<NewControlButton> OnAttack1_Released;
		public event Action<NewControlButton> OnAttack2_Pressed;
		public event Action<NewControlButton> OnAttack2_Released;
		public event Action<NewControlButton> OnAttack3_Pressed;
		public event Action<NewControlButton> OnAttack3_Released;
		public event Action<NewControlButton> OnDash_Pressed;
		public event Action<NewControlButton> OnDash_Released;
		public event Action<NewControlButton> OnReload_Pressed;
		public event Action<NewControlButton> OnReload_Released;
		public event Action<NewControlButton> OnInteract_Pressed;
		public event Action<NewControlButton> OnInteract_Released;
		public event Action<NewControlButton> OnSwapWeapon_Pressed;
		public event Action<NewControlButton> OnSwapWeapon_Released;

		private Player owner;
		private PlayerControls controls;
		private NewInputAxis AimAxis;
		private NewInputAxis MoveAxis;
		private NewInputAxis UIAxis;
		private NewControlButton Select;
		private NewControlButton Cancel;
		private NewControlButton Jump;
		private NewControlButton DashRightShoulder;
		private NewControlButton Attack1RightTrigger;
		private NewControlButton Attack2LeftTrigger;
		private NewControlButton Attack3Circle;
		private NewControlButton ReloadTriangle;
		private NewControlButton SwapWeaponSquare;
		private NewControlButton InteractRightShoulder;
		private NewControlButton Pause;
		private NewControlButton Unpause;

		private bool initialized;
		private Body body => _body ??= GetComponent<Body>();
		private Body _body;

		private void Start()
		{
			SetAxes();
		}

		public void InitializeAndLinkToPlayer(Player player)
		{
			if (initialized) return;
			initialized = true;
			if (player == null) return;

			owner = player;
			controls = new PlayerControls();

			SetAxes();

			MoveAxis.OnChange += (input, angle) => OnMoveAxis_Change?.Invoke(angle);
			MoveAxis.OnInactive += axis => OnMoveAxis_Inactive?.Invoke(axis);
			MoveAxis.OnActive += axis => OnMoveAxis_Active?.Invoke(axis);

			UIAxis.OnUp += axis => OnUIAxis_Up?.Invoke(axis);
			UIAxis.OnDown += axis => OnUIAxis_Down?.Invoke(axis);
			UIAxis.OnLeft += axis => OnUIAxis_Left?.Invoke(axis);
			UIAxis.OnRight += axis => OnUIAxis_Right?.Invoke(axis);


			AimAxis.OnChange += (input,angle) => OnAimAxis_Change?.Invoke(angle);
			AimAxis.OnInactive += (x) => OnAimAxis_Inactive?.Invoke();
			AimAxis.OnActive += (x) => OnAimAxis_Active?.Invoke();


			Select = new NewInputButton(controls.UI.Select, owner);
			Select.OnPress += button => OnSelect_Pressed?.Invoke(button);
			Select.OnRelease += button => OnSelect_Released?.Invoke(button);

			Cancel = new NewInputButton(controls.UI.Cancel, owner);
			Cancel.OnPress += button => OnCancel_Pressed?.Invoke(button);
			Cancel.OnRelease += button => OnCancel_Released?.Invoke(button);

			Unpause = new NewInputButton(controls.UI.Unpause, owner);
			Unpause.OnPress += button => OnUnpause_Pressed?.Invoke(button);
			Unpause.OnRelease += button => OnUnpause_Released?.Invoke(button);

			Pause = new NewInputButton(controls.PlayerMovement.Pause, owner);
			Pause.OnPress += button => OnPause_Pressed?.Invoke(button);
			Pause.OnRelease += button => OnPause_Released?.Invoke(button);

			Jump = new NewInputButton(controls.PlayerMovement.Jump, owner);
			Jump.OnPress += button => OnJump_Pressed?.Invoke(button);
			Jump.OnRelease += button => OnJump_Released?.Invoke(button);

			DashRightShoulder = new NewInputButton(controls.PlayerMovement.DashLeftShoulder, owner);
			DashRightShoulder.OnPress += button => OnDash_Pressed?.Invoke(button);
			DashRightShoulder.OnRelease += button => OnDash_Released?.Invoke(button);

			Attack1RightTrigger = new NewInputButton(controls.PlayerMovement.Attack1RightTrigger, owner);
			Attack1RightTrigger.OnPress += button => OnAttack1_Pressed?.Invoke(button);
			Attack1RightTrigger.OnRelease += button => OnAttack1_Released?.Invoke(button);

			Attack2LeftTrigger = new NewInputButton(controls.PlayerMovement.Attack2LeftTrigger, owner);
			Attack2LeftTrigger.OnPress += button => OnAttack2_Pressed?.Invoke(button);
			Attack2LeftTrigger.OnRelease += button => OnAttack2_Released?.Invoke(button);

			Attack3Circle = new NewInputButton(controls.PlayerMovement.Attack3Circle, owner);
			Attack3Circle.OnPress += button => OnAttack3_Pressed?.Invoke(button);
			Attack3Circle.OnRelease += button => OnAttack3_Released?.Invoke(button);

			ReloadTriangle = new NewInputButton(controls.PlayerMovement.ReloadTriangle, owner);
			ReloadTriangle.OnPress += button => OnReload_Pressed?.Invoke(button);
			ReloadTriangle.OnRelease += button => OnReload_Released?.Invoke(button);

			InteractRightShoulder = new NewInputButton(controls.PlayerMovement.InteractRightShoulder, owner);
			InteractRightShoulder.OnPress += button => OnInteract_Pressed?.Invoke(button);
			InteractRightShoulder.OnRelease += button => OnInteract_Released?.Invoke(button);

			SwapWeaponSquare = new NewInputButton(controls.PlayerMovement.SwapWeaponSquare, owner);
			SwapWeaponSquare.OnPress += button => OnSwapWeapon_Pressed?.Invoke(button);
			SwapWeaponSquare.OnRelease += button => OnSwapWeapon_Released?.Invoke(button);
		}

		public Vector2 GetAimPoint(float maxAimDistance) {
			if (owner == null || body == null || body.AimCenter == null) return Vector2.zero;

			if (!owner.isUsingMouse)
				return (Vector2) body.AimCenter.transform.position + (Vector2) owner.Controller.GetAimAxisAngle() * maxAimDistance;
			return body.AimCenter.transform.position + (CursorManager.GetMousePosition() - body.AimCenter.transform.position).normalized * maxAimDistance;

		}

		private void FixedUpdate()
		{
			if (!initialized) return;
			MoveAxis?.update();
		}

		private void SetAxes()
		{
			if (owner == null || controls == null) return;

			AimAxis = owner.isUsingMouse
				? new NewInputAxis(controls.PlayerMovement.MousePosition, owner)
				: new NewInputAxis(controls.PlayerMovement.StickAiming, owner);

			UIAxis = new NewInputAxis(controls.UI.Movement, owner);
			MoveAxis = new NewInputAxis(controls.PlayerMovement.Movement, owner);
		}


	}
}
