using System;
using UnityEngine;

public class CarAccessInteraction : TimedInteraction
{
	private bool occupied;
	private bool hasBeenOpened;
	private bool hasGas;
	public event Action<Player> OnCarAccessActionPressed;

	private bool gasFilled;

	protected override void Start()
	{
		base.Start();
		OnPlayerEnters += Interactable_OnPlayerEnters;
		OnPlayerExits += Interactable_OnPlayerExits;
		OnTimeComplete += Interactable_OnTimeComplete;
	}

	protected override void InteractableOnActionPress(Player player)
	{
		if (gasFilled)
		{
			if (HasKey())
			{
				OnCarAccessActionPressed?.Invoke(player);
				FinishInteraction(player);
			}
			else
				player.Say("Needs a key...", 0);
		}
		else
		{
			if (!HasEnoughGas())
				player.Say("Needs more gas...", 0);
			else
			{
				player.Say("Filling gas...", 0);
				SFX.sounds.siphon_gas_sound.PlayRandomAt(transform.position);
				base.InteractableOnActionPress(player);
			}
		}
	}

	private bool HasKey() => DoAnyPlayersHaveAKey();

	private bool HasEnoughGas() => GetTotalGasFromAllJoinedPlayers() >= GlobalManager.GasGoal;

	private bool DoAnyPlayersHaveAKey()
	{
		foreach (var player in Players.AllJoinedPlayers)
		{
			if (player.hasKey) return true;
		}

		return false;
	}

	private int GetTotalGasFromAllJoinedPlayers()
	{
		var totalGas = 0;
		foreach (var player in Players.AllJoinedPlayers)
		{
			totalGas += player.GetPlayerStatAmount(PlayerStat.StatType.Gas);
		}

		return totalGas;
	}

	private bool HasSomeGas() => GetTotalGasFromAllJoinedPlayers() > 0 && GetTotalGasFromAllJoinedPlayers() < GlobalManager.GasGoal;

	private void Interactable_OnTimeComplete(Player player)
	{
		if (!HasEnoughGas() || gasFilled) return;
		gasFilled = true;
		player.ChangePlayerStat(PlayerStat.StatType.Gas, -GlobalManager.GasGoal);
		occupied = false;
		Interactable_OnPlayerEnters(player);
	}

	private void Interactable_OnPlayerEnters(Player player)
	{
		if (occupied) return;
		occupied = true;
		if (gasFilled)
		{
			if (HasKey())
			{
				player.Say("Let's go!", 0);
				Debug.Log("Success!");
			}
			else
				player.Say("Needs a key...", 0);
		}
		else
		{
			if (HasEnoughGas())
			{
				player.Say("Hold to fill.", 0);
				return;
			}

			if (HasSomeGas())
				player.Say("Needs more gas...", 0);
			else
				player.Say("Needs gas...", 0);
		}
	}

	private void Interactable_OnPlayerExits(Player player)
	{
		if (!occupied) return;
		occupied = false;
		player.StopSaying();
	}
}