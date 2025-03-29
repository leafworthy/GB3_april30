using __SCRIPTS.Cursor;
using UnityEngine;

namespace __SCRIPTS
{
	public class AimAbility : MonoBehaviour
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

		public virtual void Start()
		{
			Init();
		}

		private void Init()
		{
			if (hasInitiated) return;
			hasInitiated = true;
			moveAbility = GetComponent<MoveAbility>();
			body = GetComponent<Body>();
			life = GetComponent<Life>();
			if (life == null) return;
			owner = life.player;
			if (owner == null) return;
			if (owner.isUsingMouse) return;
			if (!owner.IsPlayer()) return;
			owner.Controller.AimAxis.OnChange += AimerOnAim;
			AimDir = Vector2.right;
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
			if (PauseManager.IsPaused) return;
			if (owner.isDead()) return;
			if (hasEnoughMagnitude()) AimDir = newAimDir;
		}

		protected virtual void Update()
		{
			if (PauseManager.IsPaused) return;
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
				ASSETS.LevelAssets.BuildingLayer);

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
			Init();
			if (!owner.isUsingMouse)
				return (Vector2)body.AimCenter.transform.position + (Vector2)GetRealAimDir() * maxAimDistance;
			else
				return body.AimCenter.transform.position + (CursorManager.GetMousePosition() - body.AimCenter.transform.position).normalized * maxAimDistance;
		}



		protected virtual Vector3 GetRealAimDir()
		{
			Init();

			if (owner.isUsingMouse)
				return CursorManager.GetMousePosition() - body.AimCenter.transform.position;

			return owner.Controller.AimAxis.GetCurrentAngle();
		}

		public bool hasEnoughMagnitude()
		{
			if (owner.isUsingMouse) return true;
			return GetRealAimDir().magnitude > minMagnitude;
		}
	}
}