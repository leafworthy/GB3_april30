using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class CharacterButton : MonoBehaviour
	{
		[SerializeField] public Character character;
		[SerializeField] private HideRevealObjects titleStates;
		[SerializeField] private HideRevealObjects graphicStates;
		[SerializeField] private List<Player> playersWhoSelectedThisCharacter = new List<Player>();
		[SerializeField] private List<Player> playersWhoHighlightedThisCharacter = new List<Player>();
		[SerializeField] private ButtonState currentState;
		private CharacterSelectIndicators characterSelectIndicators;

		private void OnEnable()
		{
			playersWhoHighlightedThisCharacter.Clear();
			playersWhoSelectedThisCharacter.Clear();
			characterSelectIndicators = GetComponentInChildren<CharacterSelectIndicators>();
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
			if (player == null) return;

			playersWhoHighlightedThisCharacter.Add(player);

			UpdateState();
		}

		public void UnHighlightButton(Player player)
		{
			if (player == null) return;
			playersWhoHighlightedThisCharacter.Remove(player);
			UpdateState();
		}

		public void SelectCharacter(Player player)
		{
			Debug.Log("select character");
			if(playersWhoSelectedThisCharacter.Contains(player))
			{
				Debug.Log("double select character attempt");
				return;
			}
			playersWhoSelectedThisCharacter.Add(player);
			UnHighlightButton(player);
			characterSelectIndicators.Set(player.input.playerIndex, 2);
			UpdateState();
		}

		public void DeselectCharacter(Player player)
		{
			Debug.Log("deselect character");
			playersWhoSelectedThisCharacter.Remove(player);
			HighlightButton(player);
			characterSelectIndicators.Set(player.input.playerIndex, 1);
			UpdateState();
		}

		private void UpdateState()
		{
			if (playersWhoHighlightedThisCharacter.Count <= 0)
			{
				SetState(playersWhoSelectedThisCharacter.Count <= 0 ? ButtonState.Unhighlighted : ButtonState.Selected);
			}
			else
			{
				SetState(ButtonState.Highlighted);
			}

			UpdateIndicator();
		}

		public void SetPlayerColors()
		{

			characterSelectIndicators.SetColors();
		}

		private void UpdateIndicator()
		{
			//Debug.Log("update indicator function called by " + name, this);
			characterSelectIndicators = GetComponentInChildren<CharacterSelectIndicators>();
			characterSelectIndicators.SetAllUnhighlighted();
			foreach (var player in playersWhoHighlightedThisCharacter)
			{
				characterSelectIndicators.Set(player.input.playerIndex, 1);
				//Debug.Log("Button " + character + " is highlighted for player " + player.input.playerIndex);
			}

			foreach (var player in playersWhoSelectedThisCharacter)
			{
				characterSelectIndicators.Set(player.input.playerIndex, 2);
				//Debug.Log("Button " + character + " is selected for player " + player.input.playerIndex, this);
			}

		}

		private void SetState(ButtonState newState)
		{
			currentState = newState;
			switch (newState)
			{
				case ButtonState.Unhighlighted:
					titleStates.Set(0);
					graphicStates.Set(0);
					break;
				case ButtonState.Highlighted:
					titleStates.Set(1);
					graphicStates.Set(1);
					break;
				case ButtonState.Selected:
					titleStates.Set(2);
					graphicStates.Set(2);
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
