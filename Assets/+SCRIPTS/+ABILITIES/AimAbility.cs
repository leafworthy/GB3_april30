using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public interface IAimAbility
	{
		Vector2 GetAimPoint();
		Vector3 AimDir { get; set; }
		bool IsAiming();
		RaycastHit2D CheckRaycastHit();
	}

	public class AimAbility : DoableActivity, IAimAbility
	{
		protected Player player;
		protected Vector2 aimDir;
		public Vector3 AimDir
		{
			get => aimDir;
			set => aimDir = value;
		}
		private float minMagnitude = .2f;
		private float maxAimDistance = 30;

		private IMove controlMoveAbility;
		private Vector3 aimDir1;
		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => false;

		public override void StartActivity()
		{
		}
		public virtual void SetPlayer(Player _player)
		{
			controlMoveAbility = GetComponent<IMove>();
			if (life == null) return;

			player = _player;

			if (player == null) return;
			aimDir = Vector2.right;
			if (player.isUsingMouse) return;
			if (!player.IsPlayer()) return;
			player.Controller.OnAimAxis_Change += AimerOnAim;
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (player.isUsingMouse) return;
			if (!player.IsPlayer()) return;
			player.Controller.OnAimAxis_Change -= AimerOnAim;
		}

		private void AimerOnAim(Vector2 newAimDir)
		{
			if (pauseManager.IsPaused) return;
			if (player.isDead()) return;
			if (IsAiming()) aimDir = newAimDir;
		}

		protected virtual void Update()
		{
			if (pauseManager.IsPaused) return;
			if (!player.IsPlayer()) return;
			if (player.isDead()) return;
			if (player.isUsingMouse) aimDir = GetRealAimDir();
			RotateAimObjects(aimDir);
			FaceAimDir();
		}

		private void FaceAimDir()
		{
			if (controlMoveAbility.IsMoving()) return;
			if (body.doableLegs.isActive) return;
			body.BottomFaceDirection(aimDir.x >= 0);
		}

		public RaycastHit2D CheckRaycastHit()
		{
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, AimDir.normalized, life.PrimaryAttackRange,
				assetManager.LevelAssets.BuildingLayer);

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
			if (player == null || body == null || body.AimCenter == null) return Vector2.zero;
			if (!player.isUsingMouse)
				return (Vector2) body.AimCenter.transform.position + (Vector2) GetRealAimDir() * maxAimDistance;
			return body.AimCenter.transform.position + (CursorManager.GetMousePosition() - body.AimCenter.transform.position).normalized * maxAimDistance;
		}

		protected virtual Vector3 GetRealAimDir()
		{
			if (player.isUsingMouse)
				return CursorManager.GetMousePosition() - body.AimCenter.transform.position;

			return player.Controller.GetAimAxisAngle();
		}

		public bool IsAiming()
		{
			if (player == null) return false;
			if (player.isUsingMouse) return true;
			return GetRealAimDir().magnitude > minMagnitude;
		}


	}
}
