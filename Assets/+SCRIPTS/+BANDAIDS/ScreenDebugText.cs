using __SCRIPTS;
using UnityEngine;

public class ScreenDebugText : MonoBehaviour
{
	public string text = "Hello World!"; // Text to display
	public int fontSize = 16; // Font size
	public Color fontColor = Color.white; // Text color
	public Vector2 padding = new Vector2(10, 10); // Offset from top-right
	private Body body;
	private GUIStyle guiStyle;



	private void Awake()
	{
		guiStyle = new GUIStyle();
		guiStyle.fontSize = fontSize;
		guiStyle.normal.textColor = fontColor;
		guiStyle.alignment = TextAnchor.UpperRight;
		body = GetComponent<Body>();
	}

	private void Update()
	{
		SetText("Arms: " + (body.doableArms.CurrentAbility?.AbilityName ?? "Idle") + "\nLegs: " + (body.doableLegs.CurrentAbility?.AbilityName ?? "Idle"));
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
