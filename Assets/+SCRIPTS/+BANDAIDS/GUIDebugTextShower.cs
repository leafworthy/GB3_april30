using UnityEngine;

public class GUIDebugTextShower : MonoBehaviour
{
	protected string text = "Hello World!";
	protected int fontSize = 40;
	protected Color fontColor = Color.white;
	protected Vector2 padding = new Vector2(10, 10);
	protected GUIStyle guiStyle;
	protected virtual TextAnchor alignment  => TextAnchor.UpperRight;

	protected void Awake()
	{
		guiStyle = new GUIStyle();
		guiStyle.fontSize = fontSize;
		guiStyle.normal.textColor = fontColor;
		guiStyle.alignment = alignment;
	}

	private void OnGUI()
	{
		// Calculate position at top-right with padding
		float x = Screen.width - padding.x;
		float y = padding.y;

		GUI.Label(new Rect(0, 0, Screen.width - padding.x, fontSize + 4), text, guiStyle);
	}

	// Optional helper to update the text at runtime
	public void SetText(string newText)
	{
		text = newText;
	}
}
