using System.Collections.Generic;
using UnityEngine;

public class HUDS : MonoBehaviour
{
	private bool isOn;
	public HUDHandler currentHUD;
	private static HUDS I;

	private void Awake()
	{
		I = this;
	}

	private void Start()
	{
		Players.OnPlayerJoins += JoinInGame;
		DisableAllHUDSlots();
		//currentHUD.gameObject.SetActive(false);
		CreateHUD(Players.AllJoinedPlayers);
	}

	  



	private void CreateHUD(List<Player> players)
	{
		foreach (var player in players)
		{
			SetHUDSlotCharacter(player);
		}
	}

	private static void JoinInGame(Player player)
	{
		if (!Game_GlobalVariables.IsInLevel) return;
		var slot = I.currentHUD.HUDSlots[(int)player.input.playerIndex];
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
