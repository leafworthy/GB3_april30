using System;
using System.Collections.Generic;
using _SCRIPTS;
using UnityEngine;

public class HUD : Singleton<HUD>
{
	public GameObject GangstaBeanHUDPrefab;
	public GameObject BrockLeeHUDPrefab;
	public List<GameObject> HUDSlots = new List<GameObject>();

	public static void SetPlayers(List<Player> players)
	{
		for (var index = 0; index < players.Count; index++){
			var player = players[index];
			switch (player.currentCharacter)
			{
				case Character.None:
					CreateCharHUD(I.GangstaBeanHUDPrefab, player,I.HUDSlots[index]);
					break;
				case Character.Karrot:
					CreateCharHUD(I.GangstaBeanHUDPrefab, player, I.HUDSlots[index]);
					break;
				case Character.Bean:
					CreateCharHUD(I.GangstaBeanHUDPrefab, player, I.HUDSlots[index]);
					break;
				case Character.Brock:
					CreateCharHUD(I.BrockLeeHUDPrefab, player, I.HUDSlots[index]);
					break;
				case Character.Tmato:
					CreateCharHUD(I.BrockLeeHUDPrefab, player, I.HUDSlots[index]);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

		}
	}

	private static void CreateCharHUD(GameObject charHUDPrefab, Player player, GameObject parent)
	{
		var newHUD = MAKER.Make(charHUDPrefab, Vector2.zero);
		newHUD.transform.SetParent(parent.transform);
		newHUD.transform.localPosition = Vector3.zero;
		var newHudChar = newHUD.GetComponent<CharacterHUD>();
		newHudChar.SetPlayer(player);
	}
}
