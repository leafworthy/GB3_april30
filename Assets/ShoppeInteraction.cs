using System.Collections.Generic;
using UnityEngine;

public class ShoppeInteraction : PlayerInteractable
{
	private static readonly int IsOpen = Animator.StringToHash("IsOpen");
	private List<Player> playersInShoppe = new();
	private Animator animator;

	private void Start()
	{
		animator = GetComponentInChildren<Animator>();
		OnPlayerEnters += PlayerEnters;
		OnPlayerExits += PlayerExits;
		OnActionPress += ActionPress;
	}

	private void ActionPress(Player player)
	{
		OpenShoppeWindow(player);
	}

	private void OpenShoppeWindow(Player player)
	{
		if (!playersInShoppe.Contains(player)) playersInShoppe.Add(player);
		animator.SetBool(IsOpen, true);
		Players.PlayerOpensShoppe(player);
	}

	private void PlayerClosesShoppe(Player player)
	{
		if (playersInShoppe.Contains(player)) playersInShoppe.Remove(player);
		if (playersInShoppe.Count == 0) animator.SetBool(IsOpen, false);
	}

	private void PlayerEnters(Player player)
	{
		Player.OnPlayerLeavesUpgradeSetupMenu += PlayerClosesShoppe;
		player.Say("Buy?");
	}

	private void PlayerExits(Player player)
	{
		Player.OnPlayerLeavesUpgradeSetupMenu -= PlayerClosesShoppe;
		player.StopSaying();
		PlayerClosesShoppe(player);
	}
}