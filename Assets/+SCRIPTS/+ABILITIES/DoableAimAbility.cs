using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableAimAbility : ServiceAbility
	{
		[HideInInspector] public Vector2 AimDir = Vector2.right;
		private bool hasEnoughMagnitude() => player.Controller.AimAxis.isActive;
		private float maxAimDistance = 30;

		private MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		public override string VerbName => "Aim";

		public override bool canStop()
		{
			Debug.Log("testing can stop");
			return true;
		}

		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => false;

		public override bool canDo()
		{
			if (!base.canDo()) return false;

			if (body.doableArms.IsActive)
			{
				Debug.Log("BLOCKED: Cannot aim, arms are active" + (body.doableArms.CurrentAbility != null ? $"({body.doableArms.CurrentAbility.VerbName})" : ""));
				return false;
			}
			return true;
		}

		public override void Stop()
		{
			base.Stop();
			Debug.Log("trying to stop aim ability");
		}

		protected override void DoAbility()
		{
			AimDir = GetRealAimDir();
			RotateAimObjects(AimDir);
			FaceAimDir();
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			player.Controller.AimAxis.OnChange += AimerOnAim;
		}

		private void OnDisable()
		{
			if (player == null) return;
			if (player.isUsingMouse) return;
			if (!player.IsPlayer()) return;
			player.Controller.AimAxis.OnChange -= AimerOnAim;
		}

		private void AimerOnAim(IControlAxis controlAxis, Vector2 newAimDir)
		{
			if (canDo() && hasEnoughMagnitude()) AimDir = newAimDir;
			Do();
		}

		protected virtual void Update()
		{
			Do();
		}

		private void FaceAimDir()
		{
			if (moveAbility.IsMoving()) return;
			if (body.legs.isActive) return;
			body.BottomFaceDirection(AimDir.x >= 0);
		}

		public RaycastHit2D CheckRaycastHit(Vector3 targetDirection)
		{
			var raycastHit = Physics2D.Raycast(body.FootPoint.transform.position, targetDirection.normalized, life.PrimaryAttackRange,
				assets.LevelAssets.BuildingLayer);

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
			if (player == null) return Vector2.zero;

			if (player.isUsingMouse)
				return CursorManager.GetMousePosition() - body.AimCenter.transform.position;

			return player.Controller.AimAxis.GetCurrentAngle();
		}


	}
}
