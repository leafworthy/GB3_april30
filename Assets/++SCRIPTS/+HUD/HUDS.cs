using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDS : Singleton<HUDS>
{
	private bool isOn;
	private List<HUDSlot> currentHUDSlots = new List<HUDSlot>();

	public GameObject Vignette;
	public GameObject Clock;

	private void Start()
	{
		Vignette.SetActive(false);
		Clock.SetActive(false);
		Debug.Log("huds starts");
		LevelManager.OnStartLevel += LevelSceneOnStartLevel;
		LevelManager.OnStopLevel += LevelSceneOnStopLevel;
		LevelManager.OnPlayerSpawned += LevelScene_OnPlayerSpawned;
		DisableAllHUDSlots();
		
	}

	private void LevelSceneOnStopLevel(GameLevel gameLevel)
	{
		Vignette.SetActive(false);
		Clock.SetActive(false);
		Players.OnPlayerJoins -= JoinInGame;
		DisableAllHUDSlots();
	}

	

	private void LevelScene_OnPlayerSpawned(Player player)
	{
		SetHUDSlotPlayer(player);
	}

	private void LevelSceneOnStartLevel(GameLevel gameLevel)
	{
		Debug.Log("made it here");
		CreateHUDForPlayers(Players.AllJoinedPlayers);
		Players.OnPlayerJoins += JoinInGame;
		Players.OnPlayerGetUpgrades += OpenUpgradePanel;
		Vignette.SetActive(true);
		Clock.SetActive(true);
	}

	private void OpenUpgradePanel(Player player)
	{
		var slot = I.currentHUDSlots[(int)player.input.playerIndex];
		slot.gameObject.SetActive(true);
		Clock.SetActive(true);
		slot.StartUpgradeSelectMenu(player);
	}


	private void CreateHUDForPlayers(List<Player> players)
	{
		foreach (var player in players)
		{
			Debug.Log("set hud slot");
			SetHUDSlotPlayer(player);
		}
	}

	private static void JoinInGame(Player player)
	{
		Debug.Log("join in game");
		var slot = I.currentHUDSlots[(int) player.input.playerIndex];
		slot.gameObject.SetActive(true);
		slot.StartCharSelectMenu(player);
	}

	private static void SetHUDSlotPlayer(Player player)
	{
		var slot = I.currentHUDSlots[(int)player.input.playerIndex];
		slot.gameObject.SetActive(true);
		slot.SetPlayer(player);
	}
	private void DisableAllHUDSlots()
	{
		currentHUDSlots = GetComponentsInChildren<HUDSlot>(true).ToList();
		foreach (var hudSlot in currentHUDSlots) hudSlot.gameObject.SetActive(false);
	}
}