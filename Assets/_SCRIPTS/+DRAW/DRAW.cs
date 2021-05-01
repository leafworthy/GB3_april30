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

	public static GameObject arrowHead;
	public GameObject arrowHeadPrefab;
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
	public GameObject circleSprite;
	public float filledCircleRadius = 1f;
	private static List<GameObject> Circles = new List<GameObject>();

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

	public static Vector2 trajectory (Vector2 vel, Vector2 start, int foresight, float _gravity = 1)
	{
		if (Circles.Count > 0)
		{
			foreach (var circle in Circles)
			{
				MAKER.Unmake(circle);
			}
		}
		Debug.Log("DRAWINGDRAJECTORY");
		Vector2 lastP = start;
		Vector2 p = start;
		int chunk = 1;
		var circle1 = MAKER.Make(I.circleSprite, p);
		Circles.Add(circle1);
		Vector2 lookPos = Vector2.zero;
		if (vel != Vector2.zero) {

			for (int i = 0; i < foresight + 1; i++) {

				p = lastP;
				var circle = MAKER.Make(I.circleSprite, p);
				Circles.Add(circle);
				vel = new Vector2 (vel.x, vel.y - _gravity * Time.fixedDeltaTime) * chunk;
				p += vel * chunk;

				if (i == foresight - 1) {
					lookPos = lastP - p;
				}
				lastP = p;

			}

			arrowHead.SetActive (true);
			arrowHead.transform.position = lastP;



			//float angle = Mathf.Atan2 (lookPos.y, lookPos.x) * Mathf.Rad2Deg + 90f;


			//arrowHead.transform.localRotation = Quaternion.AngleAxis (angle, Vector3.forward);

		}
		else
		{
			Debug.Log("NO vel");
		}
		return Vector2.zero;
	}

	public static void clearAll ()
	{
//		arrowHead.SetActive (false);
	}

	public void init ()
	{


		texture2D = setTexture;

		gravity = -Physics2D.gravity.y * Time.fixedDeltaTime;
		arrowHead = (GameObject)Instantiate (arrowHeadPrefab, Vector2.zero, Quaternion.identity);
		clearAll ();

	}

	#endregion

	#region private functions

	#endregion

	#region event handlers

	#endregion
}
