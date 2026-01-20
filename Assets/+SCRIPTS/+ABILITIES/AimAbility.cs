using __SCRIPTS.Cursor;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public interface IAimAbility
	{
		bool hasEnoughMagnitude();
		Vector2 AimDir { get; }
		Vector2 GetAimPoint();
	}

	[DisallowMultipleComponent]
	public class AimAbility : MonoBehaviour, IAimAbility, INeedPlayer
	{
		Life life => _life ??= GetComponent<Life>();
		Life _life;
		public Vector2 AimDir { get; private set; }
		const float aimSmoothSpeed = 10;
		const float maxAimDistance = 30;
		public bool hasEnoughMagnitude() => player.Controller.AimAxis.isActive;
		MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		MoveAbility _moveAbility;

		Player player;
		Body body => _body ??= GetComponent<Body>();
		Body _body;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			AimDir = Vector2.right;
		}

		void Update()
		{
			if (Services.pauseManager.IsPaused) return;
			if (life.IsDead()) return;
			Aim();
		}

		void Aim()
		{
			if (hasEnoughMagnitude()) AimDir = Vector2.Lerp(AimDir, GetRealAimDir().normalized, aimSmoothSpeed * Time.deltaTime);
			RotateAimObjects(AimDir);
			BottomFaceAimDir();
		}

		void BottomFaceAimDir()
		{
			if (moveAbility.IsMoving()) return;
			body.BottomFaceDirection(AimDir.x >= 0);
		}

		void RotateAimObjects(Vector2 direction)
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

		Vector3 GetRealAimDir()
		{
			if (player.isUsingMouse) return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
			return player.Controller.AimAxis.GetCurrentAngle();
		}
	}
}
