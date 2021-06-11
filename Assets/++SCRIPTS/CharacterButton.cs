using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SCRIPTS
{
	public class CharacterButton : MonoBehaviour
	{
		[SerializeField] public Character character;
		[SerializeField] private HideRevealObjects titleStates;
		[SerializeField] private HideRevealObjects graphicStates;
		[SerializeField] private List<Player> playersWhoSelectedThisCharacter = new List<Player>();
		[SerializeField] private List<Player> playersWhoHighlightedThisCharacter = new List<Player>();
		[SerializeField] private ButtonState currentState;
		[SerializeField] private Indicators indicators;

		private void Start()
		{
			UpdateIndicator();
		}

		private enum ButtonState
		{
			Unhighlighted,
			Highlighted,
			Selected
		}

		public void HighlightButton(Player player)
		{
			player.currentButton = this;
			playersWhoHighlightedThisCharacter.Add(player);

			UpdateState();
		}

		public void UnHighlightButton(Player player)
		{
			playersWhoHighlightedThisCharacter.Remove(player);
			UpdateState();
		}

		public void SelectCharacter(Player player)
		{
			playersWhoSelectedThisCharacter.Add(player);
			player.currentCharacter = character;
			player.hasSelectedCharacter = true;
			indicators.Select(player.playerIndex);
			UpdateState();
		}

		public void DeselectCharacter(Player player)
		{
			playersWhoSelectedThisCharacter.Remove(player);
			player.currentCharacter = character;
			player.hasSelectedCharacter = false;
			indicators.Select(player.playerIndex);
			UpdateState();
		}

		private void UpdateState()
		{
			if (playersWhoSelectedThisCharacter.Count <= 0)
			{
				if (playersWhoHighlightedThisCharacter.Count <= 0)
				{
					SetState(ButtonState.Unhighlighted);
				}
				else
				{
					SetState(ButtonState.Highlighted);

				}
			}
			else
			{
				SetState(ButtonState.Selected);
			}

			UpdateIndicator();
		}

		public void SetPlayerColors(List<Player> players)
		{
			var p1 = players.FirstOrDefault(p => p.playerIndex == PlayerIndex.One);
			var p2 = players.FirstOrDefault(p => p.playerIndex == PlayerIndex.Two);
			var p3 = players.FirstOrDefault(p => p.playerIndex == PlayerIndex.Three);
			var p4 = players.FirstOrDefault(p => p.playerIndex == PlayerIndex.Four);
			indicators.SetColors(p1.playerColor, p2.playerColor, p3.playerColor, p4.playerColor);
		}

		private void UpdateIndicator()
		{
			var p1 = playersWhoHighlightedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.One);
			var p2 = playersWhoHighlightedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.Two);
			var p3 = playersWhoHighlightedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.Three);
			var p4 = playersWhoHighlightedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.Four);
			indicators.HighlightTheTrueOnes(p1!=null, p2 != null, p3 != null, p4 != null);

			if (playersWhoSelectedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.One) != null)
			{
				indicators.Select(PlayerIndex.One);
			}

			if (playersWhoSelectedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.Two) != null)
			{
				indicators.Select(PlayerIndex.Two);
			}

			if (playersWhoSelectedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.Three) != null)
			{
				indicators.Select(PlayerIndex.Three);
			}

			if (playersWhoSelectedThisCharacter.FirstOrDefault(p => p.playerIndex == PlayerIndex.Four) != null)
			{
				indicators.Select(PlayerIndex.Four);
			}

		}

		private void SetState(ButtonState newState)
		{
			currentState = newState;
			switch (newState)
			{
				case ButtonState.Unhighlighted:
					titleStates.SetActiveObject(0);
					graphicStates.SetActiveObject(0);
					break;
				case ButtonState.Highlighted:
					titleStates.SetActiveObject(1);
					graphicStates.SetActiveObject(1);
					break;
				case ButtonState.Selected:
					titleStates.SetActiveObject(2);
					graphicStates.SetActiveObject(2);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
			}
		}

		public void CleanUp()
		{
			SetState(ButtonState.Unhighlighted);
			playersWhoHighlightedThisCharacter.Clear();
			playersWhoSelectedThisCharacter.Clear();
			UpdateIndicator();
		}
	}
}
