using System.Collections.Generic;

public class ShoppeInteraction : PlayerInteractable
{
	private List<Player> playersInShoppe = new();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		OnPlayerEnters += PlayerEnters;
		OnPlayerExits += PlayerExits;
		OnActionPress += ActionPress;
	}

	private void ActionPress(Player player)
	{
		var playerCash = PlayerStatsManager.I.GetStatAmount(player, PlayerStat.StatType.TotalCash);
		OpenShoppeWindow(player);
	}

	private void OpenShoppeWindow(Player player)
	{
		if (playersInShoppe.Contains(player)) return;
		playersInShoppe.Add(player);
		Players.PlayerOpensShoppe(player);
	}

	private void PlayerEnters(Player player)
	{
		player.Say("Buy?");
	}

	private void PlayerExits(Player player)
	{
		player.StopSaying();
	}

	// Update is called once per frame
	private void Update()
	{
	}
}