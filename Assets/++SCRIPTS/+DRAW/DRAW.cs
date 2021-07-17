//draw

using UnityEngine;

public class DRAW : Singleton<DRAW>
{

	public static Vector2 convertWorldToLinePoint(Vector2 v)
	{
		if (Camera.main == null) return Vector2.zero;
		v = convertScreenToLinePoint(Camera.main.WorldToScreenPoint(v));
		return v;
	}

	private static Vector2 convertScreenToLinePoint(Vector2 v)
	{
		v.y = Screen.height - v.y;
		return v;
	}

	public static void line(Vector2 start, Vector2 end, Color color, float thickness, Texture2D texture2D)
	{
		if (PAUSE.isPaused) return;
		var _coloredLineColor = color;
		var _coloredLineTexture = texture2D;
		_coloredLineTexture.SetPixel(0, 0, _coloredLineColor);
		_coloredLineTexture.wrapMode = TextureWrapMode.Clamp;
		_coloredLineTexture.Apply();

		var lineVector = end - start;
		var angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
		if (lineVector.x < 0) angle += 180;

		if (thickness < 1) thickness = 1;

		var thicknessOffset = (int) Mathf.Ceil(thickness / 2);

		GUIUtility.RotateAroundPivot(angle, start);
		GUI.DrawTexture(
			new Rect(start.x,
				start.y - thicknessOffset,
				lineVector.magnitude,
				thickness),
			_coloredLineTexture);

		GUIUtility.RotateAroundPivot(-angle, start);
	}
}
