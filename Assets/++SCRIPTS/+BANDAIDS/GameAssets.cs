using UnityEngine;

public class GameAssets : MonoBehaviour
{
	private static GameAssets _i;

	public static GameAssets i
	{
		get
		{
			if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
			return _i;
		}
	}


	public int ignoreRaycastLayer;
	public int playerLayer;
	public int enemyLayer;
	public int wallLayer;
	public int doorLayer;
	public int shieldLayer;


	public Sprite s_ShootFlash;
	public Sprite s_ShieldTransformerDestroyed;
	public Sprite s_PistolIcon;
	public Sprite s_ShotgunIcon;
	public Sprite s_RifleIcon;

	public Transform pfSwordSlash;
	public Transform pfEnemy;
	public Transform pfEnemyCharger;
	public Transform pfEnemyArcher;
	public Transform pfEnemyFlyingBody;
	public Transform pfEnemyShuriken;
	public Transform pfImpactEffect;
	public Transform pfDamagePopup;
	public Transform pfPickupHealth;

	public Material m_WeaponTracer;
	public Material m_MarineSpriteSheet;

	public Material m_DoorRed;
	public Material m_DoorGreen;
	public Material m_DoorBlue;
	public Material m_DoorKeyHoleRed;
	public Material m_DoorKeyHoleGreen;
	public Material m_DoorKeyHoleBlue;

	public Material m_LineEmissionRed;
	public Material m_LineEmissionYellow;
	public Material m_SpritesDefault;
	public Material m_PlayerWinOutline;
}
