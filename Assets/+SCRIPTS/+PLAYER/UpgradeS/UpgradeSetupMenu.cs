﻿using System;
using __SCRIPTS.HUD_Displays;
using __SCRIPTS.RisingText;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	public class UpgradeSetupMenu : ServiceUser, INeedPlayer
	{
		public UpgradeSelectButtons buttons;
		public GameObject Visible;
		private Player owner;
		private PlayerUpgrades playerUpgrades;
		private Animator anim;
		private static readonly int IsClosed = Animator.StringToHash("IsClosed");
		public event Action<Player> OnUpgradePurchased;
		public event Action<Player> OnUpgradeExit;
		private bool inMenu;
		private void OnEnable()
		{
			levelManager.OnStopLevel += LevelGameSceneOnStopLevel;
			pauseManager.OnPause += PauseWhileInMenu;

		}

		private void Start()
		{
			Visible.SetActive(false);
		}

		private void PauseWhileInMenu(Player player)
		{
			if (!inMenu) return;
			if (player == owner)
			{
				Players.SetActionMap(owner, Players.UIActionMap);
				anim.SetBool(IsClosed, false);
			}
			else
			{
				Players.SetActionMap(player, Players.PlayerActionMap);
				anim.SetBool(IsClosed, true);
			}
		}


		private void LevelGameSceneOnStopLevel(GameLevel gameLevel)
		{
			Unsetup();
		}

		private void Unsetup()
		{
			CloseUpgradeSelectMenu();
		}

		private void OnDisable()
		{
			levelManager.OnStopLevel -= LevelGameSceneOnStopLevel;
			pauseManager.OnPause -= PauseWhileInMenu;
		}



		public void SetPlayer(Player _player)
		{

			owner = _player;


			Visible.SetActive(false);
		}

		private void Buttons_OnExit()
		{
			Players.SetActionMap(owner, Players.PlayerActionMap);
			anim.SetBool(IsClosed, true);
			//gameObject.SetActive(false);
			CloseUpgradeSelectMenu();

			buttons.OnUpgradeChosen -= Buttons_OnUpgradeChosen;
		}

		private void CloseUpgradeSelectMenu()
		{
			inMenu = false;
			Visible.SetActive(false);
			buttons.OnExit -= Buttons_OnExit;
			buttons.OnUpgradeChosen -= Buttons_OnUpgradeChosen;
			OnUpgradeExit?.Invoke(owner);
			owner = null;
			//gameObject.SetActive(false);
		}

		private void Buttons_OnUpgradeChosen(Upgrade upgrade)
		{

			if (!playerUpgrades.BuyUpgrade(upgrade))
			{
				sfx.sounds.charSelect_deselect_sounds.PlayRandom();
				risingText.CreateRisingText("Not enough cash" , owner.SpawnedPlayerGO.transform.position, Color.red);
				return;
			}

			sfx.sounds.charSelect_select_sounds.PlayRandom();

			risingText.CreateRisingText(upgrade.GetDescription, owner.SpawnedPlayerGO.transform.position, Color.white);
			OnUpgradePurchased?.Invoke(owner);

		}

		public void StartUpgradeSelectMenu(Player player)
		{

			inMenu = true;
			SetPlayer(player);
			playerUpgrades = player.GetComponent<PlayerUpgrades>();
			Visible.SetActive(true);

			buttons.OnUpgradeChosen += Buttons_OnUpgradeChosen;
			buttons.OnExit += Buttons_OnExit;
			buttons.Init(player);
			Players.SetActionMap(player, Players.UIActionMap);
			anim = GetComponentInChildren<Animator>();
			anim.SetBool(IsClosed, false);


		}
	}
}
