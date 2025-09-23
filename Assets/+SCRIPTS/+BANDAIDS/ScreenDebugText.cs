using __SCRIPTS;

public class ScreenDebugText : GUIDebugTextShower
{
	private Body body  => _body ??= GetComponent<Body>();
	private Body _body;
	private ThreeWeaponSwitchAbility threeWeaponSwitchAbility => _threeWeaponSwitchAbility ??= GetComponent<ThreeWeaponSwitchAbility>();
	private ThreeWeaponSwitchAbility _threeWeaponSwitchAbility;
	private ChainsawAttack chainsawAttack => _chainsawAttack ??= GetComponent<ChainsawAttack>();
	private ChainsawAttack _chainsawAttack;
	private ShieldAbility shieldAbility => _shieldAbility ??= GetComponent<ShieldAbility>();
	private ShieldAbility _shieldAbility;
	private GunAttack gunAttack => _gunAttack ??= GetComponent<GunAttack>();
	private GunAttack _gunAttack;


	private void Update()
	{
		SetText("Arms: " + (body?.doableArms.CurrentAbility?.AbilityName ?? "Idle") +
		        "\nLegs: " + (body?.doableLegs.CurrentAbility?.AbilityName ?? "Idle")+
		        "\nChainsaw: " + (chainsawAttack?.AbilityName + " " + chainsawAttack?.currentState) +
		        "\nShield: " + (shieldAbility?.AbilityName + " " + shieldAbility?.currentState) +
		        "\nGunAttack: " + (gunAttack?.AbilityName + " " + gunAttack?.currentState) +
		        "\nCurrentWeapon: " + (threeWeaponSwitchAbility?.currentWeapon?.AbilityName + " " + threeWeaponSwitchAbility?.currentWeapon?.AbilityName)) ;
	}


}
