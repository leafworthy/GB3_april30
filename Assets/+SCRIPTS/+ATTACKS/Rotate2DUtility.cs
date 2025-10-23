using UnityEngine;

namespace __SCRIPTS
{
	public static class Rotate2DUtility
	{
		public static void RotateTowardDirection2D(Transform transform, Vector2 direction, float rotationSpeed = 0f, float angleOffset = 0f)
		{
			if (direction == Vector2.zero)
				return;

			var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
			var currentAngle = transform.eulerAngles.z;

			if (rotationSpeed > 0f)
			{
				var newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
				transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
			}
			else
				transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
		}
	}
}
