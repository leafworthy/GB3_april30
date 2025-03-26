using System;
using TMPro;
using UnityEngine;

namespace UpgradeS
{
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
		public TextMeshProUGUI upgradeDescription;
	
	

		public void Init(Player player)
		{
			if (hasInit) return;
			_player = player;
			ResetButton();
			Debug.Log("buttons initialized");
			player.Controller.Attack3Circle.OnPress += OnCircleButton;
			player.Controller.UIAxis.OnLeft += OnLeft;
			player.Controller.UIAxis.OnRight += OnRight;
			player.Controller.Select.OnPress += OnSelect;
			player.Controller.Cancel.OnPress += OnCancel;
		

		}

		private void OnCircleButton(NewControlButton obj)
		{
			OnExit?.Invoke();
		}

		private void ResetButton()
		{
			hasSelected = false;
			
			hasInit = true;
			buttons[0] = DefaultButton;
			DeselectAllButtons();
			SetCurrentButton(DefaultButton);
		}

		private void SetCurrentButton(UpgradeSelectButton defaultButton)
		{
			currentlySelectedButton = defaultButton;
			currentlySelectedButton.Highlight();
			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
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
			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
		}

		private void OnLeft(NewInputAxis obj)
		{
			if (hasSelected) return;
			Debug.Log("on left");
			SFX.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToLeft;
			currentlySelectedButton.Highlight();
			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
		}
	}
}