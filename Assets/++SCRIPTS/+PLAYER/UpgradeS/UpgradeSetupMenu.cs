using System;
using UnityEngine;

public class UpgradeSetupMenu : MonoBehaviour, INeedPlayer
{
	public UpgradeSelectButtons buttons;
	public GameObject Visible;
	private Player owner;
	private bool inputEnabled;
	private PlayerUpgrades playerUpgrades;
	private Animator anim;
	private static readonly int IsClosed = Animator.StringToHash("IsClosed");

	private void OnEnable()
	{
		LevelManager.OnStopLevel += LevelGameSceneOnStopLevel;
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		Visible.SetActive(false);
		Debug.Log("set active start");
	}

	private void LevelGameSceneOnStopLevel(GameLevel gameLevel)
	{
		Unsetup();
	}

	private void Unsetup()
	{
		owner = null;
		Visible.SetActive(false);
	}

	private void OnDisable()
	{
		LevelManager.OnStopLevel -= LevelGameSceneOnStopLevel;
	}

	public event Action<Player> OnUpgradePurchased;

	public void SetPlayer(Player player)
	{
		Debug.Log("made it in menu", this);
		owner = player;

		Debug.Log("set player");
		Visible.SetActive(false);
	}

	private void Buttons_OnExit()
	{
		Players.SetActionMap(owner, Players.PlayerActionMap);
		anim.SetBool(IsClosed, true);
		//gameObject.SetActive(false);
		buttons.OnUpgradeChosen -= Buttons_OnUpgradeChosen;
	}

	private void Buttons_OnUpgradeChosen(Upgrade upgrade)
	{
	
		if (!playerUpgrades.BuyUpgrade(upgrade))
		{
			SFX.sounds.charSelect_deselect_sounds.PlayRandom();
			return;
		}

		SFX.sounds.charSelect_select_sounds.PlayRandom();
		OnUpgradePurchased?.Invoke(owner);
		Buttons_OnExit();
	}

	public void StartUpgradeSelectMenu(Player player)
	{
		Debug.Log("started upgrade menu");
		SetPlayer(player);
		playerUpgrades = player.GetComponent<PlayerUpgrades>();
		Visible.SetActive(true);
		buttons.OnUpgradeChosen += Buttons_OnUpgradeChosen;
		buttons.OnExit += Buttons_OnExit;
		buttons.Init(player);
		Players.SetActionMap(player, Players.UIActionMap);
		anim.SetBool(IsClosed, false);
	}
}