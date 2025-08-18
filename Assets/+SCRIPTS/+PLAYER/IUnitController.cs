using System;
using UnityEngine;

namespace __SCRIPTS
{
	public interface IUnitController
	{

		public Vector2 GetAimAxisAngle();
		public bool GetAimAxisInactive();
		public Vector2 GetMoveAxisAngle();
		public bool GetMoveAxisInactive();
		public Vector2 GetUIAxisAngle();
		public bool GetUIAxisInactive();

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
		void InitializeAndLinkToPlayer(Player player);
		Vector2 GetAimPoint(float maxAimDistance);
	}
}
