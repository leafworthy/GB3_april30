using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class CharacterSelectButtons : MonoBehaviour
	{
		public event Action<Character> OnCharacterChosen;
		CharacterSelectButton currentlyHighlightedButton;
		[SerializeField] CharacterSelectButton[] buttons;
		public GameObject visible;
		Player player;
		State currentState;
		enum State
		{
			not,
			highlighting,
			selected,
			chosen
		}

		void SetState(State state)
		{
			currentState = state;
		}

		public void SetPlayer(Player newPlayer)
		{
			Debug.Log( "[CharacterSelectButtons] SetPlayer called with newPlayer " + newPlayer.name, newPlayer);
			if (player == null) ListenToPlayer(newPlayer);
			visible.SetActive(true);
			StartHighlighting();


		}

		void CancelHighlighting()
		{
			SetState(State.not);
			visible.SetActive(false);
			DeselectAllButtons();
		}

		void CancelSelection()
		{
			SetState(State.highlighting);
			currentlyHighlightedButton.Unhighlight();
		}

		void HighlightFirstButton()
		{
			currentlyHighlightedButton = buttons[0];
			currentlyHighlightedButton.Highlight();
		}

		void ListenToPlayer(Player player)
		{
			this.player = player;
			if (this.player == null) return;
			Debug.Log( "listening to player " + this.player.playerIndex, this.player);
			this.player.Controller.UIAxis.OnLeft -= OnLeft;
			this.player.Controller.UIAxis.OnRight -= OnRight;
			this.player.Controller.Select.OnPress -= OnSelect;
			this.player.Controller.Cancel.OnPress -= OnCancel;

			this.player.Controller.UIAxis.OnLeft += OnLeft;
			this.player.Controller.UIAxis.OnRight += OnRight;
			this.player.Controller.Select.OnPress += OnSelect;
			this.player.Controller.Cancel.OnPress += OnCancel;
		}


		void DeselectAllButtons()
		{
			foreach (var button in buttons)
			{
				button.Unhighlight();
				button.Deselect();
			}
		}

		void OnCancel(NewControlButton obj)
		{
			switch (currentState)
			{
				case State.not:
					break;
				case State.highlighting:
					Services.sfx.sounds.charSelect_deselect_sounds.PlayRandom();
					CancelHighlighting();
					break;
				case State.selected:
					CancelSelection();
					StartHighlighting();
					Services.sfx.sounds.charSelect_deselect_sounds.PlayRandom();
					currentlyHighlightedButton.Deselect();
					currentlyHighlightedButton.Highlight();
					break;
				case State.chosen:
					break;
			}
		}

		void OnSelect(NewControlButton obj)
		{
			switch (currentState)
			{
				case State.not:
					StartHighlighting();
					break;
				case State.highlighting:
					SelectHighlighted();
					break;
				case State.selected:
					ChooseSelected();
					break;
				case State.chosen:
					break;
			}
		}

		void StartHighlighting()
		{
			DeselectAllButtons();
			SetState(State.highlighting);
			visible.SetActive(true);
			Services.sfx.sounds.pickup_speed_sounds.PlayRandom();HighlightFirstButton();
		}

		void SelectHighlighted()
		{
			SetState(State.selected);
			currentlyHighlightedButton.Select();
			Services.sfx.sounds.charSelect_select_sounds.PlayRandom();
		}

		void ChooseSelected()
		{
			SetState(State.chosen);
			currentlyHighlightedButton.Unhighlight();
			Services.sfx.sounds.press_start_sounds.PlayRandom();
			OnCharacterChosen?.Invoke(currentlyHighlightedButton.character);
			visible.SetActive(false);
		}

		void SetCurrentlySeletedButton(CharacterSelectButton newButton)
		{
			Debug.Log( "setting currently selected button to " + newButton.name, newButton);
			currentlyHighlightedButton.Unhighlight();
			currentlyHighlightedButton = newButton;
			currentlyHighlightedButton.Highlight();
			Services.sfx.sounds.charSelect_move_sounds.PlayRandom();
		}

		void OnLeft(NewInputAxis obj)
		{
			Debug.Log("OnLeft called with input in hudSlotState " + currentState);
			if (currentState != State.highlighting) return;
			SetCurrentlySeletedButton(currentlyHighlightedButton.buttonToLeft);
		}

		void OnRight(NewInputAxis obj)
		{
			Debug.Log( "OnRight called with input in hudSlotState " + currentState);
			if (currentState != State.highlighting) return;
			SetCurrentlySeletedButton(currentlyHighlightedButton.buttonToRight);
		}

		public void ResetCharacterSelection()
		{
			visible.SetActive(true);
			StartHighlighting();
			DeselectAllButtons();
			HighlightFirstButton();
		}
	}
}
