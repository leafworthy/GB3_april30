using UnityEngine;
using System.Collections;

public class CustomCursor : MonoBehaviour
{
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	private Vector2 hotSpot = Vector2.zero;

	void Start()
	{
		hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
}
