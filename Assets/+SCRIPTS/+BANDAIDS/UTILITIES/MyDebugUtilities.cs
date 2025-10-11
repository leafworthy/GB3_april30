using UnityEngine;

namespace __SCRIPTS
{
	public static class MyDebugUtilities
	{
		public static void DrawX(Vector2 pos, float size, Color color, float Duration = 1)
		{
			var topLeft = new Vector2(pos.x - size, pos.y - size);
			var topRight = new Vector2(pos.x + size, pos.y - size);
			var bottomLeft = new Vector2(pos.x - size, pos.y + size);
			var bottomRight = new Vector2(pos.x + size, pos.y + size);
			Debug.DrawLine(topLeft, bottomRight, color, Duration);
			Debug.DrawLine(topRight, bottomLeft, color, Duration);
		}

		public static void DrawAttack(Attack attack, Color lineColor)
		{
			DrawX(attack.OriginWithHeight, 2, Color.red);
			DrawX(attack.OriginFloorPoint, 2, Color.yellow);
			DrawX(attack.DestinationFloorPoint, 2, Color.green);
			DrawX(attack.DestinationWithHeight, 2, Color.blue);
			Debug.DrawLine(attack.OriginWithHeight, attack.DestinationWithHeight, lineColor);
		}

		public static void DrawCircle(Vector3 transformPosition, float radius, Color color)
		{
			Gizmos.color = color;
			var segments = 15;
			Vector3 center = transformPosition;
			Vector3 prev = center + new Vector3(Mathf.Cos(0), Mathf.Sin(0)) * radius;

			for (int i = 1; i <= segments; i++)
			{
				float angle = i * Mathf.PI * 2f / segments;
				Vector3 next = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
				Gizmos.DrawLine(prev, next);
				prev = next;
			}
		}
	}
}
