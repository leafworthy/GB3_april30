using System;
using System.Collections.Generic;
using UnityEngine;

public class HUD : Singleton<HUD>
{
	public List<HUDSlot> HUDSlots = new List<HUDSlot>();

	public static void SetPlayers(List<Player> players)
	{
		DisableAllHUDSlots();
		for (var index = 0; index < players.Count; index++)
		{
			var player = players[index];
			var slot = I.HUDSlots[index];
			slot.gameObject.SetActive(true);
			slot.SetCharacter(player.currentCharacter, players[index]);
		}
	}

	private static void DisableAllHUDSlots()
	{
		foreach (var hudSlot in I.HUDSlots) hudSlot.gameObject.SetActive(false);
	}
}
