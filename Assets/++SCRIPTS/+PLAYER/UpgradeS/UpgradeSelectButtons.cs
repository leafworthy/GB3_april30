using System;
using UnityEngine;

public class UpgradeSelectButtons : MonoBehaviour
{
	public UpgradeSelectButton DefaultButton;
	public event Action OnExit;
	public event Action<Upgrade> OnUpgradeChosen;

	private UpgradeSelectButton currentlySelectedButton;
	private bool hasSelected;

	[SerializeField] private UpgradeSelectButton[] buttons;
	private Player _player;
	private bool hasInit;
	
	

	public void Init(Player player)
	{
		if (ResetButton(player)) return;
		Debug.Log("buttons initialized");
		player.Controller.UIAxis.OnLeft += OnLeft;
		player.Controller.UIAxis.OnRight += OnRight;
		player.Controller.Select.OnPress += OnSelect;
		player.Controller.Cancel.OnPress += OnCancel;
		

	}

	private bool ResetButton(Player player)
	{
		hasSelected = false;
		_player = player;
		if (hasInit) return true;
		hasInit = true;
		buttons[0] = DefaultButton;
		DeselectAllButtons();
		currentlySelectedButton = DefaultButton;
		currentlySelectedButton.Highlight();
		return false;
	}

	private void OnDisable()
	{
		if(!hasInit) return;
		hasInit = false;
		hasSelected = false;
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
		SFX.sounds.charSelect_deselect_sounds.PlayRandom();
		currentlySelectedButton.Deselect();
	}


	private void OnSelect(NewControlButton obj)
	{
		ChooseUpgrade();
	}

	

	private void ChooseUpgrade()
	{
		
		currentlySelectedButton.Select();
		currentlySelectedButton.Unhighlight();
		SFX.sounds.charSelect_select_sounds.PlayRandom();
		if( currentlySelectedButton.upgrade == null)
		{
			
			OnExit?.Invoke();
			return;
		}
		OnUpgradeChosen?.Invoke(currentlySelectedButton.upgrade);
	}


	private void OnRight(NewInputAxis obj)
	{
		if (hasSelected) return;
		Debug.Log("on right");
		SFX.sounds.charSelect_move_sounds.PlayRandom();
		currentlySelectedButton.Unhighlight();
		currentlySelectedButton = currentlySelectedButton.buttonToRight;
		currentlySelectedButton.Highlight();
	}

	private void OnLeft(NewInputAxis obj)
	{
		if (hasSelected) return;
		Debug.Log("on left");
		SFX.sounds.charSelect_move_sounds.PlayRandom();
		currentlySelectedButton.Unhighlight();
		currentlySelectedButton = currentlySelectedButton.buttonToLeft;
		currentlySelectedButton.Highlight();
	}
}