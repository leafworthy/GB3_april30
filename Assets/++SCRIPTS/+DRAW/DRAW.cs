//draw

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _SCRIPTS;

public class DRAW: Singleton<DRAW>
{
	#region events and delegates

	#endregion

	#region private variables

	private static float gravity;

	#endregion

	#region public variables

	public float LineThickness = 3f;
	public  static Color LineColorA = Color.white;
	public  static Color LineColorB = Color.white;
	public static float lineThickness;
	public  static Color lineColor;
	public static Color lineColorA;
	public static Color lineColorB;

	public float radius = 1;
	public Texture2D setTexture;
	public  static Texture2D texture2D;

	#endregion

	#region init

	void Start ()
	{
		init ();
	}


	public static void X (Vector2 v, float size, Color color)
	{

		line (DRAW.convertWorldToLinePoint (new Vector2 (v.x - size, v.y + size)), DRAW.convertWorldToLinePoint (new Vector2 (v.x + size, v.y - size)), color, 2);
		line (DRAW.convertWorldToLinePoint (new Vector2 (v.x + size, v.y + size)), DRAW.convertWorldToLinePoint (new Vector2 (v.x - size, v.y - size)), color, 2);
	}

	#endregion

	#region update

	void Update ()
	{

		lineThickness = LineThickness;



	}


	public static Vector2 convertWorldToLinePoint (Vector2 v)
	{
		v = convertScreenToLinePoint (Camera.main.WorldToScreenPoint (v));
		return v;
	}

	public static Vector2 convertScreenToLinePoint (Vector2 v)
	{
		v.y = Screen.height - v.y;
		return v;
	}









	#endregion

	#region public functions









	public static void line (Vector2 start, Vector2 end, Color color, float thickness = 2, bool fade = false)
	{
		//START AND END IN SCREEN SPACE
		thickness = lineThickness;
		var _coloredLineColor = color;
		var _coloredLineTexture = texture2D;
		_coloredLineTexture.SetPixel (0, 0, _coloredLineColor);
		_coloredLineTexture.wrapMode = TextureWrapMode.Clamp;
		_coloredLineTexture.Apply ();

		Vector2 lineVector = end - start;
		float angle = Mathf.Rad2Deg * Mathf.Atan (lineVector.y / lineVector.x);
		if (lineVector.x < 0) {
			angle += 180;
		}

		if (thickness < 1) {
			thickness = 1;
		}

		// The center of the line will always be at the center
		// regardless of the thickness.
		int thicknessOffset = (int)Mathf.Ceil (thickness / 2);

		GUIUtility.RotateAroundPivot (angle,
			start);
		GUI.DrawTexture (new Rect (start.x,
			start.y - thicknessOffset,
			lineVector.magnitude,
			thickness),
			_coloredLineTexture);

			GUIUtility.RotateAroundPivot(-angle, start);

	}



	public static void clearAll ()
	{
//		arrowHead.SetActive (false);
	}

	public void init ()
	{


		texture2D = setTexture;

		gravity = -Physics2D.gravity.y * Time.fixedDeltaTime;
		clearAll ();

	}

	#endregion

	#region private functions

	#endregion

	#region event handlers

	#endregion
}
