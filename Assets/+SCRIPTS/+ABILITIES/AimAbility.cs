using __SCRIPTS.Cursor;
using __SCRIPTS.HUD_Displays;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class AimAbility : ServiceUser, INeedPlayer
	{
		private Life life;
		protected Player owner;
		protected Body body;
		[HideInInspector] public Vector2 AimDir;
		public const float aimDistanceFactor = 100;
		private float minMagnitude = .2f;
		private bool hasInitiated;
		private float maxAimDistance = 30;


		private MoveAbility moveAbility;

		public virtual void SetPlayer(Player _player)
		{
			moveAbility = GetComponent<MoveAbility>();
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			if (life == null) return;

			owner = _player;

			if (owner == null) return;
			AimDir = Vector2.right;
			if (owner.isUsingMouse) return;
			if (!owner.IsPlayer()) return;
			owner.Controller.AimAxis.OnChange += AimerOnAim;
		}

		private void OnDisable()
		{
			if (owner == null) return;
			if (owner.isUsingMouse) return;
			if (!owner.IsPlayer()) return;
			owner.Controller.AimAxis.OnChange -= AimerOnAim;
		}


		private void AimerOnAim(IControlAxis controlAxis, Vector2 newAimDir)
		{
			if (pauseManager.IsPaused) return;
			if (owner.isDead()) return;
			if (hasEnoughMagnitude()) AimDir = newAimDir;
		}

		protected virtual void Update()
		{
			if (pauseManager.IsPaused) return;
			if (!owner.IsPlayer()) return;
			if (owner.isDead()) return;
			if (owner.isUsingMouse) AimDir = GetRealAimDir();
			RotateAimObjects(AimDir);
			FaceAimDir();
		}

		private void FaceAimDir()
		{
			if (moveAbility.IsMoving()) return;
			if (body.legs.isActive) return;
			body.BottomFaceDirection(AimDir.x >= 0);
		}

		public RaycastHit2D CheckRaycastHit(Vector3 targetDirection)
		{
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized,
				life.PrimaryAttackRange,
				 AssetManager.LevelAssets.BuildingLayer);

			return raycastHit;
		}

		private void RotateAimObjects(Vector2 direction)
		{
			var rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			foreach (var obj in body.RotateWithAim)
			{
				if (obj == null) continue;
				obj.transform.eulerAngles = new Vector3(0, 0, rotation);
			}
		}

		public Vector2 GetAimPoint()
		{
			if(owner == null || body == null || body.AimCenter == null) return Vector2.zero;
			if (!owner.isUsingMouse)
				return (Vector2)body.AimCenter.transform.position + (Vector2)GetRealAimDir() * maxAimDistance;
			else
				return body.AimCenter.transform.position + (CursorManager.GetMousePosition() - body.AimCenter.transform.position).normalized * maxAimDistance;
		}



		protected virtual Vector3 GetRealAimDir()
		{
			if (owner == null) return Vector2.zero;

			if (owner.isUsingMouse)
				return CursorManager.GetMousePosition() - body.AimCenter.transform.position;

			return owner.Controller.AimAxis.GetCurrentAngle();
		}

		public bool hasEnoughMagnitude()
		{
			if(owner == null) return false;
			if (owner.isUsingMouse) return true;
			return GetRealAimDir().magnitude > minMagnitude;
		}
	}
}
