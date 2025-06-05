using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class CharacterSelectButtons : MonoBehaviour
	{
		public CharacterSelectButton DefaultButton;
		public event Action<Character> OnCharacterChosen;

		private CharacterSelectButton currentlySelectedButton;
		private bool hasSelected;
		private bool hasJoined;

		[SerializeField] private CharacterSelectButton[] buttons;
		private Player _player;

		public void Init(Player player)
		{
			//Debug.Log("buttons initialized");
			hasSelected = false;
			_player = player;
			player.Controller.UIAxis.OnLeft += OnLeft;
			player.Controller.UIAxis.OnRight += OnRight;
			player.Controller.Select.OnPress += OnSelect;
			player.Controller.Cancel.OnPress += OnCancel;
			buttons[0] = DefaultButton;
			DeselectAllButtons();
			currentlySelectedButton = DefaultButton;
			currentlySelectedButton.Highlight();

		}

		private void OnDisable()
		{
			hasSelected = false;
			hasJoined = false;
			if(_player == null) return;
			_player.Controller.UIAxis.OnLeft -= OnLeft;
			_player.Controller.UIAxis.OnRight -= OnRight;
			_player.Controller.Select.OnPress -= OnSelect;
			_player.Controller.Cancel.OnPress -= OnCancel;
	
		}

		private void DeselectAllButtons()
		{
			foreach (var button in buttons)
			{
				button.Unhighlight();
				button.Deselect();
			}
		}

		private void OnCancel(NewControlButton obj)
		{
			if (!hasSelected) return;
			hasSelected = false;
			SFX.I.sounds.charSelect_deselect_sounds.PlayRandom();
			currentlySelectedButton.Deselect();
		}


		private void OnSelect(NewControlButton obj)
		{
			if (SelectButtonIsPressedFromJoining()) return;
			if (hasSelected)
			{
				ChooseCharacter();
				return;
			}

			SelectCharacter();
		}

		private bool SelectButtonIsPressedFromJoining()
		{
			if (hasJoined) return false;
			hasJoined = true;
			return true;

		}

		private void SelectCharacter()
		{
			hasSelected = true;
			currentlySelectedButton.Select();
		}

		private void ChooseCharacter()
		{
			currentlySelectedButton.Unhighlight();
			SFX.I.sounds.charSelect_select_sounds.PlayRandom();
			OnCharacterChosen?.Invoke(currentlySelectedButton.character);
		}

		private void OnRight(NewInputAxis obj)
		{
			if (hasSelected) return;
			SFX.I.sounds.charSelect_move_sounds.PlayRandom();
			hasJoined = true;
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToRight;
			currentlySelectedButton.Highlight();
		}

		private void OnLeft(NewInputAxis obj)
		{
			if (hasSelected) return;
			hasJoined = true;
			SFX.I.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToLeft;
			currentlySelectedButton.Highlight();
		}
	}
}