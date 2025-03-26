using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using RisingText;

namespace UpgradeS
{
	public class UpgradeSetupMenu : MonoBehaviour, INeedPlayer
	{
		public UpgradeSelectButtons buttons;
		public GameObject Visible;
		private Player owner;
		private PlayerUpgrades playerUpgrades;
		private Animator anim;
		private static readonly int IsClosed = Animator.StringToHash("IsClosed");
		public event Action<Player> OnUpgradePurchased;
		public event Action<Player> OnUpgradeExit;
		private void OnEnable()
		{
			LevelManager.OnStopLevel += LevelGameSceneOnStopLevel;
			anim = GetComponentInChildren<Animator>();
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
			CloseUpgradeSelectMenu();
			buttons.OnUpgradeChosen -= Buttons_OnUpgradeChosen;
		}

		private void CloseUpgradeSelectMenu()
		{
			Visible.SetActive(false);
			buttons.OnExit -= Buttons_OnExit;
			buttons.OnUpgradeChosen -= Buttons_OnUpgradeChosen;
			OnUpgradeExit?.Invoke(owner);
			//gameObject.SetActive(false);
		}

		private void Buttons_OnUpgradeChosen(Upgrade upgrade)
		{
	
			if (!playerUpgrades.BuyUpgrade(upgrade))
			{
				SFX.sounds.charSelect_deselect_sounds.PlayRandom();
				RisingTextCreator.CreateRisingText("Not enough cash" , owner.SpawnedPlayerGO.transform.position, Color.red);
				return;
			}

			SFX.sounds.charSelect_select_sounds.PlayRandom();
			transform.DOPunchScale(Vector3.one*.1f, 0.5f, 1, 0.2f);
			RisingTextCreator.CreateRisingText(upgrade.GetDescription, owner.SpawnedPlayerGO.transform.position, Color.white);
			OnUpgradePurchased?.Invoke(owner);
			
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
}