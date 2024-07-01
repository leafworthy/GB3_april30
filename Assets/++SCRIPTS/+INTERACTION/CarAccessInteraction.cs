using System;
using __SCRIPTS._COMMON;
using __SCRIPTS._MANAGERS;
using __SCRIPTS._PLAYER;

namespace __SCRIPTS._INTERACTION
{
	public class CarAccessInteraction : TimedInteraction
	{

		private bool occupied;
		private bool hasBeenOpened;
		public event Action<Player> OnCarAccessActionPressed;
		public Player owner;
		protected override void Start()
		{
			base.Start();
			OnPlayerEnters += Interactable_OnPlayerEnters;
			OnPlayerExits += Interactable_OnPlayerExits;
			OnTimeComplete += Interactable_OnTimeComplete;
		}

		protected override void InteractableOnActionPress(Player player)
		{
			if (hasBeenOpened)
			{
				OnCarAccessActionPressed?.Invoke(player);
				return;
			}
			if (player.GetPlayerStatAmount(PlayerStat.StatType.Gas) > GlobalManager.GasGoal) return;
			base.InteractableOnActionPress(player);
		}


		private void Interactable_OnTimeComplete(Player player)
		{
			if (hasBeenOpened) return;
			owner = player;
			player.ChangePlayerStat(PlayerStat.StatType.Gas, -GlobalManager.GasGoal);
			hasBeenOpened = true;
			OnCarAccessActionPressed?.Invoke(player);
		}

		private void Interactable_OnPlayerEnters(Player player)
		{
			if (occupied) return;
			if (hasBeenOpened)
			{
				player.Say("E - drive", 0);
			}
			else
			{
				if (player.GetPlayerStatAmount(PlayerStat.StatType.Gas) < GlobalManager.GasGoal)
				{
					player.Say("Need more gas...", 0);
				}
				else
				{
					player.Say("E - fill gas", 0);
				}
			}

			occupied = true;
		}
		private void Interactable_OnPlayerExits(Player player)
		{
			if (!occupied) return;
			occupied = false;
			player.StopSaying();
		}

	
	}
}