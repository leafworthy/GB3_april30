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

		[SerializeField] private CharacterSelectButton[] buttons;
		private Player _player;

		public void Init(Player player)
		{
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
			Services.sfx.sounds.charSelect_deselect_sounds.PlayRandom();
			currentlySelectedButton.Deselect();
		}


		private void OnSelect(NewControlButton obj)
		{

			if (hasSelected)
			{
				ChooseCharacter();
				return;
			}

			SelectCharacter();
		}


		private void SelectCharacter()
		{
			hasSelected = true;
			currentlySelectedButton.Select();
		}

		private void ChooseCharacter()
		{
			currentlySelectedButton.Unhighlight();
			Services.sfx.sounds.charSelect_select_sounds.PlayRandom();
			OnCharacterChosen?.Invoke(currentlySelectedButton.character);
		}

		private void OnRight(NewInputAxis obj)
		{
			if (hasSelected) return;
			Services.sfx.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToRight;
			currentlySelectedButton.Highlight();
		}

		private void OnLeft(NewInputAxis obj)
		{
			if (hasSelected) return;
			Services.sfx.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToLeft;
			currentlySelectedButton.Highlight();
		}

		public void CleanUp()
		{
			hasSelected = false;
			DeselectAllButtons();
			currentlySelectedButton = DefaultButton;
			if (_player == null) return;
			_player.Controller.UIAxis.OnLeft -= OnLeft;
			_player.Controller.UIAxis.OnRight -= OnRight;
			_player.Controller.Select.OnPress -= OnSelect;
			_player.Controller.Cancel.OnPress -= OnCancel;
		}
	}
}
