using System.Collections.Generic;
using UnityEngine;

public class HUDS : Singleton<HUDS>
{
	private bool isOn;
	public HUDHandler currentHUD;
	public GameObject Vignette;

	private void Start()
	{
		Vignette.SetActive(false);
		
		LevelGameScene.OnStart += LevelScene_OnStart;
		LevelGameScene.OnStop += LevelScene_OnStop;
		LevelGameScene.OnPlayerSpawned += LevelScene_OnPlayerSpawned;
		DisableAllHUDSlots();
		
	}

	private void LevelScene_OnStop(GameScene.Type obj)
	{
		Vignette.SetActive(false);
		Players.OnPlayerJoins -= JoinInGame;
		DisableAllHUDSlots();
	}

	

	private void LevelScene_OnPlayerSpawned(Player player)
	{
		SetHUDSlotCharacter(player);
	}

	private void LevelScene_OnStart()
	{
		//CreateHUDForPlayers(Players.AllJoinedPlayers);
		Players.OnPlayerJoins += JoinInGame;
		Players.OnPlayerGetUpgrades += OpenUpgradePanel;
		Vignette.SetActive(true);
	}

	private void OpenUpgradePanel(Player player)
	{
		var slot = I.currentHUD.HUDSlots[(int)player.input.playerIndex];
		slot.gameObject.SetActive(true);
		slot.StartUpgradeSelectMenu(player);
	}


	private void CreateHUDForPlayers(List<Player> players)
	{
		foreach (var player in players)
		{
			SetHUDSlotCharacter(player);
		}
	}

	private static void JoinInGame(Player player)
	{
		Debug.Log("join in game");
		var slot = I.currentHUD.HUDSlots[(int) player.input.playerIndex];
		slot.gameObject.SetActive(true);
		slot.StartCharSelectMenu(player);
	}

	private static void SetHUDSlotCharacter(Player player)
	{
		var slot = I.currentHUD.HUDSlots[(int)player.input.playerIndex];
		slot.gameObject.SetActive(true);
		slot.SetCharacter(player);
	}
	private void DisableAllHUDSlots()
	{
		foreach (var hudSlot in currentHUD.HUDSlots) hudSlot.gameObject.SetActive(false);
	}
}