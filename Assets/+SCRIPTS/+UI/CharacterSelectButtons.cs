using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class CharacterSelectButtons : ServiceUser
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
			player.Controller.OnUIAxis_Left += OnLeft;
			player.Controller.OnUIAxis_Right += OnRight;
			player.Controller.OnSelect_Pressed += OnSelect;
			player.Controller.OnCancel_Pressed += OnCancel;
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
			_player.Controller.OnUIAxis_Left -= OnLeft;
			_player.Controller.OnUIAxis_Right -= OnRight;
			_player.Controller.OnSelect_Pressed -= OnSelect;
			_player.Controller.OnCancel_Pressed -= OnCancel;

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
			sfx.sounds.charSelect_deselect_sounds.PlayRandom();
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
			sfx.sounds.charSelect_select_sounds.PlayRandom();
			OnCharacterChosen?.Invoke(currentlySelectedButton.character);
		}

		private void OnRight(NewInputAxis obj)
		{
			if (hasSelected) return;
			sfx.sounds.charSelect_move_sounds.PlayRandom();
			hasJoined = true;
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToRight;
			currentlySelectedButton.Highlight();
		}

		private void OnLeft(NewInputAxis obj)
		{
			if (hasSelected) return;
			hasJoined = true;
			sfx.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToLeft;
			currentlySelectedButton.Highlight();
		}
	}
}
