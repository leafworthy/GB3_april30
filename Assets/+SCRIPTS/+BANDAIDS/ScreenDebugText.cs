using __SCRIPTS;
using UnityEngine;

public class ScreenDebugText : MonoBehaviour
{
	public string text = "Hello World!"; // Text to display
	private int fontSize = 40; // Font size
	public Color fontColor = Color.white; // Text color
	public Vector2 padding = new Vector2(10, 10); // Offset from top-right
	private Body body  => _body ??= GetComponent<Body>();
	private Body _body;
	private GUIStyle guiStyle;
	private ThreeWeaponSwitchAbility threeWeaponSwitchAbility => _threeWeaponSwitchAbility ??= GetComponent<ThreeWeaponSwitchAbility>();
	private ThreeWeaponSwitchAbility _threeWeaponSwitchAbility;
	private ChainsawAttack chainsawAttack => _chainsawAttack ??= GetComponent<ChainsawAttack>();
	private ChainsawAttack _chainsawAttack;
	private ShieldAbility shieldAbility => _shieldAbility ??= GetComponent<ShieldAbility>();
	private ShieldAbility _shieldAbility;
	private GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
	private GunAttack _gunAttack;
	private   ShieldDashAbility shieldDashAbility  => _shieldDashAbility ??= GetComponent<ShieldDashAbility>();
	private   ShieldDashAbility _shieldDashAbility ;

	private void Awake()
	{
		guiStyle = new GUIStyle();
		guiStyle.fontSize = fontSize;
		guiStyle.normal.textColor = fontColor;
		guiStyle.alignment = TextAnchor.UpperRight;
	}

	private void Update()
	{
		SetText("Arms: " + (body?.doableArms.CurrentAbility?.AbilityName ?? "Idle") +
		        "\nLegs: " + (body?.doableLegs.CurrentAbility?.AbilityName ?? "Idle")+
		        "\nChainsaw: " + (chainsawAttack?.AbilityName + " " + chainsawAttack?.currentState) +
		        "\nShield: " + (shieldAbility?.AbilityName + " " + shieldAbility?.currentState) +
		        "\nGunAttack: " + (gunAttack?.AbilityName + " " + gunAttack?.currentState) +
		        "\nCurrentWeapon: " + (threeWeaponSwitchAbility?.currentWeapon?.AbilityName + " " + threeWeaponSwitchAbility?.currentWeapon?.AbilityName)) ;
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
