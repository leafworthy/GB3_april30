using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class CarAccessInteraction : TimedInteraction
	{
		private bool hasBeenOpened;
		private bool hasGas;
		public event Action<Player> OnCarAccessActionPressed;

		private bool gasFilled;

		protected override void OnEnable()
		{
			base.OnEnable();
			OnPlayerEnters += Interactable_OnPlayerEnters;
			OnPlayerExits += Interactable_OnPlayerExits;
			OnTimeComplete += Interactable_OnTimeComplete;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			OnPlayerEnters -= Interactable_OnPlayerEnters;
			 OnPlayerExits -= Interactable_OnPlayerExits;
			  OnTimeComplete -= Interactable_OnTimeComplete;

		}

		protected override void InteractableOnActionPress(Player player)
		{
			if (gasFilled)
			{
				//if (HasKey())
				//{
					OnCarAccessActionPressed?.Invoke(player);
					FinishInteraction(player);
				//}
				//else
					//player.Say("Need a key...", 0);
			}
			else
			{
				if (!HasEnoughGas())
					player.Say("Need more gas...", 0);
				else
				{
					player.Say("Filling gas...", 0);
					Services.sfx.sounds.siphon_gas_sound.PlayRandomAt(transform.position);
					base.InteractableOnActionPress(player);
				}
			}
		}

		private bool HasKey() => DoAnyPlayersHaveAKey();

		private bool HasEnoughGas() => GetTotalGasFromAllJoinedPlayers() >= Services.assetManager.Vars.GasGoal;

		private bool DoAnyPlayersHaveAKey()
		{
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				if (player.hasKey) return true;
			}

			return false;
		}

		private int GetTotalGasFromAllJoinedPlayers()
		{
			var totalGas = 0;
			foreach (var player in Services.playerManager.AllJoinedPlayers)
			{
				totalGas += (int) Services.playerStatsManager.GetStatAmount(player,PlayerStat.StatType.Gas);
			}


			return totalGas;
		}

		private bool HasSomeGas() => GetTotalGasFromAllJoinedPlayers() > 0 && GetTotalGasFromAllJoinedPlayers() < Services.assetManager.Vars.GasGoal;

		private void Interactable_OnTimeComplete(Player player)
		{
			if (!HasEnoughGas() || gasFilled) return;
			gasFilled = true;
			Services.playerStatsManager.ChangeStat(player,PlayerStat.StatType.Gas, -Services.assetManager.Vars.GasGoal);
		Interactable_OnPlayerEnters(player);
		}

		private void Interactable_OnPlayerEnters(Player player)
		{

			if (gasFilled)
			{
				//if (HasKey())
				//{
					player.Say("Let's go!", 0);

				//}
				//else
				//	player.Say("Needs a key...", 0);
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
			player.StopSaying();
		}
	}
}
