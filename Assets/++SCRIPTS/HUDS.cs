using System.Collections.Generic;

public class HUDS : Singleton<HUDS>
{
	private bool isOn;
	private HUDHandler currentHUD;

	private void Start()
	{
		LEVELS.OnLevelStart += LevelStarts;
		LEVELS.OnLevelStop += LevelStops;
		currentHUD = MAKER.Make(ASSETS.ui.HUD).GetComponent<HUDHandler>();
		currentHUD.gameObject.SetActive(false);
	}

	private void LevelStarts(List<Player> players)
	{
		SetPlayers(players);
		currentHUD.gameObject.SetActive(true);
	}

	private void LevelStops()
	{
		currentHUD.gameObject.SetActive(false);
	}

	private void SetPlayers(List<Player> players)
	{
		DisableAllHUDSlots();
		for (var index = 0; index < players.Count; index++)
		{
			var player = players[index];
			var slot = currentHUD.HUDSlots[index];
			slot.gameObject.SetActive(true);
			slot.SetCharacter(player.currentCharacter, players[index]);
		}
	}

	private void DisableAllHUDSlots()
	{
		foreach (var hudSlot in currentHUD.HUDSlots) hudSlot.gameObject.SetActive(false);
	}
}
