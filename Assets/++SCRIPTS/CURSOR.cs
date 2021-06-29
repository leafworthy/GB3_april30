using UnityEngine;
using static ASSETS;

public class CURSOR : Singleton<CURSOR>
{
	private static Vector2 hotSpot = Vector2.zero;

	public static void ShowCursor()
	{
		hotSpot = new Vector2(FX.cursorTexture2D.width / 2, FX.cursorTexture2D.height / 2);
		Cursor.visible = true;
		Cursor.SetCursor(FX.cursorTexture2D, hotSpot, CursorMode.Auto);
	}

	public static void HideCursor()
	{
		Cursor.visible = false;
	}
}
