using System;
using UnityEngine;

public class UpgradeSetupMenu : MonoBehaviour
{
	public UpgradeSelectButtons buttons;

	private Player owner;
	private bool inputEnabled;
	private PlayerUpgrades playerUpgrades;
	private Animator anim;
	private static readonly int IsClosed = Animator.StringToHash("IsClosed");

	private void OnEnable()
	{
		LevelGameScene.OnStop += LevelGameScene_OnStop;
		anim = GetComponent<Animator>();
	}

	private void LevelGameScene_OnStop(GameScene.Type obj)
	{
		Unsetup();
	}

	private void Unsetup()
	{
		owner = null;
		gameObject.SetActive(false);
	}

	public event Action<Player> OnUpgradePurchased;

	public void Setup(Player player, HUDSlot hudSlot)
	{
		Debug.Log("made it in menu", this);
		owner = player;
		playerUpgrades = player.GetComponent<PlayerUpgrades>();
		gameObject.SetActive(true);
		buttons.OnUpgradeChosen += Buttons_OnUpgradeChosen;
		buttons.OnExit += Buttons_OnExit;
		buttons.Init(player);
		Players.SetActionMap(player, Players.UIActionMap);
		anim.SetBool(IsClosed, false);
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



}