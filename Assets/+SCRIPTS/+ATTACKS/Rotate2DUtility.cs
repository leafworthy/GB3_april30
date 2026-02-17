using UnityEngine;

namespace __SCRIPTS
{
	public static class Rotate2DUtility
	{
		public static void RotateTowardDirection2D(Transform transform, Vector2 direction, float angleOffset = 0f)
		{
			if (direction.sqrMagnitude < 0.001f)
				return;

			direction.Normalize();

			float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
			Debug.Log(Quaternion.Euler(0f, 0f, targetAngle) + " is the target rotation for direction " + direction, transform);
			transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
		}
	}
}
