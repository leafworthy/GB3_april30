using UnityEngine;

namespace __SCRIPTS
{
	public static class MyDebugUtilities
	{
		public static void DrawX(Vector2 pos, float size, Color color)
		{
			var topLeft = new Vector2(pos.x - size, pos.y - size);
			var topRight = new Vector2(pos.x + size, pos.y - size);
			var bottomLeft = new Vector2(pos.x - size, pos.y + size);
			var bottomRight = new Vector2(pos.x + size, pos.y + size);
			Debug.DrawLine(topLeft, bottomRight, color);
			Debug.DrawLine(topRight, bottomLeft, color);
		}

		public static void DrawAttack(Attack attack)
		{
			DrawX(attack.OriginWithHeight, 2, Color.red);
			DrawX(attack.OriginFloorPoint, 2, Color.yellow);
			DrawX(attack.DestinationFloorPoint, 2, Color.green);
			DrawX(attack.DestinationWithHeight, 2, Color.blue);
		}
	}
}