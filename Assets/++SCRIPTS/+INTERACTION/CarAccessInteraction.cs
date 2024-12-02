using System;
using UnityEngine;

public class CarAccessInteraction : TimedInteraction
{
	private bool occupied;
	private bool hasBeenOpened;
	private bool hasGas;
	public event Action<Player> OnCarAccessActionPressed;
	public Player owner;
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
			if (HasKey(player))
			{
				OnCarAccessActionPressed?.Invoke(player);
				FinishInteraction(player);
				Debug.Log("Success!");
				return;
			}
			else
			{
				player.Say("Needs a key...", 0);
				return;
			}
		}
		else
		{
			if (!HasEnoughGas(player))
			{
				player.Say("Needs more gas...", 0);
				return;
			}
			else
			{
				player.Say("Filling gas...", 0);
				SFX.sounds.siphon_gas_sound.PlayRandomAt(transform.position);
				base.InteractableOnActionPress(player);
				return;
			}
		}
	}

	private bool HasKey(Player player) => player.hasKey;

	private bool HasEnoughGas(Player player) => player.GetPlayerStatAmount(PlayerStat.StatType.Gas) >= GlobalManager.GasGoal;

	private bool HasSomeGas(Player player) => player.GetPlayerStatAmount(PlayerStat.StatType.Gas) > 0 &&
	                                          player.GetPlayerStatAmount(PlayerStat.StatType.Gas) < GlobalManager.GasGoal;

	private void Interactable_OnTimeComplete(Player player)
	{
		if (!HasEnoughGas(player) || gasFilled) return;
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
			if (HasKey(player))
			{


				player.Say("Let's go!", 0);
				Debug.Log("Success!");
				return;
			}
			else
			{

				player.Say("Needs a key...", 0);
				return;
			}
		}
		else
		{
			if (HasEnoughGas(player))
			{
				player.Say("Hold to fill.", 0);
				return;
			}
			if (HasSomeGas(player))
			{

				player.Say("Needs more gas...", 0);
				return;
			}
			else
			{
				player.Say("Needs gas...", 0);
				return;
			}
		}

		

		

		

		
	}

	private void Interactable_OnPlayerExits(Player player)
	{
		if (!occupied) return;
		occupied = false;
		player.StopSaying();
	}

	
}