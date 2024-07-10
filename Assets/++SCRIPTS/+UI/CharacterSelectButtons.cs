using System;
using UnityEngine;

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
		currentlySelectedButton.Deselect();
	}


	private void OnSelect(NewControlButton obj)
	{
		Debug.Log("select");
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
		Debug.Log("Joining");
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
		OnCharacterChosen?.Invoke(currentlySelectedButton.character);
	}

	private void OnRight(NewInputAxis obj)
	{
		if (hasSelected) return;
		hasJoined = true;
		currentlySelectedButton.Unhighlight();
		currentlySelectedButton = currentlySelectedButton.buttonToRight;
		Debug.Log(currentlySelectedButton.character.ToString());
		currentlySelectedButton.Highlight();
	}

	private void OnLeft(NewInputAxis obj)
	{
		if (hasSelected) return;
		hasJoined = true;
		currentlySelectedButton.Unhighlight();
		currentlySelectedButton = currentlySelectedButton.buttonToLeft;
		currentlySelectedButton.Highlight();
	}
}